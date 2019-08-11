using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

public class PlayMenu : MonoBehaviour
{
    private string _path;
    private Action _backButtonCallback;

    public void setBackCallback(Action backButtonCallback)
    {
        _backButtonCallback = backButtonCallback;
    }

    public void clickPlay()
    {
        if (_path != null && _path.Length > 0)
        {
            SceneManager.LoadScene("AudioLoadingScreen");
        }
    }

    public void clickBack()
    {
        _backButtonCallback();
        _path = null;
        gameObject.SetActive(false);
    }

    public void clickLoadAudioFile()
    {
        _path = EditorUtility.OpenFilePanel("Choose Audio File", "", "");

        if (_path.Length == 0)
        {
            Debug.Log("No File Selected!");
            return;
        }
        // TODO add difficulty choice
        GlobalStorage global = new GlobalStorage();
        GlobalStorage.Instance.Difficulty = Game.DIFFICULTY_EASY;
        GlobalStorage.Instance.AudioPath = _path;
    }
}
