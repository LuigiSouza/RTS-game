using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using T4.Globals;
using T4.Managers;
using T4.Resource;
using T4.Units;
using T4.Units.Buildings;
using T4.Units.Characters;
using UnityEngine;

namespace T4.AI.Enemy
{
    public class AIEnemyManager : MonoBehaviour
    {
        private readonly List<GameResource> foodResources = new();
        private readonly List<GameResource> goldResources = new();

        private BuildingManager Quarter;
        private BuildingManager Castle;

        private int ID;

        private const int UNITS_PER_RESOURCE = 3;
        private List<CharacterManager> unitsWorking = new();

        void Start()
        {
            LocateResources();

            ID = GameManager.Instance.CpuId;

            Quarter = GameManager.Instance.USER_UNITS[ID].First(b => b.Data.code == UnitType.QUARTER).UnitManager as BuildingManager;
            Castle = GameManager.Instance.USER_UNITS[ID].First(b => b.Data.code == UnitType.CASTLE).UnitManager as BuildingManager;
        }

        void FixedUpdate()
        {
            int foodWorkers = 0, goldWorkers = 0;
            List<CharacterManager> newList = new(unitsWorking);
            foreach (CharacterManager unit in unitsWorking)
            {
                GameObject target = unit.GetComponent<CharacterBehaviour>().Target;
                if (target.TryGetComponent<GameResource>(out GameResource res))
                {
                    switch (res.Type)
                    {
                        case ResourceType.FOOD:
                            foodWorkers++;
                            break;
                        case ResourceType.STONE:
                            break;
                        case ResourceType.GOLD:
                            goldWorkers++;
                            break;
                        default:
                            newList.Remove(unit);
                            break;
                    }
                }
                else
                {
                    newList.Remove(unit);
                }
            }
            unitsWorking = newList;

            if (foodWorkers < UNITS_PER_RESOURCE) { }
            else if (goldWorkers < UNITS_PER_RESOURCE) { }
        }

        private void LocateResources()
        {
            GameResource[] gameResources = GameObject.FindGameObjectsWithTag(TagValues.ResourceVein)
                .Select(g => g.GetComponent<GameResource>())
                .ToArray();
            foreach (GameResource res in gameResources)
            {
                switch (res.Type)
                {
                    case ResourceType.FOOD:
                        foodResources.Add(res);
                        break;
                    case ResourceType.STONE:
                        break;
                    case ResourceType.GOLD:
                        goldResources.Add(res);
                        break;
                    default:
                        throw new NotImplementedException($"Valor de {res.Type} não implementado.");
                }
            }
        }
    }
}