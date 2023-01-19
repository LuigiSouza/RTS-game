using System;
using T4.Globals;
using UnityEngine;

namespace T4.Units.Characters
{
    class VillagerActionManager : ActionManager
    {
        protected override void CheckValidHits()
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _raycastHit;

            if (Physics.Raycast(_ray, out _raycastHit, 1000f, LayerMaskValues.ResourceVeilLayer))
            {
                behaviour.SetTarget(_raycastHit.transform.gameObject, BehaviourType.COLLECT);
            }
            else
            {
                behaviour.SetTarget(null, BehaviourType.NONE);
            }
        }
    }
}