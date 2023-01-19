using UnityEngine;
using T4.Globals;
using T4.Units;
using T4.Units.Characters;
using UnityEngine.AI;
using T4.Events;

namespace T4.Managers
{
    public class MovementManager : MonoBehaviour
    {
        [SerializeField]
        private NavMeshSurface NAV_MESH_SURFACE;

        private void Awake()
        {
            if (NAV_MESH_SURFACE == null)
            {
                NAV_MESH_SURFACE = GameObject.Find("Terrain").GetComponent<NavMeshSurface>();
            }
        }

        private void Start()
        {
            EventManager.Instance.AddListener<UpdateNavMeshHandler>(UpdateNavMeshSurface);
        }

        private void UpdateNavMeshSurface(UpdateNavMeshHandler e)
        {
            NAV_MESH_SURFACE.UpdateNavMesh(NAV_MESH_SURFACE.navMeshData);
        }

        private void Update()
        {
            if (GameManager.Instance.SELECTED_UNITS.Count == 0 || !Input.GetMouseButtonUp(1)) return;

            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                _ray,
                out RaycastHit _raycastHit,
                1000f,
                LayerMaskValues.TerrainLayer
            ))
            {
                foreach (UnitManager um in GameManager.Instance.SELECTED_UNITS)
                {
                    if (um.GetType() == typeof(CharacterManager))
                    {
                        ((CharacterManager)um).MoveTo(_raycastHit.point);
                    }
                }
            }
        }
    }
}