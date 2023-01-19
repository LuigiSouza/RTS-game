using UnityEngine;

namespace T4.Resource
{
    public class GameResource : MonoBehaviour
    {
        [SerializeField]
        private int remainingResources;

        [SerializeField]
        private ResourceType type;

        [SerializeField]
        private int range;
        [SerializeField]
        private BoxCollider rangeObject;

        public ResourceType Type { get { return type; } }

        private void Awake()
        {
            rangeObject.size = new Vector3(range, 0, range);
        }

        public bool Extract()
        {
            if (!CanExtract())
            {
                Destroy(this);
                return false;
            }
            remainingResources--;

            return true;
        }

        public bool CanExtract()
        {
            return remainingResources > 0;
        }
    }
}
