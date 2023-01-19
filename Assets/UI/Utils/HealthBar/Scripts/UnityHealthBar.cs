using T4.Utils;
using UnityEngine;

namespace T4.UI.Match.Info
{
    public class UnityHealthBar : HealthBar
    {
        private float yOffset;

        private Transform target;
        private Vector3 lastTargetPosition;

        private Camera _camera;
        private Vector3 lastCameraPosition;
        private float lastCameraSize;

        protected override void Awake()
        {
            _camera = Camera.main;
            lastCameraSize = _camera.orthographicSize;
        }

        private void Update()
        {
            if (!target) return;

            if (lastCameraPosition != _camera.transform.position || lastTargetPosition != target.position)
            {
                SetPosition();
                if (!_camera.orthographic)
                {
                    CalculateOffSet();
                }
            }
            if (_camera.orthographic && lastCameraSize != _camera.orthographicSize)
            {
                CalculateOffSet();
            }
        }

        public void Initialize(Transform target)
        {
            this.target = target;
            CalculateOffSet();
            transform.SetParent(GameObject.Find("Canvas").transform);
        }

        private void CalculateOffSet()
        {
            float offSet = RectUtils.WorldBoundsToScreenRect(
                _camera,
                target.Find("Mesh").GetComponent<Renderer>().bounds
            ).height;
            yOffset = offSet;
        }

        public void SetPosition()
        {
            if (!target) return;
            Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
            pos.y += yOffset;
            gameObject.transform.position = pos;
            lastTargetPosition = target.position;
            lastCameraPosition = _camera.transform.position;
        }
    }
}
