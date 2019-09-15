using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleFileBrowser;

namespace MenuMainMenu
{
    /**
     * Main Menu UI script to handle button clicks.
     **/
    public class MainMenu : MonoBehaviour
    {
        public GameObject mainMenu;
        public GameObject playMenu;
        public GameObject optionsMenu;

        private void Start()
        {
            FileBrowser.HideDialog();

            (playMenu.GetComponent<MonoBehaviour>() as PlayMenu).setBackCallback(_setMainMenuActive);
            (optionsMenu.GetComponent<MonoBehaviour>() as OptionsMenu).setBackCallback(_setMainMenuActive);
        }

        public void clickPlay()
        {
            playMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void clickOnsetTest()
        {
            SceneManager.LoadScene("OnsetTest");
        }

        public void clickOptions()
        {
            optionsMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void clickQuit()
        {
            Application.Quit();
        }

        private void _setMainMenuActive()
        {
            mainMenu.SetActive(true);
        }
    }

}
