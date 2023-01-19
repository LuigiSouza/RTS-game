using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using T4.Units.Buildings;
using T4.Events;
using T4.Resource;
using T4.Units;
using UnityEngine;
using T4.Units.Characters;
using T4.Player;
using System;
using System.Linq;

namespace T4.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                    if (!_instance)
                    {
                        Debug.LogError($"There needs to be one active {typeof(GameManager)} script on a GameObject in your scene.");
                    }
                    else
                    {
                        _instance.Init();
                    }
                }

                return _instance;
            }
        }

        public int PlayerId { get; private set; }
        public int CpuId { get; private set; }
        private Dictionary<int, PlayerData> Players = new();

        private void Init()
        {
            BuildingDataBuilder.LoadAvaiableBuildings();
            CharacterDataBuilder.LoadAvaiableCharacters();
            UnitDataBuilder.LoadAvaiablePlayers();

            PlayerId = UnitDataBuilder.Player.id; CpuId = UnitDataBuilder.CPU.id;
            Players.Add(PlayerId, UnitDataBuilder.Player);
            Players.Add(CpuId, UnitDataBuilder.CPU);

            InitializeResources();
            SELECTED_UNITS.CollectionChanged += (object s, NotifyCollectionChangedEventArgs a) => EventManager.Instance.Raise(new ChangeSelectedUnitsEventHandler());

            InitializeDictionaries();
            InitializePlayers();
        }

        private void InitializeDictionaries()
        {
            USER_UNITS.Add(PlayerId, new()); USER_UNITS.Add(CpuId, new());
            USER_CHARACTERS.Add(PlayerId, new()); USER_CHARACTERS.Add(CpuId, new());
        }

        private void InitializePlayers()
        {
            Vector3 playerPos = new(15, 0, 75);
            Vector3 enemyPos = new(180, 0, 110);
            BuildingPlacer.Instance.SpawnBuilding(BuildingDataBuilder.Castle, CpuId, enemyPos);
            BuildingPlacer.Instance.SpawnBuilding(BuildingDataBuilder.Castle, PlayerId, playerPos);
            EventManager.Instance.Raise(new MoveCameraEventHandler(playerPos));

            foreach (KeyValuePair<int, HashSet<Unit>> val in USER_UNITS)
            {
                PlayerCastles.Add(val.Key, (Building)val.Value.ToArray()[0]);
            }
        }

        public PlayerData GetPlayer(int id)
        {
            return Players[id];
        }

        public Resource.Resource GetResource(int playerId, ResourceType type)
        {
            return GAME_RESOURCES[playerId][type];
        }

        private void InitializeResources()
        {
            GAME_RESOURCES.Add(PlayerId, new()
            {
                { ResourceType.FOOD, new Resource.Resource(ResourceType.FOOD, 200) },
                { ResourceType.STONE, new Resource.Resource(ResourceType.STONE, 200) },
                { ResourceType.GOLD, new Resource.Resource(ResourceType.GOLD, 50) },
            });
            GAME_RESOURCES.Add(CpuId, new()
            {
                { ResourceType.FOOD, new Resource.Resource(ResourceType.FOOD, 5000) },
                { ResourceType.STONE, new Resource.Resource(ResourceType.STONE, 5000) },
                { ResourceType.GOLD, new Resource.Resource(ResourceType.GOLD, 5000) },
            });
        }

        private Dictionary<int, Dictionary<ResourceType, Resource.Resource>> GAME_RESOURCES { get; } = new();

        public Dictionary<int, HashSet<Unit>> USER_UNITS = new();
        public Dictionary<int, HashSet<Character>> USER_CHARACTERS = new();

        public Dictionary<int, Building> PlayerCastles { get; private set; } = new();

        public ObservableCollection<UnitManager> SELECTED_UNITS { get; } = new();
        [HideInInspector]
        public BuildingData[] BUILDING_DATA;
    }
}
