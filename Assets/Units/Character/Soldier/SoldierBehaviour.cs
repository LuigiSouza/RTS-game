using T4.Events;
using T4.Globals;
using T4.Managers;
using T4.Resource;
using T4.Units.Buildings;
using UnityEngine;

namespace T4.Units.Characters
{
    public class SoldierBehaviour : CharacterBehaviour
    {
        private Unit unit;

        protected override void FixedUpdate()
        {
            if (behaviourTarget == BehaviourType.NONE)
            {
                unit = null;
                base.FixedUpdate();
                return;
            }

            if (behaviourTarget == BehaviourType.ATTACK)
            {
                if (isDirty)
                {
                    SetEnemyTarget();
                }
                else
                {
                    AttackBehaviour();
                }
            }
        }

        #region ATTACK

        private void SetEnemyTarget()
        {
            if (1 << target.layer == LayerMaskValues.UnitLayer)
            {
                MoveCharacter(target.transform.position, UnitState.FOLLOWING);
                unit = target.GetComponent<UnitManager>().Unit;
            }
            LastAcionTime = -1;
            isDirty = false;
        }

        private void AttackBehaviour()
        {
            if (character.Data.state == UnitState.FOLLOWING)
            {
                if (canAct && target != null)
                {
                    MoveCharacter(target.transform.position, UnitState.ATTTACKING);
                }
            }
            else if (character.Data.state == UnitState.ATTTACKING)
            {
                if (!canAct)
                {
                    LastAcionTime = -1;
                    MoveCharacter(target.transform.position, UnitState.FOLLOWING);
                }
                else if (LastAcionTime < 0)
                {
                    LastAcionTime = Time.realtimeSinceStartup;
                }
                else
                {
                    AttackUnit();
                }
            }
        }
        private void AttackUnit()
        {
            float passed = Time.realtimeSinceStartup - LastAcionTime;
            if (passed >= Data.actionTime)
            {
                if (unit != null)
                {
                    unit.TakeHit(-Data.strength);
                    LastAcionTime = Time.realtimeSinceStartup;
                }
                else
                {
                    UpdateTarget(character.gameObject, BehaviourType.NONE, UnitState.IDLE);
                }
            }
        }
        #endregion
    }
}