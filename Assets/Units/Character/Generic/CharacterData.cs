
using System.Collections.Generic;
using T4.Resource;
using UnityEngine;

namespace T4.Units.Characters
{
    [CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Characters", order = 3)]
    public class CharacterData : UnitData
    {
        public int resourceCapacity;
        public int resourceQuantity;
        public ResourceType resourceType;
        public float collectionTime;

        public CharacterData(int owner) : base(owner)
        {
        }
    }
}
