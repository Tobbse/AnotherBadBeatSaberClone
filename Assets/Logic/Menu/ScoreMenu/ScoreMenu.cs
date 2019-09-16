using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Global;

namespace MenuScoreMenu
{
    /// <summary>
    /// Score Menu UI script, showing the player's score data and the highscore.
    /// Also triggers resettings of player data and score tracker values for the next song to be played.
    /// </summary>
    public class ScoreMenu : MonoBehaviour
    {
        public TextMeshProUGUI headlineText;
        public TextMeshProUGUI hitsText;
        public TextMeshProUGUI missesText;
        public TextMeshProUGUI beatsText;
        public TextMeshProUGUI averageComboText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI highscoreText;

        private HighscoreData _currentScore;
        private List<HighscoreData> _highscores;
        private PlayerData _playerData;
        private ScoreTracker _scoreTracker;

        void Start()
        {
            _scoreTracker = ScoreTracker.getInstance();
            _playerData = PlayerData.getInstance();

            _handleHighscores();
            _displayScore();
        }

        private void _handleHighscores()
        {
            HighscoreController handler = new HighscoreController(ScoreTracker.getInstance().Score);
            _highscores = handler.getHighscores();
            _currentScore = handler.CurrentScore;
        }

        private void _displayScore()
        {
            headlineText.text = _playerData.IsGameOver ? "GAME OVER :(" : "YOUR SCORE :)";
            hitsText.text = "HITS: " + _scoreTracker.Hits.ToString();
            missesText.text = "MISSES: " + _scoreTracker.Misses.ToString();
            beatsText.text = "BEATS: " + _scoreTracker.NumBeats.ToString();
            averageComboText.text = "BEATS: " + _scoreTracker.AverageCombo.ToString();
            scoreText.text = "SCORE: " + _currentScore.Score;
            rankText.text = "RANK: " + _currentScore.Rank;
            highscoreText.text = "HIGHSCORE: " + _highscores[0].Score;

            _resetGlobalInstances();
        }

        private void _resetGlobalInstances()
        {
            PlayerData.getInstance().destroy();
            ScoreTracker.getInstance().destroy();
            GlobalStorage.getInstance().destroy();
        }

        public void clickReturnToMain()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

}
