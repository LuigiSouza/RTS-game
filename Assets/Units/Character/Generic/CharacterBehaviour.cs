using System.Collections.Generic;
using UnityEngine;

namespace T4.Units.Characters
{
    public enum BehaviourType
    {
        NONE,
        ATTACK,
        BUILD,
        COLLECT
    }

    [RequireComponent(typeof(CharacterManager))]
    public abstract class CharacterBehaviour : MonoBehaviour
    {
        protected bool isDirty = false;
        protected bool canAct = false;

        protected GameObject prevTarget;
        protected GameObject target;
        protected BehaviourType behaviourTarget = BehaviourType.NONE;
        protected CharacterManager character;

        protected HashSet<GameObject> colliding = new();

        public float LastAcionTime { get; protected set; }
        public UnitState CurrentState { get { return character.Data.state; } }
        public CharacterData Data { get { return (CharacterData)character.Data; } }
        public GameObject Target { get => target; }

        private void Start()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void FixedUpdate()
        {
            character.Data.state = UnitState.IDLE;

            if (character.IsMoving())
            {
                character.Data.state = UnitState.MOVING;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            colliding.Add(other.gameObject);
            if (other.gameObject == target) canAct = true;
        }
        private void OnTriggerExit(Collider other)
        {
            colliding.Remove(other.gameObject);
            if (other.gameObject == target) canAct = false;
        }

        public virtual void SetTarget(GameObject target, BehaviourType behaviour)
        {
            this.prevTarget = this.target;
            this.target = target;
            this.behaviourTarget = behaviour;

            if (colliding.Contains(target)) { canAct = true; }
            else { isDirty = true; canAct = false; }
        }

        #region MOVE CHARACTER
        protected void MoveCharacter(Vector3 destiny, UnitState state)
        {
            character.Data.state = state;
            character.MoveTo(destiny);
        }

        protected void UpdateTarget(GameObject target, BehaviourType behaviour, UnitState state)
        {
            if(target == null) target = character.gameObject;
            SetTarget(target, behaviour);
            MoveCharacter(target.transform.position, state);
        }
        #endregion
    }
}