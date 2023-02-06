
using UnityEngine;
using System;
using System.Collections.Generic;
using T4.Events;
using T4.Fog;
using T4.Managers;
using T4.Units.Abilities;

namespace T4.Units
{
    public class Unit : IDisposable
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
        public List<AbilityManager> AbilityManagers { get => abilitiesManagers; }

        public Unit(UnitData data, int owner)
        {
            data.owner = owner;
            Data = data;
            HP = data.healthpoints;
            Uid = System.Guid.NewGuid().ToString();

            GameObject g = GameObject.Instantiate(Data.prefab);
            _transform = g.transform;
            UnitManager = _transform.GetComponent<UnitManager>();
            UnitManager.Initialize(this);

            AbilityManager sm; abilitiesManagers = new List<AbilityManager>();
            foreach (AbilityData ability in data.abilities)
            {
                sm = g.AddComponent<AbilityManager>(); ability.unitReference.owner = Data.owner;
                sm.Initialize(ability, g);
                abilitiesManagers.Add(sm);
            }
        }

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

        public virtual void TakeHit(int value)
        {
            HP += value;
            UnitManager.UpdateHealthBar();
            EventManager.Instance.Raise(new UpdateHealthHandler(UnitManager, HP, MaxHP));
            if(HP <= 0) UnitManager.Kill();
        }

        public virtual void Dispose()
        {
            GameManager.Instance.USER_UNITS[Owner].Remove(this);
        }
    }
}
