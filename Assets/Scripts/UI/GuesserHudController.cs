using BlindCrocodile.Gameplay.Drawing;
using BlindCrocodile.Services.LobbyFactory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlindCrocodile.UI
{
    public class GuesserHudController : MonoBehaviour
    {
        [SerializeField] private MaskController _maskController;
        [SerializeField] private Drawer _canvasDrawer;
        [SerializeField] private Button _compareButton;
        [SerializeField] private Button _backGuessingButton;
        [SerializeField] private GameObject _guessingPanel;
        [SerializeField] private GameObject _drawingPanel;
        [SerializeField] private Transform _colorStatItemsParent;
        [SerializeField] private ComparisonController _comparisonController;

        private ILobbyFactory _lobbyFactory;
        private List<ColorStatItem> _colorStatItems;

        private void Awake()
        {
            _colorStatItems = new List<ColorStatItem>();
            _compareButton.onClick.AddListener(OnCompare);
            _backGuessingButton.onClick.AddListener(OnBackGuessing);

            _canvasDrawer.SetBlocked(false);
            _maskController.SetBlocked(true);
        }

        public void Construct(ILobbyFactory lobbyFactory, byte[] artistCanvasBytes)
        {
            _lobbyFactory = lobbyFactory;
            _comparisonController.Construct(artistCanvasBytes, _canvasDrawer.CanvasSize);
        }

        private void OnCompare()
        {
            _drawingPanel.SetActive(false);
            _compareButton.gameObject.SetActive(false);

            _guessingPanel.SetActive(true);
            _backGuessingButton.gameObject.SetActive(true);

            _canvasDrawer.SetBlocked(true);
            _maskController.SetBlocked(false);

            _comparisonController.CompareTo(_canvasDrawer.CanvasTexture);

            Color[] canvasColors = _canvasDrawer.GetCanvasColors();

            Dictionary<Color, int> uniqueColors = new();

            foreach (Color color in canvasColors) 
            {
                if (uniqueColors.ContainsKey(color))
                    uniqueColors[color]++;
                else
                    uniqueColors.Add(color, 1);
            }

            foreach (ColorStatItem item in _colorStatItems)
                Destroy(item.gameObject);
            
            _colorStatItems.Clear();

            foreach (KeyValuePair<Color, int> color in uniqueColors)
            {
                float colorAmount = (float)color.Value / canvasColors.Length;
                ColorStatItem colorStatItem = _lobbyFactory.CreateColorStatItem(color.Key, colorAmount, _colorStatItemsParent);
                _colorStatItems.Add(colorStatItem);
            }
        }

        private void OnBackGuessing()
        {
            _drawingPanel.SetActive(true);
            _compareButton.gameObject.SetActive(true);

            _guessingPanel.SetActive(false);
            _backGuessingButton.gameObject.SetActive(false);

            _canvasDrawer.SetBlocked(false);
            _maskController.SetBlocked(true);


        }
    }
}