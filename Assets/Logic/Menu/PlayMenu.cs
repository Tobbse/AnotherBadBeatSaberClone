using UnityEngine;
using System;
using SimpleFileBrowser;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour
{
    public GameObject audioLoadingStart;
    public GameObject loadingScreen;
    public GameObject leftSaber;
    public GameObject rightSaber;

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
            _setEnableButtons(false);
            leftSaber.SetActive(false);
            rightSaber.SetActive(false);
            gameObject.SetActive(false);
            loadingScreen.SetActive(true);
            Instantiate(audioLoadingStart);
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
        _setEnableButtons(false);

        FileBrowser.SetDefaultFilter(".mp3");
        FileBrowser.SetFilters(false, new FileBrowser.Filter[] {new FileBrowser.Filter("Audio Files", ".mp3")});
        FileBrowser.ShowLoadDialog(new FileBrowser.OnSuccess(_onAudioFilePicked), new FileBrowser.OnCancel(_onAudioFileCancelled), false, Application.dataPath + "/Resources/Audio", "Choose Audio File to Play");
    }

    private void _onAudioFilePicked(string target)
    {
        _setEnableButtons(true);
        if (_path != null && _path.Length == 0)
        {
            Debug.Log("No File Selected!");
            return;
        }
        _path = target;
        _playButton.enabled = true;

        GlobalStorage global = new GlobalStorage();
        GlobalStorage.getInstance().Difficulty = Game.DIFFICULTY_EASY;
        GlobalStorage.getInstance().AudioPath = _path;
    }

    private void _onAudioFileCancelled()
    {
        _setEnableButtons(true);
    }

    private void _setEnableButtons(bool value)
    {
        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.enabled = value;
        }
    }
}
