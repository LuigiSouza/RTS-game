using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using T4.Events;
using T4.Player;
using T4.Resource;
using T4.Units;
using T4.Units.Buildings;
using T4.Units.Characters;

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
            BuildingPlacer.Instance.SpawnBuilding(BuildingDataBuilder.Quarter, CpuId, enemyPos + new Vector3(10,0,10));
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
                { ResourceType.FOOD, new Resource.Resource(ResourceType.FOOD, 2000) },
                { ResourceType.STONE, new Resource.Resource(ResourceType.STONE, 2000) },
                { ResourceType.GOLD, new Resource.Resource(ResourceType.GOLD, 2000) },
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

        private int CheckVictory()
        {
            if(PlayerCastles[PlayerId] == null && PlayerCastles[CpuId] == null)
                return 0;
            if(PlayerCastles[PlayerId] == null)
                return CpuId;
            if(PlayerCastles[CpuId] == null)
                return PlayerId;
            return -1;
        }

        private void Update()
        {
            int victory = CheckVictory();
            if(victory == -1)
                return;
            else if(victory == 0)
                Debug.Log("Empatou");
            else if(victory == CpuId)
                Debug.Log("CPU GANHOU");
            else if(victory == PlayerId)
                Debug.Log("Player GANHOU");
            else
                throw new NotImplementedException("Condição de vitória desconhecida");
        }
    }
}
