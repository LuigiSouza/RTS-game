using System.Collections.Generic;
using T4.Managers;
using T4.UI.Match.Info;
using T4.Fog;
using UnityEngine;
using T4.Events;
using System;
using System.Collections;
using T4.Units.Abilities;

namespace T4.Units
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class UnitManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject selectionCircle;

        [SerializeField]
        private GameObject healthBarPrefab;

        [SerializeField]
        private FieldOfView _fov;

        public FieldOfView FOV { get { return _fov; } }


        private UnityHealthBar healthBar;
        private Transform canvas;

        protected BoxCollider _collider;
        public virtual Unit Unit { get; protected set; }
        public UnitData Data { get { return Unit.Data; } }

        private bool isUsingAbility = false;

        public bool CastingAbility { get => isUsingAbility; }

        private void Awake()
        {
            canvas = GameObject.Find("Canvas").transform;
        }

        public void Initialize(Unit unit)
        {
            _collider = GetComponent<BoxCollider>();
            this.Unit = unit;
            SetOwnerColor();
        }

        private void OnMouseDown()
        {
            if (IsActive())
            {
                Select(true, Input.GetKey(KeyCode.LeftShift));
            }
        }
        private void SelectUtil()
        {
            if (IsSelected()) return;
            CreateHealthBar();

            GameManager.Instance.SELECTED_UNITS.Add(this);
            selectionCircle.SetActive(true);
        }

        public void Select() { Select(false, false); }
        public void Select(bool singleClick, bool holdingShift)
        {
            if (!IsActive())
            {
                return;
            }

            if (!singleClick)
            {
                SelectUtil();
                return;
            }
            if (!holdingShift)
            {
                List<UnitManager> selectedUnits = new(GameManager.Instance.SELECTED_UNITS);
                foreach (UnitManager u in selectedUnits)
                {
                    u.Deselect();
                }
                SelectUtil();
            }
            else
            {
                if (!IsSelected())
                {
                    SelectUtil();
                }
                else
                {
                    Deselect();
                }
            }
        }

        public bool IsSelected()
        {
            return GameManager.Instance.SELECTED_UNITS.Contains(this);
        }

        public void Deselect()
        {
            if (!IsSelected()) return;
            if (healthBar)
            {
                Destroy(healthBar.gameObject);
                healthBar = null;
            }

            GameManager.Instance.SELECTED_UNITS.Remove(this);
            selectionCircle.SetActive(false);
        }
        protected virtual bool IsActive()
        {
            return true;
        }
        public bool IsPlayersUnit()
        {
            return Unit.Owner == GameManager.Instance.PlayerId;
        }

        private void CreateHealthBar()
        {
            if (healthBar == null && healthBarPrefab != null)
            {
                GameObject healthBarObj = Instantiate(healthBarPrefab, canvas);
                healthBar = healthBarObj.GetComponent<UnityHealthBar>();
                healthBar.Initialize(transform);
                healthBar.SetPosition();
                healthBar.UpdateHealth(Unit.HP, Unit.MaxHP);
            }
        }

        public void SetOwnerColor()
        {
            Color playerColor = GameManager.Instance.GetPlayer(Unit.Owner).color;
            Material[] materials = transform.Find("Mesh").GetComponent<Renderer>().materials;
            materials[0].color = playerColor;
            transform.Find("Mesh").GetComponent<Renderer>().materials = materials;
        }

        public void UpdateHealthBar()
        {
            if (healthBar != null) healthBar.UpdateHealth(Unit.HP, Unit.MaxHP);
        }

        private List<Tuple<int, GameObject>> queueSkill = new();

        public void TriggerSkill(int index, GameObject target = null)
        {
            queueSkill.Add(new Tuple<int, GameObject>(index, target));
            if (!isUsingAbility) StartCoroutine(QueueHandler());
        }

        private IEnumerator QueueHandler()
        {
            SetUsingAbility(true);
            yield return null;
            while (queueSkill.Count > 0)
            {
                Tuple<int, GameObject> value = queueSkill[0]; queueSkill.RemoveAt(0);
                AbilityManager ability = Unit.AbilityManagers[value.Item1];
                yield return ability.Trigger(value.Item2);
                if (!ability.Succeed) queueSkill.RemoveAll(e => e.Item1 == value.Item1);
            }
            SetUsingAbility(false);
            yield return null;
        }
        private void SetUsingAbility(bool isUsing)
        {
            isUsingAbility = isUsing;
        }

        public virtual void Kill()
        {
            GameManager.Instance.USER_UNITS[Data.owner].Remove(Unit);
            Destroy(gameObject);
        }
    }
}