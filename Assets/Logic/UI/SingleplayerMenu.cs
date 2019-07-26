using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SingleplayerMenu : MonoBehaviour
{
    private string _path;

    public void clickPlay()
    {
        if (_path != null && _path.Length > 0)
        {
            SceneManager.LoadScene("AudioLoadingScreen");
        }
    }

    public void clickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void clickLoadAudioFile()
    {
        _path = EditorUtility.OpenFilePanel("Choose Audio File", "", "");

        if (_path.Length == 0)
        {
            Debug.Log("No File Selected!");
            return;
        }
        GlobalStorage global = new GlobalStorage();
        GlobalStorage.Instance.AudioPath = _path;
    }
}
