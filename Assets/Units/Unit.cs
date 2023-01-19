
using System.Collections.Generic;
using T4.Managers;
using T4.Units.Abilities;
using T4.Fog;
using UnityEngine;
using T4.Events;

namespace T4.Units
{
    public class Unit
    {
        protected Transform _transform;
        public virtual UnitManager UnitManager { get; protected set; }

        public Transform Transform { get => _transform; }
        public string Uid { get; private set; }
        public int HP { get; protected set; }
        public int MaxHP { get => Data.healthpoints; }
        public virtual UnitData Data { get; private set; }
        public int Owner { get { return Data.owner; } }
        public UnitType Code { get => Data.code; }


        protected List<AbilityManager> abilitiesManagers;

        public Unit(UnitData data, int owner)
        {
            Data = data;
            data.owner = owner;
            HP = data.healthpoints;
            Uid = System.Guid.NewGuid().ToString();

            GameObject g = Object.Instantiate(Data.prefab);
            _transform = g.transform;
            UnitManager = _transform.GetComponent<UnitManager>();
            UnitManager.Initialize(this);

            AbilityManager sm; abilitiesManagers = new List<AbilityManager>();
            foreach (AbilityData ability in data.abilities)
            {
                sm = g.AddComponent<AbilityManager>();
                sm.Initialize(ability, g);
                abilitiesManagers.Add(sm);
            }
        }
        public void TriggerSkill(int index, GameObject target = null)
        {
            abilitiesManagers[index].Trigger(target);
        }
        public List<AbilityManager> AbilityManagers { get => abilitiesManagers; }


        public void SetPosition(Vector3 position)
        {
            _transform.position = position;
        }

        public virtual void Place()
        {
            foreach (ResourceValue resource in Data.cost)
            {
                GameManager.Instance.GetResource(Owner, resource.code).AddAmount(-resource.amount);
            }
            if (Owner == GameManager.Instance.PlayerId)
            {
                _transform.GetComponent<UnitManager>().FOV.EnableFov();
                EventManager.Instance.Raise(new ChangeResourceEventHandler());
            }
            GameManager.Instance.USER_UNITS[Owner].Add(this);

            UnitManager.SetOwnerColor();
        }

        public bool CanBuy()
        {
            return Data.CanBuy();
        }
    }
}
