using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void playGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void loadOnsetTest()
    {
        SceneManager.LoadScene("Onset_Test");
    }

    public void loadOptions()
    {
        SceneManager.LoadScene("Options");
    }
}
