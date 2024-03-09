using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlindCrocodile.UI
{
    public class BrushColorButton : MonoBehaviour
    {
        public event Action<Color> OnColorChanged;

        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        private void Awake() =>
            _button.onClick.AddListener(Notify);

        private void OnDestroy() =>
            _button.onClick.RemoveListener(Notify);

        private void Notify() =>
            OnColorChanged?.Invoke(_image.color);
    }
}