using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlindCrocodile.Gameplay.Drawing
{
    public class Drawer : MonoBehaviour
    {
        private static readonly int _PaintColorID = Shader.PropertyToID("_PaintColor");
        private static readonly int _PaintPosID = Shader.PropertyToID("_PaintPos");
        private static readonly int _BrushSizeID = Shader.PropertyToID("_BrushSize");

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Material _paintMaterial;
        [SerializeField] private RawImage _canvas;
        [SerializeField] private Color _color;
        [Range(5f, 25f)][SerializeField] private float _brushSize = 15f;

        private Vector2 _previousMousePos;
        private Vector2 _canvasSize;
        private RenderTexture _tmpRT;
        private RenderTexture _canvasRT;

        private void Start()
        {
            _canvasSize = _rectTransform.sizeDelta;

            _canvasRT = RenderTexture.GetTemporary((int)_canvasSize.x, (int)_canvasSize.y);
            _canvasRT.filterMode = FilterMode.Point;

            _tmpRT = RenderTexture.GetTemporary(_canvasRT.descriptor);

            _canvas.texture = _canvasRT;
        }

        private void Update()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform,
                Input.mousePosition,
                null,
                out Vector2 currentMouse);
            currentMouse += _canvasSize / 2f;

            if (Input.GetMouseButton(0))
            {
                _paintMaterial.SetColor(_PaintColorID, _color);
                _paintMaterial.SetFloat(_BrushSizeID, _brushSize);
                PaintCanvas(currentMouse);
            }

            if (Input.GetMouseButton(1))
                _canvasRT.Release();

            _previousMousePos = currentMouse;
        }

        public byte[] GetCanvasBytes()
        {
            Texture2D texture2D = new((int)_canvasSize.x, (int)_canvasSize.y);

            RenderTexture.active = _canvasRT;
            texture2D.ReadPixels(new Rect(Vector2.zero, _canvasSize), 0, 0);
            texture2D.Apply();

            byte[] png = texture2D.EncodeToPNG();
            Debug.Log($"Texture length: {png.Length}");
            Destroy(texture2D);

            return png;
        }

        public void CreateFromBytes(byte[] textureBytes)
        {            
            Texture2D texture = new((int)_canvasSize.x, (int)_canvasSize.y);
            texture.LoadImage(textureBytes);
            texture.Apply();
            Graphics.Blit(texture, _canvasRT);
            Destroy(texture);
        }

        private void PaintCanvas(Vector2 mousePos)
        {
            float distance = Vector2.Distance(mousePos, _previousMousePos);
            float lerpStep = _brushSize * 0.5f / distance;

            for (float lerp = 0; lerp <= 1f; lerp += lerpStep)
            {
                Vector2 pos = Vector2.Lerp(_previousMousePos, mousePos, lerp);
                _paintMaterial.SetVector(_PaintPosID, pos);
                Graphics.Blit(_canvasRT, _tmpRT, _paintMaterial);
                Graphics.CopyTexture(_tmpRT, _canvasRT);
            }
        }
    }

    // paint material 


    // service for drawing - set brush and paint position
    // drawing UI controller - change brush size and color | support more features like: bucket, lines ext.
}