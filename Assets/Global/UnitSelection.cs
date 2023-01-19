using System.Collections.Generic;
using T4.Globals;
using T4.Managers;
using T4.Units;
using T4.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace T4.Global
{
    public class UnitSelection : MonoBehaviour
    {
        private bool isDragging = false;
        private Vector3 dragStartPosition;

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging && dragStartPosition != Input.mousePosition)
                {
                    SelectUnitsInDraggingBox();
                }
                isDragging = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeselectAllUnits();
            }

            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                dragStartPosition = Input.mousePosition;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, 1000f) && !raycastHit.transform.CompareTag(TagValues.Unit))
                {
                    DeselectAllUnits();
                }
            }
        }

        void OnGUI()
        {
            if (isDragging)
            {
                // Create a rect from both mouse positions
                var rect = RectUtils.GetScreenRect(dragStartPosition, Input.mousePosition);
                RectUtils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
                RectUtils.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
            }
        }

        private void DeselectAllUnits()
        {
            List<UnitManager> selectedUnits = new(GameManager.Instance.SELECTED_UNITS);
            foreach (UnitManager um in selectedUnits)
            {
                um.Deselect();
            }
        }

        private void SelectUnitsInDraggingBox()
        {
            Rect selectionRect = RectUtils.GetScreenRect(dragStartPosition, Input.mousePosition);
            GameObject[] selectableUnits = GameObject.FindGameObjectsWithTag(TagValues.Unit);
            bool inBounds;
            foreach (GameObject unit in selectableUnits)
            {
                Bounds unitWorldBounds = unit.GetComponent<Collider>().bounds;
                Rect unitViewRect = RectUtils.WorldBoundsToScreenRect(Camera.main, unitWorldBounds);
                inBounds = RectUtils.IsValidSelection(unitViewRect, selectionRect);

                if (inBounds)
                {
                    unit.GetComponent<UnitManager>().Select();
                }
                else
                {
                    unit.GetComponent<UnitManager>().Deselect();
                }
            }
        }
    }
}
