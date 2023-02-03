using System;
using T4.Globals;
using T4.Managers;
using T4.Units.Buildings;
using UnityEngine;

namespace T4.Units.Characters
{
    class VillagerActionManager : ActionManager
    {
        protected override void CheckValidHits()
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _raycastHit;

            if (Physics.Raycast(_ray, out _raycastHit, 1000f, LayerMaskValues.ResourceVeinLayer))
            {
                behaviour.SetTarget(_raycastHit.transform.gameObject, BehaviourType.COLLECT);
            }
            else if (Physics.Raycast(_ray, out _raycastHit, 1000f, LayerMaskValues.UnitLayer))
            {
                UnitManager unitManager = _raycastHit.transform.GetComponent<UnitManager>();
                if (!(unitManager is BuildingManager) || unitManager.Unit.Owner != GameManager.Instance.PlayerId) return;
                Building objectHit = unitManager.Unit as Building;
                if (objectHit.IsPlaced) behaviour.SetTarget(_raycastHit.transform.gameObject, BehaviourType.BUILD);
            }
            else
            {
                behaviour.SetTarget(null, BehaviourType.NONE);
            }
        }
    }
}