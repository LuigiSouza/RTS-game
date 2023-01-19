using UnityEngine;
using T4.Globals;
using T4.Units.Buildings;
using UnityEngine.EventSystems;
using T4.Events;
using T4.Units;
using System.Collections.Generic;

namespace T4.Managers
{
    public class BuildingPlacer : MonoBehaviour
    {
        private static BuildingPlacer _instance;

        public static BuildingPlacer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(BuildingPlacer)) as BuildingPlacer;

                    if (!_instance)
                    {
                        Debug.LogError($"There needs to be one active {typeof(BuildingPlacer)} script on a GameObject in your scene.");
                    }
                }

                return _instance;
            }
        }

        private Building GhostBuilding = null;
        private RaycastHit _raycastHit;

        void Update()
        {
            if (GhostBuilding == null) return;

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                CancelPlacedBuilding();
                return;
            }

            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _raycastHit, 1000f, LayerMaskValues.TerrainLayer))
            {
                GhostBuilding.SetPosition(_raycastHit.point);
                GhostBuilding.CheckValidPlacement();
            }
            if (GhostBuilding.IsValid && Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceBuilding();
            }
        }

        public void SpawnBuilding(BuildingData data, int owner, Vector3 position)
        {
            Building prevPlacedBuilding = GhostBuilding;

            Building spawned = GhostBuilding = BuildingBuilder.Build(data.code, owner);
            GhostBuilding.SetPosition(position);
            PlaceBuilding();
            spawned.CompleteBuilding();

            // restore the previous state
            GhostBuilding = prevPlacedBuilding;
        }

        public void SelectPlacedBuilding(UnitType type)
        {
            PreparePlacedBuilding(type);
        }

        private void PlaceBuilding()
        {
            if (GhostBuilding.CanBuy())
            {
                GhostBuilding.Place();
                EventManager.Instance.Raise(new ChangeResourceEventHandler());
                EventManager.Instance.Raise(new PlaceBuildingEventHandler(GhostBuilding));

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    PreparePlacedBuilding(GhostBuilding.Code);
                }
                else
                {
                    GhostBuilding = null;
                }

                EventManager.Instance.Raise(new UpdateNavMeshHandler());
            }
        }

        private void PreparePlacedBuilding(UnitType type)
        {
            if (GhostBuilding != null && !GhostBuilding.IsPlaced)
            {
                Destroy(GhostBuilding.Transform.gameObject);
            }

            Building building = BuildingBuilder.Build(type, GameManager.Instance.PlayerId);

            GhostBuilding = building;
            building.SetPosition(_raycastHit.point);
        }

        private void CancelPlacedBuilding()
        {
            Destroy(GhostBuilding.Transform.gameObject);
            GhostBuilding = null;
        }

        public bool IsPlacing()
        {
            return GhostBuilding != null;
        }
    }
}