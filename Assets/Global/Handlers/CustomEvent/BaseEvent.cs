using T4.Units.Buildings;
using T4.Units;
using UnityEngine;

namespace T4.Events
{
    #region Base Class to handle events with or without data
    public abstract class BaseEvent
    {
    }
    #endregion

    #region Concrete Classes to handle events with or without data
    public class DestroyUnitEventHandler : BaseEvent { }
    public class ChangeResourceEventHandler : BaseEvent { }
    public class ChangeSelectedUnitsEventHandler : BaseEvent { }
    public class MoveCameraEventHandler : BaseEvent
    {
        public Vector3 position { get; private set; }

        public MoveCameraEventHandler(Vector3 data)
        {
            position = data;
        }
    }

    public class PlaceBuildingEventHandler : BaseEvent
    {
        public Building Building { get; private set; }
        public PlaceBuildingEventHandler(Building data)
        {
            Building = data;
        }
    }
    public class ShowBuildingCostEventHandler : BaseEvent
    {
        public UnitData UnitData { get; private set; }
        public ShowBuildingCostEventHandler(UnitData data)
        {
            UnitData = data;
        }
    }
    public class HideBuildingCostEventHandler : BaseEvent { }

    public class UpdateNavMeshHandler : BaseEvent { }
    #endregion
}
