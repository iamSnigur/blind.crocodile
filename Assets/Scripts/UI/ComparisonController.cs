using UnityEngine;
using UnityEngine.UI;

namespace BlindCrocodile.UI
{
    public class ComparisonController : MonoBehaviour
    {
        private const string ORIGINAL_TEX = "_originalTex";

        [SerializeField] private Material _comparisonMaterial;
        [SerializeField] private RawImage _heatmap;

        private RenderTexture _heatmapTexture;
        private Texture2D _original;

        public void Construct(byte[] artistCanvasBytes, Vector2 canvasSize)
        {
            _original = new((int)canvasSize.x, (int)canvasSize.y);
            _original.LoadImage(artistCanvasBytes);
            _comparisonMaterial.SetTexture(ORIGINAL_TEX, _original);

            _heatmapTexture = RenderTexture.GetTemporary((int)canvasSize.x, (int)canvasSize.y);
            _heatmapTexture.filterMode = FilterMode.Point;
            _heatmap.texture = _heatmapTexture;
        }

        public void CompareTo(RenderTexture replica)
        {
            // generate colors here

            _heatmapTexture.Release();            
            Graphics.Blit(replica, _heatmapTexture, _comparisonMaterial);
        }
    }
}