using T4.Globals;
using UnityEngine;

namespace T4.Units.Characters
{
    public class SoldierBehaviour : CharacterBehaviour
    {
        private UnitManager unit;

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
                MoveCharacter(target.transform, UnitState.FOLLOWING);
                unit = target.GetComponent<UnitManager>();
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
                    MoveCharacter(target.transform, UnitState.ATTTACKING);
                }
            }
            else if (character.Data.state == UnitState.ATTTACKING)
            {
                if (!canAct)
                {
                    LastAcionTime = -1;
                    MoveCharacter(target.transform, UnitState.FOLLOWING);
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
                    unit.Unit.TakeHit(-Data.strength);
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