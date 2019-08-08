using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void clickSinglePlayer()
    {
        SceneManager.LoadScene("SingleplayerMenu");
    }

    public void clickMultiPlayer()
    {
        Debug.Log("This does not do anything yet!");
    }

    public void clickOnsetTest()
    {
        SceneManager.LoadScene("OnsetTest");
    }

    public void clickOptions()
    {
        SceneManager.LoadScene("OptionsMenu");
    }

    public void clickQuit()
    {
        Application.Quit();
    }
}
