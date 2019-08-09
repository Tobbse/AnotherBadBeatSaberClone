using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SingleplayerMenu : MonoBehaviour
{
    public static string DIFFICULTY_EASY = "Easy";
    public static string DIFFICULTY_NORMAL = "Normal";
    public static string DIFFICULTY_HARD = "Hard";
    public static string DIFFICULTY_EXPERT = "Expert";
    public static string DIFFICULTY_EXPERT_PLUS = "ExpertPlus";

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
        // TODO add difficulty choice
        GlobalStorage global = new GlobalStorage();
        GlobalStorage.Instance.Difficulty = DIFFICULTY_EASY;
        GlobalStorage.Instance.AudioPath = _path;
    }
}
