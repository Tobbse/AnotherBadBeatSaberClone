using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using SimpleFileBrowser;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour
{
    private string _path;
    private Action _backButtonCallback;
    private Button _playButton;

    private void Start()
    {
        GameObject playButton = GameObject.Find("PlayButton");
        if (playButton != null)
        {
            _playButton = GameObject.Find("PlayButton").GetComponent<Button>();
            _playButton.enabled = false;
        }
    }

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
        setEnableButtons(false);

        FileBrowser.SetDefaultFilter(".mp3");
        FileBrowser.SetFilters(false, new FileBrowser.Filter[] {new FileBrowser.Filter("Audio Files", ".mp3")});
        FileBrowser.ShowLoadDialog(new FileBrowser.OnSuccess(_onAudioFilePicked), new FileBrowser.OnCancel(_onAudioFileCancelled), false, Application.dataPath + "/Resources/DSP_Test/Audio", "Choose Audio File to Play");
    }

    private void _onAudioFilePicked(string target)
    {
        setEnableButtons(true);
        if (_path != null && _path.Length == 0)
        {
            Debug.Log("No File Selected!");
            return;
        }
        _path = target;
        _playButton.enabled = true;

        GlobalStorage global = new GlobalStorage();
        GlobalStorage.Instance.Difficulty = Game.DIFFICULTY_EASY;
        GlobalStorage.Instance.AudioPath = _path;
    }

    private void _onAudioFileCancelled()
    {
        setEnableButtons(true);
    }

    private void setEnableButtons(bool value)
    {
        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.enabled = value;
        }
    }
}
