using System;
using T4.Globals;
using T4.Managers;
using T4.Units.Buildings;
using UnityEngine;

namespace T4.Units.Characters
{
    class SoldierActionManager : ActionManager
    {
        protected override void CheckValidHits()
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _raycastHit;

            if (Physics.Raycast(_ray, out _raycastHit, 1000f, LayerMaskValues.UnitLayer))
            {
                UnitManager unitManager = _raycastHit.transform.GetComponent<UnitManager>();
                if (unitManager.Unit.Owner == GameManager.Instance.PlayerId) return;
                behaviour.SetTarget(_raycastHit.transform.gameObject, BehaviourType.ATTACK);
            }
            else
            {
                behaviour.SetTarget(null, BehaviourType.NONE);
            }
        }
    }
}