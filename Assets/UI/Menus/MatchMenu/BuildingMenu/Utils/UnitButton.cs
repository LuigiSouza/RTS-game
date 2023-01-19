using T4.Events;
using T4.Managers;
using T4.Units;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace T4.UI.Utils
{
    [RequireComponent(typeof(Button))]
    public class UnitButton : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Button Button { get; private set; }
        private UnitData unitData;

        public void Initialize(UnitData unitData)
        {
            this.unitData = unitData;
            Button = GetComponent<Button>();
            TMP_Text t = GetComponentInChildren<TMP_Text>();
            t.text = unitData.code.ToString();
            Button.name = unitData.code.ToString();
        }

        public void SetInteractable(bool interactable)
        {
            Button.interactable = interactable;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventManager.Instance.Raise(new ShowBuildingCostEventHandler(unitData));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EventManager.Instance.Raise(new HideBuildingCostEventHandler());
        }
    }
}