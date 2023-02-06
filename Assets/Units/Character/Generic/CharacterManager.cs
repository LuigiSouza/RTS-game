using T4.Globals;
using T4.Managers;
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
            if (!agent)
            {
                agent = GetComponent<NavMeshAgent>();
                if (!agent)
                {
                    agent = gameObject.AddComponent<NavMeshAgent>();
                }
            }
        }

        private void Start()
        {
            FOV.SetFovSize(Data.fieldOfView);
        }

        public void MoveTo(Vector3 targetPosition, float radius = 0)
        {
            float radianPos = Random.Range(0, 2 * Mathf.PI) * radius;
            Vector3 targetCircle = targetPosition;
            targetCircle.x = targetPosition.x + Mathf.Cos(radianPos);
            targetCircle.y = targetPosition.y;
            targetCircle.z = targetPosition.z + Mathf.Sin(radianPos);
            agent.destination = targetCircle;
        }

        public bool IsMoving()
        {
            return agent.remainingDistance > agent.stoppingDistance;
        }
    }
}