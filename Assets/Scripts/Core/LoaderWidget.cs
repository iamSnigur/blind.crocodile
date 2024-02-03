using System.Collections;
using UnityEngine;

namespace BlindCrocodile.Core
{
    public class LoaderWidget : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvas;
        [Range(0.1f, 3f)][SerializeField] private float _timeToFill;
        [Range(0.1f, 3f)][SerializeField] private float _timeToFade;

        private void Awake() => 
            DontDestroyOnLoad(this);

        public void Show()
        {
            _canvas.alpha = 1f;
            gameObject.SetActive(true);
        }

        public void Hide() =>
            StartCoroutine(FadeIn());

        private IEnumerator FadeIn()
        {
            float fadeStep = Time.fixedDeltaTime / _timeToFade;

            while (_canvas.alpha > 0f)
            {
                _canvas.alpha -= fadeStep;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            gameObject.SetActive(false);
        }
    }
}