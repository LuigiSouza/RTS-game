using T4.Managers;
using UnityEngine;

namespace T4.Fog
{
    public class FogObjectRenderer : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        private float threshold = 0.1f;

        [SerializeField]
        private Renderer _renderer;
        private Camera unexploredCam;

        private static Texture2D shadowTexture;
        private static Rect rect;
        private static bool isDirty = true;

        private void Awake()
        {
            unexploredCam = CameraManager.Instance.UnexploredCamera;

            if(_renderer == null)
            {
                enabled = false;
            }
        }

        private Color GetColorAtPosition()
        {
            if (!unexploredCam)
            {
                return Color.white;
            }

            RenderTexture renderTexture = unexploredCam.targetTexture;
            if (!renderTexture)
            {
                return unexploredCam.backgroundColor;
            }

            if (shadowTexture == null || renderTexture.width != rect.width || renderTexture.height != rect.height)
            {
                rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
                shadowTexture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            }

            if (isDirty)
            {
                RenderTexture.active = renderTexture;
                shadowTexture.ReadPixels(rect, 0, 0);
                RenderTexture.active = null;
                isDirty = false;
            }

            Vector3 pixel = unexploredCam.WorldToScreenPoint(transform.position);
            return shadowTexture.GetPixel((int)pixel.x, (int)pixel.y);
        }

        private void Update()
        {
            isDirty = true;
        }

        void LateUpdate()
        {
            _renderer.enabled = GetColorAtPosition().grayscale >= threshold;
        }
    }
}