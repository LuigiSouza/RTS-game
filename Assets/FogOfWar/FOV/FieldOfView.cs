using UnityEngine;

namespace T4.Fog
{
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField] private Transform FOV;
        [SerializeField] private float FovSize;

        private void Awake()
        {
            if (FOV == null)
            {
                FOV = transform.Find("FOV");
            }
            FOV.localScale = new Vector3(FovSize, 1, FovSize);
        }

        public void SetFovSize(float size)
        {
            FovSize = size;
            FOV.localScale = new Vector3(FovSize, 1, FovSize);
        }

        public void EnableFov()
        {
            gameObject.SetActive(true);
        }
        public void DisableFov()
        {
            gameObject.SetActive(false);
        }
    }
}
