using System;

namespace T4.Units.Buildings
{
    public static class BuildingBuilder
    {
        public static Building Build(UnitType type, int owner)
        {
            switch (type)
            {
                case BuildingType.HOUSE:
                    return new HouseBuilding(BuildingDataBuilder.House, owner);
                case BuildingType.CASTLE:
                    return new CastleBuilding(BuildingDataBuilder.Castle, owner);
                case BuildingType.QUARTER:
                    return new QuarterBuilding(BuildingDataBuilder.Quarter, owner);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
