using BlindCrocodile.Lobbies;
using TMPro;
using UnityEngine;

namespace BlindCrocodile.UI
{
    public class PlayerHudItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] private GameObject _hostMark;
        [SerializeField] private GameObject _readyMark;

        public void Construct(LocalPlayer localPlayer)
        {
            _nameLabel.text = localPlayer.Name;
            _hostMark.SetActive(localPlayer.IsHost);
        }
    }
}
