using BlindCrocodile.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace BlindCrocodile.Gameplay.Drawing
{
    public class Drawer : MonoBehaviour
    {
        private static readonly int _PaintColorID = Shader.PropertyToID("_PaintColor");
        private static readonly int _PaintPosID = Shader.PropertyToID("_PaintPos");
        private static readonly int _BrushSizeID = Shader.PropertyToID("_BrushSize");

        public Vector2 CanvasSize { get; private set; }
        public RenderTexture CanvasTexture => _canvasRT;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Material _paintMaterial;
        [SerializeField] private RawImage _canvas;

        [SerializeField] private BrushColorButton[] _brushColorButtons;
        [SerializeField] private BrushSizeButton[] _brushSizeButtons;

        private Color _color;
        private float _brushSize = 15f;

        private Vector2 _previousMousePos;
        private RenderTexture _tmpRT;
        private RenderTexture _canvasRT;
        private bool _isBlocked;
        private CommandBuffer _canvasCommandBuffer;

        private void Start()
        {
            for (int i = 0; i < _brushColorButtons.Length; i++)
                _brushColorButtons[i].OnColorChanged += SetColor;

            for (int i = 0; i < _brushSizeButtons.Length; i++)
                _brushSizeButtons[i].OnSizeChanged += SetSize;

            _canvasCommandBuffer = new CommandBuffer();

            CanvasSize = _rectTransform.sizeDelta;
            _canvasRT = RenderTexture.GetTemporary((int)CanvasSize.x, (int)CanvasSize.y);
            _canvasRT.filterMode = FilterMode.Point;

            _canvasCommandBuffer.SetRenderTarget(_canvasRT);
            ClearCanvas(Color.white);

            _tmpRT = RenderTexture.GetTemporary(_canvasRT.descriptor);
            _canvas.texture = _canvasRT;
        }

        private void OnDestroy()
        {
            _canvasCommandBuffer.Dispose(); // null ref
            _tmpRT.Release();
            _canvasRT.Release();

            for (int i = 0; i < _brushColorButtons.Length; i++)
                _brushColorButtons[i].OnColorChanged -= SetColor;

            for (int i = 0; i < _brushSizeButtons.Length; i++)
                _brushSizeButtons[i].OnSizeChanged -= SetSize;
        }

        private void Update()
        {
            if (_isBlocked)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform,
                Input.mousePosition,
                null,
                out Vector2 currentMouse);

            currentMouse += CanvasSize / 2f;

            if (Input.GetMouseButton(0))
            {
                _paintMaterial.SetColor(_PaintColorID, _color);
                _paintMaterial.SetFloat(_BrushSizeID, _brushSize);
                PaintCanvas(currentMouse);
            }

            if (Input.GetMouseButton(1))
                ClearCanvas(Color.white);

            _previousMousePos = currentMouse;
        }

        public void SetColor(Color color) =>
            _color = color;

        public void SetSize(float size) =>
            _brushSize = size;

        public void SetBlocked(bool blocked) =>
            _isBlocked = blocked;

        public Color[] GetCanvasColors()
        {
            Texture2D texture2D = new((int)CanvasSize.x, (int)CanvasSize.y);

            RenderTexture.active = _canvasRT;
            texture2D.ReadPixels(new Rect(Vector2.zero, CanvasSize), 0, 0);
            texture2D.Apply();

            Color[] colors = texture2D.GetPixels();
            Destroy(texture2D);

            return colors;
        }

        public byte[] GetCanvasBytes()
        {
            Texture2D texture2D = new((int)CanvasSize.x, (int)CanvasSize.y);

            RenderTexture.active = _canvasRT;
            texture2D.ReadPixels(new Rect(Vector2.zero, CanvasSize), 0, 0);
            texture2D.Apply();

            byte[] png = texture2D.EncodeToPNG();
            Destroy(texture2D);

            return png;
        }

        public void CreateFromBytes(byte[] textureBytes)
        {
            Texture2D texture = new((int)CanvasSize.x, (int)CanvasSize.y);
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

        private void ClearCanvas(Color color)
        {
            _canvasCommandBuffer.ClearRenderTarget(true, true, color);
            Graphics.ExecuteCommandBuffer(_canvasCommandBuffer);
            _canvasCommandBuffer.Clear();
        }
    }
}