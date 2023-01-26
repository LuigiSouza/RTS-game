using System.Collections.Generic;
using System.Linq;
using T4.Globals;
using T4.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace T4.Units.Buildings
{
    public enum BuildingStates
    {
        VALID,
        INVALID,
        PENDING,
        PLACED
    };

    public abstract class Building : Unit
    {
        private BuildingStates state;
        private readonly List<Material> originalMaterials = new();

        private readonly Renderer Renderer;

        private BuildingManager buildingManager = null;
        public override UnitManager UnitManager
        {
            get { return buildingManager; }
            protected set { buildingManager = value is BuildingManager b ? b : null; }
        }

        public bool IsReady { get => state == BuildingStates.PLACED; }
        public bool IsPlaced { get => state == BuildingStates.PLACED || state == BuildingStates.PENDING; }
        public bool IsValid { get => state == BuildingStates.VALID; }

        private readonly BuildingData data;
        public new UnitData Data { get { return data; } }


        protected Building(BuildingData data, int owner) : base(data, owner)
        {
            this.data = data;
            GameObject g = _transform.gameObject;
            Renderer = g.GetComponentInChildren<Renderer>();
            UnitManager = g.GetComponent<BuildingManager>();
            buildingManager.Initialize(this);

            state = BuildingStates.VALID;

            foreach (Material mat in Renderer.materials)
            {
                originalMaterials.Add(mat);
            }

            SetMaterials();
        }
        public void SetMaterials() { SetMaterials(state); }
        public void SetMaterials(BuildingStates state)
        {
            List<Material> materials = originalMaterials.ToList();
            if (state == BuildingStates.INVALID)
            {
                materials.Add(GlobalConfigs.Instance.InvalidPlacementMaterial);
            }
            else if (state == BuildingStates.VALID)
            {
                materials.Add(GlobalConfigs.Instance.ValidPlacementMaterial);
            }
            Renderer.materials = materials.ToArray();
        }

        public bool Construct(int amount)
        {
            if (state != BuildingStates.PENDING) return true;

            int sum = HP + amount >= MaxHP ? MaxHP : HP + amount;
            buildingManager.TakeHit(sum - HP);

            if (sum >= MaxHP)
            {
                buildingManager.FOV.EnableFov();
                return true;
            }
            return false;
        }

        public override void Place()
        {
            base.Place();

            _transform.GetComponent<BoxCollider>().isTrigger = false;
            _transform.GetComponent<UnitManager>().FOV.DisableFov();
            _transform.GetComponent<NavMeshObstacle>().enabled = true;

            state = BuildingStates.PENDING;
            HP = 1;

            SetMaterials();
        }

        public void CompleteBuilding()
        {
            HP = MaxHP;
            state = BuildingStates.PLACED;

            if (Owner == GameManager.Instance.PlayerId)
            {
                _transform.GetComponent<UnitManager>().FOV.EnableFov();
            }
        }

        public void CheckValidPlacement()
        {
            if (HP == MaxHP && state == BuildingStates.PENDING) state = BuildingStates.PLACED;

            if (IsPlaced) return;

            state = buildingManager.CheckPlacement()
                ? BuildingStates.VALID
                : BuildingStates.INVALID;
        }

        public override void TakeHit(int value)
        {
            base.TakeHit(value);
            CheckValidPlacement();
        }
    }
}