
using System.Collections.Generic;
using UnityEngine;

namespace T4.Units.Buildings
{
    [CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Buildings", order = 2)]
    public class BuildingData : UnitData
    {
        public BuildingData(int owner) : base(owner)
        {
        }
    }
}
