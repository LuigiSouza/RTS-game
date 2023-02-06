using System;
using T4.Managers;
using T4.Units.Buildings;
using T4.Units.Characters;
using UnityEngine;
using UnityEngine.AI;

namespace T4.Units.Abilities
{
    public enum AbilityType
    {
        CREATE_CHARACTER,
        CONSTRUCT_BUILDING
    }

    [CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Abilities", order = 4)]
    public class AbilityData : ScriptableObject
    {
        public string code;
        public string abilityName;
        public string description;
        public AbilityType type;
        public UnitData unitReference;
        public float castTime;
        public Sprite sprite;

        public bool Trigger(GameObject source, GameObject target = null)
        {
            Vector3 instantiationPosition;
            BoxCollider coll;
            UnitManager sourceUnitManager;
            switch (type)
            {
                case AbilityType.CREATE_CHARACTER:
                    coll = source.GetComponent<BoxCollider>();
                    float distancex = UnityEngine.Random.Range(-0.8f, 0.8f);
                    float distancez = UnityEngine.Random.Range(-0.8f, 0.8f);
                    instantiationPosition = new Vector3(
                        source.transform.position.x - coll.size.x * distancex,
                        source.transform.position.y,
                        source.transform.position.z - coll.size.z * distancez
                    );
                    CharacterData cd = (CharacterData)unitReference;
                    if (!cd.CanBuy()) return false;

                    sourceUnitManager = source.GetComponent<UnitManager>();
                    if (sourceUnitManager == null)
                    {
                        Debug.LogError($"Não foi encontrado o componente {typeof(UnitManager)}");
                        return false;
                    }


                    Character c = CharacterBuilder.Build(cd.code, sourceUnitManager.Unit.Owner);
                    c.Transform.GetComponent<NavMeshAgent>().Warp(instantiationPosition);
                    c.Place();
                    break;
                case AbilityType.CONSTRUCT_BUILDING:
                    BuildingPlacer.Instance.SelectPlacedBuilding(unitReference.code);
                    break;
                default:
                    throw new NotImplementedException("Ação inválida.");
            }

            return true;
        }
        public virtual AbilityData Clone()
        {
            AbilityData clone = (AbilityData)MemberwiseClone();
            clone.unitReference = unitReference.Clone();
            return clone;
        }
    }
}