using T4.Events;
using T4.Managers;
using T4.Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace T4.UI.Match.Info
{
    public class UnitOptions : UIBehaviour
    {
        [SerializeField]
        private Button destroyButton;

        private Unit unit;

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            Button b = GetComponentInChildren<Button>();
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => DestroyUnit());
        }

        private void DestroyUnit()
        {
            Debug.Log("Destruir Unidade: " + unit.Code.ToString());
            EventManager.Instance.Raise(new DestroyUnitEventHandler());
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
