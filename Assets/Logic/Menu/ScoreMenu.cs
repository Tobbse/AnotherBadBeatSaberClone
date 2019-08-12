using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ScoreMenu : MonoBehaviour
{
    private HighscoreData _currentScore;
    private FastList<HighscoreData> _highscores;

    void Start()
    {
        _handleHighscores();
        _displayScore();
    }

    private void _handleHighscores()
    {
        HighscoreHandler handler = new HighscoreHandler(ScoreTracker.Instance.Score);
        _highscores = handler.getHighscores();
        _currentScore = handler.CurrentScore;
    }

    private void _displayScore()
    {
        GameObject.Find("HITS").GetComponent<Text>().text = "HITS: " + ScoreTracker.Instance.Hits.ToString();
        GameObject.Find("MISSES").GetComponent<Text>().text = "MISSES: " + ScoreTracker.Instance.Misses.ToString();
        GameObject.Find("BEATS").GetComponent<Text>().text = "BEATS: " + ScoreTracker.Instance.NumBeats.ToString();

        GameObject.Find("SCORE").GetComponent<Text>().text = "SCORE: " + _currentScore.score;
        GameObject.Find("RANK").GetComponent<Text>().text = "RANK: " + _currentScore.rank;
        GameObject.Find("HIGHSCORE").GetComponent<Text>().text = "HIGHSCORE: " + _highscores[0].score;

    }

    public void clickReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
