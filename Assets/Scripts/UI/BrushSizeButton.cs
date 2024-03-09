using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlindCrocodile.UI
{
    public class BrushSizeButton : MonoBehaviour
    {
        public event Action<float> OnSizeChanged;

        [SerializeField] private Button _button;
        [SerializeField] private float _size;

        private void Awake() =>
            _button.onClick.AddListener(Notify);

        private void OnDestroy() =>
            _button.onClick.RemoveListener(Notify);

        private void Notify() =>
            OnSizeChanged?.Invoke(_size);
    }
}