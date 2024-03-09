using UnityEngine;
using UnityEngine.EventSystems;

namespace BlindCrocodile.UI
{
    public class MaskController : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _mask;

        public float Speed;

        public Vector2 MinSize;
        public Vector2 MaxSize;

        private bool _pressed;
        private float _pressedTime;

        private bool _isBlocked;

        private void Update()
        {
            if (_isBlocked)
                return;

            if (_pressed)
            {
                _pressedTime += Time.deltaTime * Speed;
            }
            else
            {
                _pressedTime -= 2 * Time.deltaTime * Speed;
            }
            _pressedTime = Mathf.Clamp01(_pressedTime);

            float lerp = _pressed ? EaseOutElastic(_pressedTime) : EaseOutBack(_pressedTime);

            _mask.localScale = Vector2.LerpUnclamped(MinSize, MaxSize, lerp);
        }

        public void SetBlocked(bool blocked) => 
            _isBlocked = blocked;

        private float EaseOutElastic(float time)
        {
            float c4 = (2 * Mathf.PI) / 3;

            return time == 0
              ? 0
              : time == 1
              ? 1
              : Mathf.Pow(2, -10 * time) * Mathf.Sin((time * 10 - 0.75f) * c4) + 1;
        }

        private static float EaseOutBack(float time)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return 1 + c3 * Mathf.Pow(time - 1, 3) + c1 * Mathf.Pow(time - 1, 2);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            _mask.position = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pressed = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
        }
    }
}