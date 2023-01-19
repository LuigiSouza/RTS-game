using T4.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace T4.UI.Borders
{
    public class ScreenBorder : UIBehaviour
    {
        [SerializeField]
        private CameraDirections screenBorder;

        public CameraDirections direction { get { return screenBorder; } }
    }
}
