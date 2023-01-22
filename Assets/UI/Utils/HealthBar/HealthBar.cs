using T4.Events;
using T4.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace T4.UI.Match.Info
{
    public abstract class HealthBar : UIBehaviour
    {
        [SerializeField]
        private RectTransform healthRect;
        private Image rectImage;

        protected override void OnEnable()
        {
            rectImage = healthRect.GetComponent<Image>();
        }

        public void UpdateHealth(int value, int maxValue)
        {
            rectImage.fillAmount = (float)value / maxValue;
        }
    }
}
