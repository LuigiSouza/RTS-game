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

        public void Trigger(GameObject source, GameObject target = null)
        {
            Vector3 instantiationPosition;
            BoxCollider coll;
            UnitManager sourceUnitManager;
            switch (type)
            {
                case AbilityType.CREATE_CHARACTER:
                    coll = source.GetComponent<BoxCollider>();
                    instantiationPosition = new Vector3(
                        source.transform.position.x - coll.size.x * 0.7f,
                        source.transform.position.y,
                        source.transform.position.z - coll.size.z * 0.7f
                    );
                    CharacterData cd = (CharacterData)unitReference;
                    if (!cd.CanBuy()) return;

                    sourceUnitManager = source.GetComponent<UnitManager>();
                    if (sourceUnitManager == null)
                    {
                        Debug.LogError($"Não foi encontrado o componente {typeof(UnitManager)}");
                        return;
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
        }
    }
}