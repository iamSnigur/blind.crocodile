using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlindCrocodile.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CopyableText : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _label;

        public void OnPointerClick(PointerEventData eventData)
        {
            GUIUtility.systemCopyBuffer = _label.text;
            Debug.Log($"{_label.text} Copied!");
        }
    }
}
