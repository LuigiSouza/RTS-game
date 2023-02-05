
using System;
using System.Collections.Generic;
using T4.Managers;
using T4.Units.Abilities;
using UnityEngine;

namespace T4.Units
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Objects/Units", order = 1)]
    public class UnitData : ScriptableObject
    {
        public string unitName;
        public string description;
        public UnitType code;
        public UnitState state;
        public int healthpoints;
        public int strength;
        public float actionTime;
        public List<ResourceValue> cost;
        public List<AbilityData> abilities = new();
        public GameObject prefab;
        public float fieldOfView;
        public int owner;

        public UnitData(int owner)
        {
            this.owner = owner;
        }

        public bool CanBuy()
        {
            foreach (ResourceValue resource in cost)
            {
                if (GameManager.Instance.GetResource(owner, resource.code).Amount < resource.amount)
                {
                    return false;
                }
            }
            return true;
        }
        public virtual UnitData Clone()
        {
            UnitData clone = (UnitData)MemberwiseClone();
            clone.cost = new List<ResourceValue>(cost);
            clone.abilities = new List<AbilityData>(abilities);
            return clone;
        }
    }
}
