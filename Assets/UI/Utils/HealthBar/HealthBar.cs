using UnityEngine;
using UnityEngine.EventSystems;

namespace T4.UI.Match.Info
{
    public abstract class HealthBar : UIBehaviour
    {
        [SerializeField]
        private RectTransform healthRect;
    }
}
