using System.Collections.Generic;
using T4.Globals;
using T4.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace T4.UI.Match.Info
{
    public class UnitCostInfo : UIBehaviour
    {
        [SerializeField]
        private TMP_Text Title;
        [SerializeField]
        private TMP_Text Description;
        [SerializeField]
        private GameObject ResourceCost;

        [Space]
        [SerializeField]
        private GameObject gameResourceCostPrefab;

        public void SetInfo(string title, string description, List<ResourceValue> resources)
        {
            Title.text = title;
            Description.text = description;

            foreach (Transform child in ResourceCost.GetComponentInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }

            if (resources.Count > 0)
            {
                GameObject g; Transform t;
                foreach (ResourceValue res in resources)
                {
                    g = Instantiate(gameResourceCostPrefab, ResourceCost.transform);
                    t = g.transform;
                    t.Find("Text").GetComponent<TMP_Text>().text = res.amount.ToString();
                    t.Find("Icon").GetComponent<Image>().sprite = GlobalConfigs.Instance.GetResourceImage(res.code);
                    if (res.amount > GameManager.Instance.GetResource(GameManager.Instance.PlayerId, res.code).Amount)
                    {
                        t.Find("Text").GetComponent<TMP_Text>().color = Color.red;
                    }
                }
            }
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
