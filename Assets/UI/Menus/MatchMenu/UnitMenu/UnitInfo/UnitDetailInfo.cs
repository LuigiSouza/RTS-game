using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace T4.UI.Match.Info
{
    public class UnitDetailInfo : UIBehaviour
    {
        [SerializeField]
        private TMP_Text HealthText;
        [SerializeField]
        private InfoHealthBar HealthBar;
        [SerializeField]
        private TMP_Text Description;

        public void SetInfo(string description)
        {
            Description.text = description;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void SetHealthBarFill(int value, int maxValue)
        {
            HealthText.text = value + "/" + maxValue;
            HealthBar.UpdateHealth(value, maxValue);
        }
    }
}
