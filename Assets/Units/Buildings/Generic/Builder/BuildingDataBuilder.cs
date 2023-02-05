using T4.Managers;
using UnityEngine;

namespace T4.Units.Buildings
{
    public static class BuildingDataBuilder
    {
        private static BuildingData _house;
        private static BuildingData _quarter;
        private static BuildingData _castle;
        public static BuildingData House { get { return (BuildingData)_house.Clone(); } private set { _house = value; } }
        public static BuildingData Quarter { get { return (BuildingData)_quarter.Clone(); } private set { _quarter = value; } }
        public static BuildingData Castle { get { return (BuildingData)_castle.Clone(); } private set { _castle = value; } }

        public static void LoadAvaiableBuildings()
        {
            House = Resources.Load<BuildingData>("ScriptableObjects/Buildings/House");
            Quarter = Resources.Load<BuildingData>("ScriptableObjects/Buildings/Quarter");
            Castle = Resources.Load<BuildingData>("ScriptableObjects/Buildings/Castle");
        }
    }
}
