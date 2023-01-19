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
        public CharacterData Data { get { return (CharacterData)character.Data; } }

        private void Start()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void FixedUpdate()
        {
            character.Unit.Data.state = UnitState.IDLE;

            if (character.IsMoving())
            {
                character.Unit.Data.state = UnitState.MOVING;
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
    }
}