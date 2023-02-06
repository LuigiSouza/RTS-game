using T4.Globals;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace T4.UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene(SceneValues.MainGameScene);
        }
    }
}
