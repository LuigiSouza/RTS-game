using T4.Events;
using T4.Globals;
using T4.Managers;
using UnityEngine;

namespace T4.Units.Buildings
{
    public class BuildingManager : UnitManager
    {
        private Building building = null;
        public override Unit Unit
        {
            get { return building; }
            protected set { building = value is Building b ? b : null; }
        }

        private int nCollisions = 0;
        private bool prevValid = true;

        private bool canBuy;

        public void Initialize(Building b)
        {
            base.Initialize(b);
            building = b;
            canBuy = building.CanBuy();
            if (!canBuy)
            {
                building.SetMaterials(BuildingStates.INVALID);
            }
            EventManager.Instance.AddListener<ChangeResourceEventHandler>(OnChangeResources);
            EventManager.Instance.AddListener<PlaceBuildingEventHandler>(OnPlaceBuilding);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagValues.Terrain)) return;
            nCollisions++;
            CheckPlacement();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagValues.Terrain)) return;
            nCollisions--;
            CheckPlacement();
        }

        public bool CheckPlacement()
        {
            if (building == null || building.IsPlaced) return false;

            if (!canBuy)
            {
                return false;
            }

            bool validPlacement = HasValidPlacement();
            if (validPlacement == prevValid)
            {
                return validPlacement;
            }
            prevValid = validPlacement;
            if (validPlacement)
            {
                building.SetMaterials(BuildingStates.VALID);
            }
            else
            {
                building.SetMaterials(BuildingStates.INVALID);
            }
            return validPlacement;
        }

        public void OnPlaceBuilding(PlaceBuildingEventHandler e)
        {
            if (e.Building != building) return;

            FOV.SetFovSize(Data.fieldOfView);

            EventManager.Instance.RemoveListener<PlaceBuildingEventHandler>(OnPlaceBuilding);
            EventManager.Instance.RemoveListener<ChangeResourceEventHandler>(OnChangeResources);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<PlaceBuildingEventHandler>(OnPlaceBuilding);
            EventManager.Instance.RemoveListener<ChangeResourceEventHandler>(OnChangeResources);
        }

        public void OnChangeResources(ChangeResourceEventHandler e)
        {
            if (building.IsPlaced) return;

            canBuy = building.CanBuy();
            if (canBuy)
            {
                if (HasValidPlacement())
                {
                    building.SetMaterials(BuildingStates.VALID);
                }
                else
                {
                    building.SetMaterials(BuildingStates.INVALID);
                }
            }
            else
            {
                building.SetMaterials(BuildingStates.INVALID);
            }
        }

        public bool HasValidPlacement()
        {
            if (nCollisions > 0) return false;

            Vector3 pos = transform.position;
            Vector3 center = _collider.center;
            Vector3 halfSize = _collider.size * 0.5f;
            float bottomY = center.y - halfSize.y + 0.5f;
            Vector3[] bottomCorners = new Vector3[]
            {
                new Vector3(center.x - halfSize.x, bottomY, center.z - halfSize.z),
                new Vector3(center.x - halfSize.x, bottomY, center.z + halfSize.z),
                new Vector3(center.x + halfSize.x, bottomY, center.z - halfSize.z),
                new Vector3(center.x + halfSize.x, bottomY, center.z + halfSize.z)
            };
            int invalidCornersCount = 0;
            foreach (Vector3 corner in bottomCorners)
            {
                if (!Physics.Raycast(pos + corner, Vector3.up * -1f, 0.6f, LayerMaskValues.TerrainLayer))
                {
                    invalidCornersCount++;
                }
                else if (Physics.Raycast(pos + corner, Vector3.up * -1f, 0.4f, LayerMaskValues.TerrainLayer))
                {
                    invalidCornersCount++;
                }
            }

            return invalidCornersCount < 3;
        }

        protected override bool IsActive()
        {
            return building.IsPlaced;
        }
    }
}