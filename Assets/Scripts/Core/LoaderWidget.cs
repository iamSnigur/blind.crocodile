using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BlindCrocodile.Core
{
    public class LoaderWidget : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private Image _topCircle;
        [SerializeField] private Image _bottomCircle;
        [Range(0.1f, 3f)][SerializeField] private float _timeToFill;
        [Range(0.1f, 3f)][SerializeField] private float _timeToFade;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            StartCoroutine(WidgetAnimationCoroutine());
        }

        public void Show()
        {
            _canvas.alpha = 1f;
            gameObject.SetActive(true);
            StartCoroutine(WidgetAnimationCoroutine());
        }

        public void Hide() =>
            StartCoroutine(FadeIn());

        private IEnumerator WidgetAnimationCoroutine()
        {
            float fillStep = Time.deltaTime / _timeToFill;
            _bottomCircle.gameObject.SetActive(true);

            while (true)
            {
                _bottomCircle.fillAmount += fillStep;

                if (_bottomCircle.fillAmount >= 1)
                    _topCircle.fillAmount += fillStep;

                if (_topCircle.fillAmount >= 1)
                {
                    _bottomCircle.fillAmount = 0;
                    _topCircle.fillAmount = 0;
                }

                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }

        private IEnumerator FadeIn()
        {
            float fadeStep = Time.deltaTime / _timeToFade;
            _bottomCircle.gameObject.SetActive(false);

            while (_canvas.alpha > 0f)
            {
                _canvas.alpha -= fadeStep;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            gameObject.SetActive(false);
        }
    }
}