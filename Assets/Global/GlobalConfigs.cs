using System;
using T4.Resource;
using UnityEngine;

namespace T4.Globals
{
    public class GlobalConfigs : MonoBehaviour
    {
        private static GlobalConfigs _instance;
        public static GlobalConfigs Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(GlobalConfigs)) as GlobalConfigs;

                    if (!_instance)
                    {
                        Debug.LogError($"There needs to be one active {typeof(GlobalConfigs)} script on a GameObject in your scene.");
                    }
                }

                return _instance;
            }
        }
        [Header("Terrain")]
        [SerializeField]
        private Terrain terrain;


        [Header("Materials")]
        [SerializeField]
        private Material invalidPlacementMaterial;
        [SerializeField]
        private Material validPlacementMaterial;

        [Header("Images")]
        [SerializeField]
        private Sprite goldResourceImage;
        [SerializeField]
        private Sprite stoneResourceImage;
        [SerializeField]
        private Sprite foodResourceImage;

        [Header("Game Mechanics")]
        [SerializeField]
        private GameObject fogOfWar;
        [SerializeField]
        private bool enableFog;

        public Vector3 TerrainSize { get { return terrain.terrainData.size; } }
        public Material InvalidPlacementMaterial { get { return invalidPlacementMaterial; } }
        public Material ValidPlacementMaterial { get { return validPlacementMaterial; } }

        private void Awake()
        {
            if (fogOfWar != null)
            {
                fogOfWar.SetActive(enableFog);
            }
        }

        public Sprite GetResourceImage(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.FOOD:
                    return foodResourceImage;
                case ResourceType.STONE:
                    return stoneResourceImage;
                case ResourceType.GOLD:
                    return goldResourceImage;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
