using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace T4.UI.Match.Info
{
    public class UnitDetailInfo : UIBehaviour
    {
        [SerializeField]
        private TMP_Text Title;
        [SerializeField]
        private InfoHealthBar HealthBar;
        [SerializeField]
        private TMP_Text Description;

        public void SetInfo(string title, string description)
        {
            Title.text = title;
            Description.text = description;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
