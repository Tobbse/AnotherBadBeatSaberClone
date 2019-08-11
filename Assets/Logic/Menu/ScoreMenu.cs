using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreMenu : MonoBehaviour
{
    void Start()
    {
        GameObject.Find("HITS").GetComponent<Text>().text = "HITS: " + PScoreTracker.Instance.Hits.ToString();
        GameObject.Find("SCORE").GetComponent<Text>().text = "SCORE: " + PScoreTracker.Instance.Score.ToString();
        GameObject.Find("MISSES").GetComponent<Text>().text = "MISSES: " + PScoreTracker.Instance.Misses.ToString();
        GameObject.Find("TOTAL").GetComponent<Text>().text = "TOTAL: " + PScoreTracker.Instance.TotalBeats.ToString();
    }

    public void clickReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
