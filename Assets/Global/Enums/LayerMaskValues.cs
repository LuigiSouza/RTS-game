using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T4.Globals
{
    public static class LayerMaskValues
    {
        public static int TerrainLayer { get; } = 1 << LayerMask.NameToLayer("Terrain");
        public static int FlatTerrainLayer { get; } = 1 << LayerMask.NameToLayer("FlatTerrain");
        public static int FogLayer { get; } = 1 << LayerMask.NameToLayer("Fog");
        public static int ObstaclesLayer { get; } = 1 << LayerMask.NameToLayer("Obstacles");
        public static int ResourceVeinLayer { get; } = 1 << LayerMask.NameToLayer("ResourceVein");
        public static int UnitLayer { get; } = 1 << LayerMask.NameToLayer("Unit");
    }
}
