using UnityEngine;

namespace T4.Managers
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(UIManager)) as UIManager;

                    if (!_instance)
                    {
                        Debug.LogError($"There needs to be one active {typeof(UIManager)} script on a GameObject in your scene.");
                    }
                }

                return _instance;
            }
        }
    }
}
