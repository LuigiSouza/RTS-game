using T4.Globals;
using UnityEngine;
using UnityEngine.AI;

namespace T4.Units.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterManager : UnitManager
    {
        [SerializeField]
        private NavMeshAgent agent;

        private Character character = null;
        public override Unit Unit
        {
            get { return character; }
            protected set { character = value is Character c ? c : null; }
        }

        private void Awake()
        {
            if (agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
            }
        }

        private void Start()
        {
            FOV.SetFovSize(Data.fieldOfView);
        }

        public void MoveTo(Vector3 targetPosition)
        {
            agent.destination = targetPosition;
        }

        public bool IsMoving()
        {
            return agent.remainingDistance > agent.stoppingDistance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagValues.ResourceRange))
            {
                Debug.Log("Ola");
            }
        }
    }
}