using System;
using T4.Events;
using T4.Globals;
using T4.Minimap;
using T4.UI.Borders;
using T4.Utils;
using UnityEngine;

namespace T4.Managers
{
    public enum CameraDirections
    {
        TOP,
        RIGHT,
        LEFT,
        BOTTOM,
        NONE
    }

    [RequireComponent(typeof(Camera))]
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager _instance;
        public static CameraManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(CameraManager)) as CameraManager;
                    if (!_instance)
                    {
                        Debug.LogError($"There needs to be one active {typeof(CameraManager)} script on a GameObject in your scene.");
                    }
                }

                return _instance;
            }
        }
        [Header("Cameras")]
        [SerializeField]
        private Camera unexploredCamera;
        [SerializeField]
        private Camera exploredCamera;

        public Camera UnexploredCamera { get { return unexploredCamera; } }
        public Camera ExploredCamera { get { return exploredCamera; } }

        [Header("Camera Configs")]
        [SerializeField]
        private float translationSpeed = 20f;
        [SerializeField]
        private float zoomSpeed = 60f;

        [SerializeField]
        private MinimapIndicator minimapIndicator;

        private const float MIN_DIST = 6;
        private const float MAX_DIST = 16;
        private float staticCamHeight;

        private CameraDirections mouseScreenBorder = CameraDirections.NONE;

        private Camera _camera;
        private Vector3 forward;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            forward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            staticCamHeight = _camera.transform.position.y;

            EventManager.Instance.AddListener<MoveCameraEventHandler>(OnMoveCamera);
        }

        private void Start()
        {
            if (minimapIndicator != null)
            {
                minimapIndicator.ComputeIndicatorMesh();
                minimapIndicator.TranslateToCenter();
            }
        }

        private void Update()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                ZoomCamera(Input.mouseScrollDelta.y > 0 ? 1 : -1);
            }

            if (mouseScreenBorder != CameraDirections.NONE)
            {
                TranslateCamera(mouseScreenBorder);
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    TranslateCamera(CameraDirections.TOP);
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    TranslateCamera(CameraDirections.RIGHT);
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    TranslateCamera(CameraDirections.BOTTOM);
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    TranslateCamera(CameraDirections.LEFT);
                }
            }
        }

        private void ClampCamera()
        {
            float x = GlobalConfigs.Instance.TerrainSize.x;
            float z = GlobalConfigs.Instance.TerrainSize.z;

            Vector3 center = RectUtils.MiddleOfScreenPointToWorld(); center.y = 0;
            Vector3 offSet = _camera.transform.position - center;
            Vector3 newPos = new(Math.Clamp(center.x, 0, x), 0, Math.Clamp(center.z, 0, z));
            newPos += offSet;
            if (_camera.orthographic)
            {
                newPos.y = staticCamHeight;
            }

            _camera.transform.position = newPos;
        }

        private void ZoomCamera(int zoom)
        {
            if (_camera.orthographic)
            {
                _camera.orthographicSize -= zoom * Time.deltaTime * zoomSpeed;
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, MIN_DIST, MAX_DIST);
            }
            else
            {
                _camera.transform.Translate(_camera.transform.forward * zoom * Time.deltaTime * zoomSpeed, Space.World);
                Vector3 newPos = _camera.transform.position; newPos.y = Mathf.Clamp(newPos.y, MIN_DIST, MAX_DIST);
                _camera.transform.position = newPos;
            }

            ClampCamera();
            if (minimapIndicator != null)
            {
                minimapIndicator.ComputeIndicatorMesh();
            }
        }

        private void TranslateCamera(CameraDirections dir)
        {
            switch (dir)
            {
                case CameraDirections.TOP:
                    _camera.transform.Translate(forward * Time.deltaTime * translationSpeed, Space.World);
                    break;
                case CameraDirections.LEFT:
                    _camera.transform.Translate(-_camera.transform.right * Time.deltaTime * translationSpeed);
                    break;
                case CameraDirections.RIGHT:
                    _camera.transform.Translate(_camera.transform.right * Time.deltaTime * translationSpeed);
                    break;
                case CameraDirections.BOTTOM:
                    _camera.transform.Translate(-forward * Time.deltaTime * translationSpeed, Space.World);
                    break;
                default:
                    throw new NotImplementedException("Direção não implementada");
            }

            ClampCamera();
            if (minimapIndicator != null)
            {
                minimapIndicator.TranslateToCenter();
            }
        }

        private void OnMoveCamera(MoveCameraEventHandler e)
        {
            Vector3 pos = e.position;
            pos.x = Math.Clamp(pos.x, 0, GlobalConfigs.Instance.TerrainSize.x);
            pos.y = 0;
            pos.z = Math.Clamp(pos.z, 0, GlobalConfigs.Instance.TerrainSize.z);
            Vector3 off = _camera.transform.position - RectUtils.MiddleOfScreenPointToWorld();
            off.y = _camera.transform.position.y;
            Vector3 newPos = pos + off;
            if (_camera.orthographic)
            {
                newPos.y = staticCamHeight;
            }

            _camera.transform.position = newPos;

            ClampCamera();
            if (minimapIndicator != null)
            {
                minimapIndicator.TranslateToCenter();
            }
        }

        public void OnMouseEnterScreenBorder(ScreenBorder border)
        {
            mouseScreenBorder = border.direction;
        }

        public void OnMouseExitScreenBorder()
        {
            mouseScreenBorder = CameraDirections.NONE;
        }

    }
}