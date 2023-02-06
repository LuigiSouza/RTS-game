using System.Collections.Generic;
using T4.Events;
using T4.Globals;
using T4.Managers;
using T4.Resource;
using T4.UI.Match.Info;
using T4.UI.Utils;
using T4.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using T4.Units.Abilities;
using System;
using T4.Units.Buildings;

namespace T4.UI.Match.Menus
{
    public class MatchMenu : UIBehaviour
    {
        [SerializeField]
        private UnitDetailInfo unitInfo;
        [SerializeField]
        private Transform unitAbilities;
        [SerializeField]
        private UnitOptions unitOptions;
        [Space]

        [SerializeField]
        private SpawnButton abilityButton;
        [SerializeField]
        private Transform topBar;
        [SerializeField]
        private GameObject resourceDisplayPrefab;

        [Space]
        [SerializeField]
        private GameObject unitCostInfoPrefab;

        private UnitCostInfo unitCostInfo;

        private Unit selectedUnit;

        private readonly List<SpawnButton> dictButtons = new();
        private Dictionary<ResourceType, TMP_Text> ResourceTexts { get; } = new();

        protected override void Start()
        {
            InitializeResourcesBar();
            InitializeUnitInfo();

            EventManager.Instance.AddListener<ChangeResourceEventHandler>(UpdateResourceTexts);
            EventManager.Instance.AddListener<ChangeResourceEventHandler>(UpdateAbilitiesButtons);
            EventManager.Instance.AddListener<ShowUnitCostEventHandler>(OnHoverAbilityButton);
            EventManager.Instance.AddListener<HideUnitCostEventHandler>(OnUnhoverAbilityButton);
            EventManager.Instance.AddListener<ChangeSelectedUnitsEventHandler>(OnChangeSelectedUnits);
            EventManager.Instance.AddListener<UpdateHealthHandler>(OnChangeHealth);
        }

        protected override void OnDisable()
        {
            EventManager.Instance.RemoveListener<ChangeResourceEventHandler>(UpdateResourceTexts);
            EventManager.Instance.RemoveListener<ChangeResourceEventHandler>(UpdateAbilitiesButtons);
            EventManager.Instance.RemoveListener<ShowUnitCostEventHandler>(OnHoverAbilityButton);
            EventManager.Instance.RemoveListener<HideUnitCostEventHandler>(OnUnhoverAbilityButton);
            EventManager.Instance.RemoveListener<ChangeSelectedUnitsEventHandler>(OnChangeSelectedUnits);
            EventManager.Instance.RemoveListener<UpdateHealthHandler>(OnChangeHealth);
        }

        private void OnChangeHealth(UpdateHealthHandler e)
        {
            if (selectedUnit == null || e.unit != selectedUnit.UnitManager) return;

            unitInfo.SetHealthBarFill(e.hp, e.maxHp);
        }

        private void InitializeResourcesBar()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                Resource.Resource res = GameManager.Instance.GetResource(GameManager.Instance.PlayerId, type);
                GameObject display = Instantiate(resourceDisplayPrefab, topBar);
                display.name = type.ToString();
                display.transform.Find("Icon").GetComponent<Image>().sprite = GlobalConfigs.Instance.GetResourceImage(type);
                ResourceTexts[type] = display.GetComponentInChildren<TMP_Text>();
                SetResourceText(ResourceTexts[type], res.Amount);
            }
        }

        private void InitializeUnitInfo()
        {
            if (unitCostInfoPrefab != null)
            {
                unitCostInfo = unitCostInfoPrefab.GetComponent<UnitCostInfo>();
                unitCostInfo.SetActive(false);
            }
        }

        private void ShowInfoPanel()
        {
            unitInfo.SetInfo(selectedUnit.Data.description);
            unitInfo.SetActive(true);
            unitInfo.SetHealthBarFill(selectedUnit.HP, selectedUnit.MaxHP);
            unitOptions.Initialize(selectedUnit);
            unitOptions.SetActive(true);
        }

        private void HideUnitMenu()
        {
            unitInfo.SetActive(false);
        }
        private void ShowUnitMenu()
        {
            unitInfo.SetActive(false);
        }

        private void HideAbilitiesMenu()
        {
            unitAbilities.gameObject.SetActive(false);
        }

        private void ShowAbilitiesMenu()
        {
            unitAbilities.gameObject.SetActive(true);
        }

        private void InitializeAbilitiesMenu()
        {
            dictButtons.Clear();
            foreach (Transform child in unitAbilities)
            {
                Destroy(child.gameObject);
            }
            if (selectedUnit is Building && !(selectedUnit as Building).IsReady ||
                selectedUnit.AbilityManagers.Count == 0 ||
                selectedUnit.Owner != GameManager.Instance.PlayerId)
            {
                HideAbilitiesMenu();
                return;
            }

            SpawnButton g; Transform t; Button b;
            for (int i = 0; i < selectedUnit.AbilityManagers.Count; i++)
            {
                AbilityData ability = selectedUnit.AbilityManagers[i].ability;
                g = Instantiate(abilityButton.gameObject, unitAbilities).GetComponent<SpawnButton>();
                g.Initialize(ability.unitReference); g.SetInteractable();
                t = g.transform; b = g.GetComponent<Button>();
                t.Find("Text").GetComponent<TMP_Text>().text = ability.abilityName;
                AddUnitAbilityButtonListener(selectedUnit, b, i);
                dictButtons.Add(g);
            }

            ShowAbilitiesMenu();
        }

        private void SetResourceText(TMP_Text resourceText, int value)
        {
            resourceText.text = value.ToString();
        }

        private void AddUnitAbilityButtonListener(Unit unit, Button b, int i)
        {
            b.onClick.AddListener(() => unit.UnitManager.TriggerSkill(i));
        }

        private void OnChangeSelectedUnits(ChangeSelectedUnitsEventHandler e)
        {
            unitOptions.SetActive(false);
            if (GameManager.Instance.SELECTED_UNITS.Count > 1)
            {
                HideUnitMenu();
                HideAbilitiesMenu();
            }
            else if (GameManager.Instance.SELECTED_UNITS.Count == 1)
            {
                selectedUnit = GameManager.Instance.SELECTED_UNITS[0].Unit;
                HideUnitMenu();
                ShowInfoPanel();
                InitializeAbilitiesMenu();
            }
            else
            {
                ShowUnitMenu();
                HideAbilitiesMenu();
            }
        }

        private void UpdateResourceTexts(ChangeResourceEventHandler e)
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                Resource.Resource res = GameManager.Instance.GetResource(GameManager.Instance.PlayerId, type);
                SetResourceText(ResourceTexts[type], res.Amount);
            }
        }

        private void UpdateAbilitiesButtons(ChangeResourceEventHandler e)
        {
            foreach (SpawnButton value in dictButtons)
            {
                value.SetInteractable();
            }
        }

        private void OnHoverAbilityButton(ShowUnitCostEventHandler e)
        {
            SetInfoPanel(e.UnitData);
            ShowInfoPanel(true);
        }

        private void OnUnhoverAbilityButton(HideUnitCostEventHandler e)
        {
            ShowInfoPanel(false);
        }
        public void SetInfoPanel(UnitData data)
        {
            if (unitCostInfo == null) return;
            unitCostInfo.SetInfo(data.unitName, data.description, data.cost);
        }

        public void ShowInfoPanel(bool show)
        {
            unitCostInfo.SetActive(show);
        }
    }
}
