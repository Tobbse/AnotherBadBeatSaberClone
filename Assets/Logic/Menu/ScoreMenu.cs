using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreMenu : MonoBehaviour
{
    void Start()
    {
        GameObject.Find("HITS").GetComponent<Text>().text = "HITS: " + ScoreTracker.Instance.Hits.ToString();
        GameObject.Find("SCORE").GetComponent<Text>().text = "SCORE: " + ScoreTracker.Instance.Score.ToString();
        GameObject.Find("MISSES").GetComponent<Text>().text = "MISSES: " + ScoreTracker.Instance.Misses.ToString();
        GameObject.Find("TOTAL").GetComponent<Text>().text = "TOTAL: " + ScoreTracker.Instance.TotalBeats.ToString();
    }

    public void clickReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
