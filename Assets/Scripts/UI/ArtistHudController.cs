using BlindCrocodile.Gameplay.Drawing;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlindCrocodile.UI
{
    public class ArtistHudController : MonoBehaviour
    {
        public event Action<byte[]> OnCanvasShared;

        [SerializeField] private Drawer _canvasDrawer;
        [SerializeField] private Button _shareButton;

        private void Awake()
        {
            _shareButton.onClick.AddListener(OnShare);
            _shareButton.interactable = true;
        }

        private void OnShare()
        {
            _shareButton.interactable = false;
            byte[] canvas = _canvasDrawer.GetCanvasBytes();
            OnCanvasShared?.Invoke(canvas);
        }
    }
}