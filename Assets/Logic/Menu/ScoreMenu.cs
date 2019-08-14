using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class ScoreMenu : MonoBehaviour
{
    private HighscoreData _currentScore;
    private List<HighscoreData> _highscores;

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
        GameObject.Find("HITS").GetComponent<TextMeshProUGUI>().text = "HITS: " + ScoreTracker.Instance.Hits.ToString();
        GameObject.Find("MISSES").GetComponent<TextMeshProUGUI>().text = "MISSES: " + ScoreTracker.Instance.Misses.ToString();
        GameObject.Find("BEATS").GetComponent<TextMeshProUGUI>().text = "BEATS: " + ScoreTracker.Instance.NumBeats.ToString();

        GameObject.Find("SCORE").GetComponent<TextMeshProUGUI>().text = "SCORE: " + _currentScore.score;
        GameObject.Find("RANK").GetComponent<TextMeshProUGUI>().text = "RANK: " + _currentScore.rank;
        GameObject.Find("HIGHSCORE").GetComponent<TextMeshProUGUI>().text = "HIGHSCORE: " + _highscores[0].score;

    }

    public void clickReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
