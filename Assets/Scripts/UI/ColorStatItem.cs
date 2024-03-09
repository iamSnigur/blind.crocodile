using BlindCrocodile.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace BlindCrocodile.UI
{
    public class ColorStatItem : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _colorProgressImage;
        [SerializeField] private GameObject _overflowObject;

        public void Construct(Color color, float amount)
        {
            _backgroundImage.color = GetBackgroundColor(color);
            _colorProgressImage.color = color;
            _colorProgressImage.fillAmount = amount;
            _overflowObject.SetActive(amount > 1f);            
        }

        private Color GetBackgroundColor(Color color)
        {
            Vector3 hsv = color.RGBToHSV();
            hsv.z = Mathf.Clamp01(hsv.z - 0.2f);

            return hsv.HSVToRGB();
        }
    }
}