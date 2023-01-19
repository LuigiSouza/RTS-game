using UnityEngine.EventSystems;
using UnityEngine;
using T4.Managers;
using T4.Events;
using T4.Globals;

namespace T4.Minimap
{
    public class Minimap : UIBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Vector3 terrainSize;
        private Vector2 sizeDelta;

        private Vector2 previousMouse;
        private bool dragging = false;

        protected override void Start()
        {
            terrainSize = GlobalConfigs.Instance.TerrainSize;
            sizeDelta = GetComponent<RectTransform>().sizeDelta;
            previousMouse = Input.mousePosition;
        }

        private void Update()
        {
            if (!dragging) return;

            Vector2 delta = (Vector2)Input.mousePosition - previousMouse;
            previousMouse = Input.mousePosition;


            if (delta.magnitude > Mathf.Epsilon)
            {
                Vector2 uiPos = Input.mousePosition - transform.position;
                Vector3 newPos = new(uiPos.x / sizeDelta.x * terrainSize.x, 0f, uiPos.y / sizeDelta.y * terrainSize.z);
                EventManager.Instance.Raise(new MoveCameraEventHandler(newPos));
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            dragging = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            dragging = false;
        }
    }
}
