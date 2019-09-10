using UnityEngine;
using System;
using SimpleFileBrowser;
using UnityEngine.UI;

/**
 * Play Menu UI script to handle button clicks.
 * The UI contains a 'Load File' button. When that button is clicked, a file browser window appears.
 * This was done using the external 'SimpleFileBrowser' library.
 * A song can be and the desired difficulty level can be chosen.
 * 
 * Currently the difficulty level determines the threshold level that will be used in the Audio Analysis.
 * A lower threshold level means that more beats will be detected.
 **/
public class PlayMenu : MonoBehaviour
{
    public GameObject audioController;
    public GameObject loadingScreen;
    public Text difficultyLabel;

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
            GlobalStorage.getInstance().Difficulty = difficultyLabel.text;
            _setEnableButtons(false);
            gameObject.SetActive(false);
            loadingScreen.SetActive(true);
            Instantiate(audioController); // This instantiated object will trigger the audio loading.
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
        FileBrowser.SetFilters(false, new FileBrowser.Filter[] {
            new FileBrowser.Filter("Audio Files", new string[]{
                ".mp3",
                ".egg",
                ".ogg" })
        });
        FileBrowser.ShowLoadDialog(new FileBrowser.OnSuccess(_onAudioFilePicked), new FileBrowser.OnCancel(_onAudioFileCancelled), false, Application.dataPath + "/Resources/SongData/BeatMappings", "Choose Audio File to Play");
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
