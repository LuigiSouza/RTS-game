using System.Collections.Generic;
using T4.Units.Buildings;
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

namespace T4.UI.Match.Menus
{
    public class MatchMenu : UIBehaviour
    {
        [SerializeField]
        private Transform unitButtons;
        [SerializeField]
        private UnitDetailInfo unitInfo;
        [SerializeField]
        private Transform unitAbilities;
        [SerializeField]
        private UnitOptions unitOptions;
        [Space]

        [SerializeField]
        private GameObject abilityButton;
        [SerializeField]
        private UnitButton unitButton;
        [SerializeField]
        private Transform topBar;
        [SerializeField]
        private GameObject resourceDisplayPrefab;

        [Space]
        [SerializeField]
        private GameObject unitCostInfoPrefab;

        private UnitCostInfo unitCostInfo;

        private Unit selectedUnit;

        private Dictionary<UnitType, UnitButton> dictButtons;
        private Dictionary<ResourceType, TMP_Text> ResourceTexts { get; } = new();

        protected override void Start()
        {
            InitializeResourcesBar();
            InitializeBuildingMenu();
            InitializeBuildingInfo();

            EventManager.Instance.AddListener<ChangeResourceEventHandler>(OnUpdateResourceTexts);
            EventManager.Instance.AddListener<ChangeResourceEventHandler>(OnCheckBuildingButtons);
            EventManager.Instance.AddListener<ShowBuildingCostEventHandler>(OnHoverBuildingButton);
            EventManager.Instance.AddListener<HideBuildingCostEventHandler>(OnUnhoverBuildingButton);
            EventManager.Instance.AddListener<ChangeSelectedUnitsEventHandler>(OnChangeSelectedUnits);
        }

        protected override void OnDisable()
        {
            EventManager.Instance.RemoveListener<ChangeResourceEventHandler>(OnUpdateResourceTexts);
            EventManager.Instance.RemoveListener<ChangeResourceEventHandler>(OnCheckBuildingButtons);
            EventManager.Instance.RemoveListener<ShowBuildingCostEventHandler>(OnHoverBuildingButton);
            EventManager.Instance.RemoveListener<HideBuildingCostEventHandler>(OnUnhoverBuildingButton);
            EventManager.Instance.RemoveListener<ChangeSelectedUnitsEventHandler>(OnChangeSelectedUnits);
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

        private void InitializeBuildingInfo()
        {
            if (unitCostInfoPrefab != null)
            {
                unitCostInfo = unitCostInfoPrefab.GetComponent<UnitCostInfo>();
                unitCostInfo.SetActive(false);
            }
        }

        private void ShowInfoPanel()
        {
            string title = selectedUnit.HP + "/" + selectedUnit.MaxHP;
            unitInfo.SetInfo(title, selectedUnit.Data.description);
            unitInfo.SetActive(true);
            unitOptions.Initialize(selectedUnit);
            unitOptions.SetActive(true);
        }

        private void HideBuildingMenu()
        {
            unitButtons.gameObject.SetActive(false);
            unitInfo.SetActive(false);
        }

        private void HideAbilitiesMenu()
        {
            unitAbilities.gameObject.SetActive(false);
        }

        private void ShowBuildingMenu()
        {
            unitButtons.gameObject.SetActive(true);
            unitInfo.SetActive(false);
        }

        private void InitializeAbilitiesMenu()
        {
            foreach (Transform child in unitAbilities)
            {
                Destroy(child.gameObject);
            }
            if (selectedUnit.AbilityManagers.Count == 0) return;

            GameObject g; Transform t; Button b;
            for (int i = 0; i < selectedUnit.AbilityManagers.Count; i++)
            {
                AbilityManager ability = selectedUnit.AbilityManagers[i];
                g = GameObject.Instantiate(abilityButton, unitAbilities);
                t = g.transform;
                b = g.GetComponent<Button>();
                t.Find("Text").GetComponent<TMP_Text>().text = ability.ability.abilityName;
                AddUnitAbilityButtonListener(selectedUnit, b, i);
            }
            unitAbilities.gameObject.SetActive(true);
        }
        private void InitializeBuildingMenu()
        {
            dictButtons = new Dictionary<UnitType, UnitButton>();
            for (int i = 0; i < GameManager.Instance.BUILDING_DATA.Length; i++)
            {
                GameObject b = Instantiate(
                    unitButton.gameObject,
                    unitButtons);
                BuildingData data = GameManager.Instance.BUILDING_DATA[i];
                unitButton = b.GetComponent<UnitButton>();
                unitButton.Initialize(data);
                dictButtons[data.code] = unitButton;
                AddBuildingButtonListener(unitButton.Button, data.code);
                if (!GameManager.Instance.BUILDING_DATA[i].CanBuy())
                {
                    unitButton.SetInteractable(false);
                }
            }
            unitButtons.gameObject.SetActive(true);
        }

        private void SetResourceText(TMP_Text resourceText, int value)
        {
            resourceText.text = value.ToString();
        }

        private void AddUnitAbilityButtonListener(Unit unit, Button b, int i)
        {
            b.onClick.AddListener(() => unit.TriggerSkill(i));
        }

        private void AddBuildingButtonListener(Button b, UnitType t)
        {
            b.onClick.AddListener(() => BuildingPlacer.Instance.SelectPlacedBuilding(t));
        }

        private void OnChangeSelectedUnits(ChangeSelectedUnitsEventHandler e)
        {
            unitOptions.SetActive(false);
            unitButtons.gameObject.SetActive(false);
            if (GameManager.Instance.SELECTED_UNITS.Count > 1)
            {
                HideBuildingMenu();
                HideAbilitiesMenu();
            }
            else if (GameManager.Instance.SELECTED_UNITS.Count == 1)
            {
                selectedUnit = GameManager.Instance.SELECTED_UNITS[0].Unit;
                HideBuildingMenu();
                ShowInfoPanel();
                InitializeAbilitiesMenu();
            }
            else
            {
                ShowBuildingMenu();
                HideAbilitiesMenu();
            }
        }

        private void OnUpdateResourceTexts(ChangeResourceEventHandler e)
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                Resource.Resource res = GameManager.Instance.GetResource(GameManager.Instance.PlayerId, type);
                SetResourceText(ResourceTexts[type], res.Amount);
            }
        }

        private void OnCheckBuildingButtons(ChangeResourceEventHandler e)
        {
            foreach (BuildingData data in GameManager.Instance.BUILDING_DATA)
            {
                dictButtons[data.code].SetInteractable(data.CanBuy());
            }
        }

        private void OnHoverBuildingButton(ShowBuildingCostEventHandler e)
        {
            SetInfoPanel(e.UnitData);
            ShowInfoPanel(true);
        }

        private void OnUnhoverBuildingButton(HideBuildingCostEventHandler e)
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
