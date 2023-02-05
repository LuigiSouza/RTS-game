using T4.Events;
using T4.Globals;
using T4.Managers;
using T4.Resource;
using T4.Units.Buildings;
using UnityEngine;

namespace T4.Units.Characters
{
    public class VillagerBehaviour : CharacterBehaviour
    {
        public GameResource Resource { get; private set; }
        public Building Building { get; private set; }

        protected override void FixedUpdate()
        {
            if (behaviourTarget == BehaviourType.NONE)
            {
                Resource = null; Building = null;
                base.FixedUpdate();
                return;
            }

            if (behaviourTarget == BehaviourType.COLLECT)
            {
                if (isDirty)
                {
                    SetResourceTarget();
                }
                else
                {
                    CollectBehaviour();
                }
            }
            else if (behaviourTarget == BehaviourType.BUILD)
            {
                if (isDirty)
                {
                    SetBuildTarget();
                }
                else
                {
                    BuildBehaviour();
                }
            }
            else if (target == null)
            {
                behaviourTarget = BehaviourType.NONE;
                MoveCharacter(transform.position, UnitState.IDLE);
            }
        }

        #region COLLECT RESOURCES
        private void ReturnResources()
        {
            if (prevTarget != null)
            {
                UpdateTarget(prevTarget, BehaviourType.COLLECT, UnitState.FOLLOWING);
            }
            else
            {
                UpdateTarget(null, BehaviourType.NONE, UnitState.IDLE);
            }
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
                if (Resource && Resource.Extract())
                {
                    Data.resourceQuantity += 1;
                    LastAcionTime = Time.realtimeSinceStartup;
                }
                else
                {
                    UpdateTarget(GameManager.Instance.PlayerCastles[Data.owner].Transform.gameObject, BehaviourType.COLLECT, UnitState.RETURNING);
                }
            }
            if (Data.resourceQuantity >= Data.resourceCapacity || Resource == null)
            {
                UpdateTarget(GameManager.Instance.PlayerCastles[Data.owner].Transform.gameObject, BehaviourType.COLLECT, UnitState.RETURNING);
                LastAcionTime = Time.realtimeSinceStartup;
                canAct = false;
            }
        }

        private void SetResourceTarget()
        {
            if (1 << target.layer == LayerMaskValues.ResourceVeinLayer)
            {
                MoveCharacter(target.transform.position, UnitState.FOLLOWING);
                Resource = target.GetComponent<GameResource>();
                if (Data.resourceType != Resource.Type)
                {
                    Data.resourceQuantity = 0;
                    Data.resourceType = Resource.Type;
                }

            }
            LastAcionTime = -1;
            isDirty = false;
        }

        private void CollectBehaviour()
        {
            if (character.Data.state == UnitState.FOLLOWING)
            {
                if (canAct && target != null)
                {
                    MoveCharacter(target.transform.position, UnitState.WORKING);
                }
            }
            else if (character.Data.state == UnitState.WORKING)
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
            else if (character.Data.state == UnitState.RETURNING)
            {
                if (canAct)
                {
                    Debug.Log("Retornou os recursos");
                    ReturnResources();
                }
            }
        }
        #endregion

        #region CONTRUCT BUILDINGS
        private void SetBuildTarget()
        {
            if (target.CompareTag(TagValues.Unit))
            {
                if (target.TryGetComponent<BuildingManager>(out var _))
                {
                    MoveCharacter(target.transform.position, UnitState.FOLLOWING);
                }
                Building = target.GetComponent<BuildingManager>().Unit as Building;
            }
            LastAcionTime = -1;
            isDirty = false;
        }

        private void BuildBehaviour()
        {
            if (character.Data.state == UnitState.FOLLOWING)
            {
                if (canAct)
                {
                    MoveCharacter(target.transform.position, UnitState.WORKING);
                }
            }
            else if (character.Data.state == UnitState.WORKING)
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
                    ConstructBuild();
                }
            }
        }

        private void ConstructBuild()
        {
            float passed = Time.realtimeSinceStartup - LastAcionTime;
            if (passed >= Data.collectionTime)
            {
                if (Building != null && !Building.Construct(Data.strength))
                {
                    LastAcionTime = Time.realtimeSinceStartup;
                }
                else
                {
                    UpdateTarget(null, BehaviourType.NONE, UnitState.IDLE);
                }
            }
        }
        #endregion
    }
}