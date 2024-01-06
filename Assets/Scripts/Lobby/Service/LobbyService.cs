using BlindCrocodile.Core;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using IUnityLobbyService = Unity.Services.Lobbies.ILobbyService;
using Unity.Services.Authentication;

namespace BlindCrocodile.Lobbies
{
    public class LobbyService : ILobbyService
    {
        public LocalLobby LocalLobby { get; private set; }
        public LocalPlayer LocalPlayer { get; private set; }

        private readonly IUnityLobbyService _unityLobbyService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IStateMachine _gameStateMachine;

        private Lobby _remoteLobby;
        private ILobbyEvents _lobbyEvents;
        private Coroutine _hearbeatCoroutine;

        public LobbyService(IUnityLobbyService unityLobbyService, ICoroutineRunner coroutineRunner, IStateMachine gameStateMachine)
        {
            _unityLobbyService = unityLobbyService;
            _coroutineRunner = coroutineRunner;
            _gameStateMachine = gameStateMachine;

            LocalPlayer = new LocalPlayer() { Id = AuthenticationService.Instance.PlayerId }; // refactor
            LocalLobby = new LocalLobby();
            LocalLobby.Reset(LocalPlayer);
        }

        public async Task CreateLobbyAsync(string lobbyName, int maxConnections)
        {
            CreateLobbyOptions options = new()
            {
                IsLocked = true,
                IsPrivate = true,
                Player = new(id: LocalPlayer.Id, data: LocalPlayer.GetDataForRemoteLobby()),
            };

            try
            {
                _remoteLobby = await _unityLobbyService.CreateLobbyAsync(lobbyName, maxConnections, options);

                LocalLobby.SetRemoteData(_remoteLobby);
                _hearbeatCoroutine = _coroutineRunner.StartCoroutine(HeartbeatCoroutine(10f));
                SubscribeToLobbyEvents();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
                _gameStateMachine.Enter<LoadMenuState>();
            }
        }

        public async Task JoinLobbyByCodeAsync(string lobbyCode)
        {
            JoinLobbyByCodeOptions options = new()
            {
                Player = new(id: LocalPlayer.Id, data: LocalPlayer.GetDataForRemoteLobby())
            };

            try
            {
                _remoteLobby = await _unityLobbyService.JoinLobbyByCodeAsync(lobbyCode, options);

                LocalLobby.SetRemoteData(_remoteLobby);
                SubscribeToLobbyEvents();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
                _gameStateMachine.Enter<LoadMenuState>();
            }
        }

        public void DisconnectFromLobby()
        {
            UnsubscribeToLobbyEvents();

            if (_remoteLobby == null)
                return;

            if (LocalPlayer.IsHost)
            {
                _coroutineRunner.StopCoroutine(_hearbeatCoroutine);
                DeleteLobbyAsync();
            }
            else
                LeaveLobbyAsync();
        }

        public async Task UpdateRemotePlayerDataAsync(string allocationId, string relayConnectionInfo)
        {
            Dictionary<string, PlayerDataObject> data = LocalPlayer.GetDataForRemoteLobby();

            UpdatePlayerOptions options = new()
            {
                Data = data,
                AllocationId = allocationId,
                ConnectionInfo = relayConnectionInfo
            };

            try
            {
                _remoteLobby = await _unityLobbyService.UpdatePlayerAsync(LocalLobby.Id, LocalPlayer.Id, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        public async Task UpdateRemoteLobbyDataAsync()
        {
            Dictionary<string, DataObject> data = LocalLobby.GetDataForRemoteLobby();

            UpdateLobbyOptions options = new()
            {
                Data = data,
                IsLocked = false,
            };

            try
            {
                _remoteLobby = await _unityLobbyService.UpdateLobbyAsync(LocalLobby.Id, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        private async void SubscribeToLobbyEvents()
        {
            LobbyEventCallbacks callbacks = new();
            callbacks.LobbyChanged += OnRemoteLobbyChanged;

            _lobbyEvents = await _unityLobbyService.SubscribeToLobbyEventsAsync(LocalLobby.Id, callbacks);
        }

        private async void UnsubscribeToLobbyEvents() =>
            await _lobbyEvents.UnsubscribeAsync();

        private void OnRemoteLobbyChanged(ILobbyChanges changes)
        {
            if (changes.LobbyDeleted)
            {
                ResetLobby();
                DisconnectFromLobby();
                _gameStateMachine.Enter<LoadMenuState>();

                return;
            }

            changes.ApplyToLobby(_remoteLobby);
            LocalLobby.SetRemoteData(_remoteLobby);
        }

        private async void DeleteLobbyAsync()
        {
            try
            {
                await _unityLobbyService.DeleteLobbyAsync(LocalLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                ResetLobby();
            }
        }

        private async void LeaveLobbyAsync()
        {
            try
            {
                await _unityLobbyService.RemovePlayerAsync(LocalLobby.Id, LocalPlayer.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                ResetLobby();
            }
        }

        private void ResetLobby()
        {
            _remoteLobby = null;
            LocalPlayer.Reset();
            LocalLobby.Reset(LocalPlayer);
        }

        private IEnumerator HeartbeatCoroutine(float delaySeconds)
        {
            WaitForSeconds delay = new(delaySeconds);

            while (true)
            {
                _unityLobbyService.SendHeartbeatPingAsync(LocalLobby.Id);
                yield return delay;
            }
        }
    }
}
