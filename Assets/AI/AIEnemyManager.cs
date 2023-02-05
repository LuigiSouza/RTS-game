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
        private List<GameResource> foodResources = new();
        private List<GameResource> goldResources = new();

        private BuildingManager Quarter;
        private BuildingManager Castle;

        private int ID;

        [SerializeField, Min(0)]
        private int UnitsPerResource = 1;
        [SerializeField, Min(0)]
        private int UnitsToAttack = 1;
        private List<CharacterManager> unitsWorking = new();

        private List<CharacterManager> idleWorkers = new();

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
                GameResource res = unit.GetComponent<VillagerBehaviour>().Resource;
                if (res != null)
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

            idleWorkers = GetIdleWorkers();
            SendUnitsToCollect(foodWorkers, ref foodResources);
            // SendUnitsToCollect(goldWorkers, ref goldResources);

            if (CountUnitType(UnitType.VILLAGER) < 1 * UnitsPerResource && !Castle.CastingAbility)
            {
                Castle.TriggerSkill(0);
            }
            else if (CountUnitType(UnitType.SOLDIER) < UnitsToAttack)
            {
                Quarter.TriggerSkill(0);
            }
            else
            {
                LauchAttack();
            }
        }

        private void LauchAttack()
        {
            foreach (CharacterManager cm in GetIdleSoldiers())
            {
                Building PlayerCastle = GameManager.Instance.PlayerCastles[GameManager.Instance.PlayerId];
                cm.GetComponent<SoldierBehaviour>().SetTarget(PlayerCastle.UnitManager.gameObject, BehaviourType.ATTACK);
            }
        }

        private int CountUnitType(UnitType type)
        {
            return GameManager.Instance.USER_CHARACTERS[ID]
                .Where(c => c.Data.code == type)
                .Count();
        }

        private void SendUnitsToCollect(int workers, ref List<GameResource> resourceList)
        {
            if (workers < UnitsPerResource)
            {
                GameResource closestResource = GetClosestResource(ref resourceList);
                while (workers < UnitsPerResource && idleWorkers.Count > 0 && closestResource != null)
                {
                    CharacterManager cm = idleWorkers[0]; idleWorkers.RemoveAt(0);
                    cm.GetComponent<VillagerBehaviour>().SetTarget(closestResource.gameObject, BehaviourType.COLLECT);
                    unitsWorking.Add(cm);
                    workers++;
                }
            }
        }

        private GameResource GetClosestResource(ref List<GameResource> resourceList)
        {
            GameResource gMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = Castle.transform.position;
            List<GameResource> listCopy = new(resourceList);
            foreach (GameResource res in listCopy)
            {
                if(res == null) 
                {
                    resourceList.Remove(res);
                    continue;
                }

                Transform t = res.gameObject.transform;
                float dist = Vector3.Distance(t.position, currentPos);
                if (dist < minDist)
                {
                    gMin = res;
                    minDist = dist;
                }
            }
            return gMin;
        }

        private List<CharacterManager> GetIdleWorkers()
        {
            return GameManager.Instance.USER_CHARACTERS[ID]
                .Where(c => c.Data.state == UnitState.IDLE && c.Data.code == UnitType.VILLAGER)
                .Select(c => c.UnitManager as CharacterManager)
                .ToList();
        }

        private List<CharacterManager> GetIdleSoldiers()
        {
            return GameManager.Instance.USER_CHARACTERS[ID]
                .Where(c => c.Data.state == UnitState.IDLE && c.Data.code == UnitType.SOLDIER)
                .Select(c => c.UnitManager as CharacterManager)
                .ToList();
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