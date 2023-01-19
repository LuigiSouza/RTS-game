using T4.Events;
using T4.Globals;
using T4.Managers;
using T4.Resource;
using UnityEngine;

namespace T4.Units.Characters
{
    public class VillagerBehaviour : CharacterBehaviour
    {
        private GameResource resource;

        protected override void FixedUpdate()
        {
            if (behaviourTarget == BehaviourType.NONE)
            {
                base.FixedUpdate();
                return;
            }

            if (behaviourTarget == BehaviourType.COLLECT)
            {
                if (isDirty)
                {
                    Debug.Log("Indo Coletar");
                    SetResourceTarget();
                }
                else
                {
                    CollectBehaviour();
                }
            }
        }

        #region COLLECT RESOURCES
        private void ReturnResources()
        {
            UpdateTarget(prevTarget, BehaviourType.COLLECT, UnitState.FOLLOWING);
            GameManager.Instance.GetResource(character.Unit.Owner, Data.resourceType).AddAmount(Data.resourceQuantity);
            EventManager.Instance.Raise(new ChangeResourceEventHandler());
            Data.resourceQuantity = 0;
            canAct = false;
        }

        private void ProduceResources()
        {
            float passed = Time.realtimeSinceStartup - LastAcionTime;
            if (passed >= Data.collectionTime)
            {
                Debug.Log("Coletou");
                Data.resourceQuantity += 1;
                LastAcionTime = Time.realtimeSinceStartup;
            }
            if (Data.resourceQuantity >= Data.resourceCapacity)
            {
                Debug.Log("Voltando");
                UpdateTarget(GameManager.Instance.PlayerCastles[Data.owner].Transform.gameObject, BehaviourType.COLLECT, UnitState.RETURNING);
                LastAcionTime = Time.realtimeSinceStartup;
                canAct = false;
            }
        }

        private void SetResourceTarget()
        {
            if (1 << target.layer == LayerMaskValues.ResourceVeilLayer)
            {
                MoveCharacter(target.transform.position, UnitState.FOLLOWING);
                resource = target.GetComponent<GameResource>();
                if (Data.resourceType != resource.Type)
                {
                    Data.resourceQuantity = 0;
                    Data.resourceType = resource.Type;
                }

            }
            LastAcionTime = -1;
            isDirty = false;
        }
        #endregion

        #region MOVE CHARACTER
        private void MoveCharacter(Vector3 destiny, UnitState state)
        {
            character.Unit.Data.state = state;
            character.MoveTo(destiny);
        }

        private void UpdateTarget(GameObject target, BehaviourType behaviour, UnitState state)
        {
            SetTarget(target, behaviour);
            MoveCharacter(target.transform.position, state);
        }
        #endregion

        private void CollectBehaviour()
        {
            if (character.Unit.Data.state == UnitState.FOLLOWING)
            {
                if (canAct)
                {
                    MoveCharacter(target.transform.position, UnitState.WORKING);
                }
            }
            else if (character.Unit.Data.state == UnitState.WORKING)
            {
                if (!canAct)
                {
                    LastAcionTime = -1;
                }
                else if (LastAcionTime < 0)
                {
                    LastAcionTime = Time.realtimeSinceStartup;
                }
                else
                {
                    ProduceResources();
                }
            }
            else if (character.Unit.Data.state == UnitState.RETURNING)
            {
                if (canAct)
                {
                    ReturnResources();
                }
            }
        }
    }
}