using System.Collections.Generic;
using System.IO;

/**
 * Controller for highscores.
 * Writes and reads highscores files. Interpretes those files and creatings a ranking out of it.
 * Used in the scorescreen.
 * The 10 highest scores are saved. Adds the player's score in the correct spot, if high enough.
 **/
public class HighscoreController
{
    private JsonController _jsonFileHandler;
    private List<HighscoreData> _highscoreData;
    private HighscoreData _currentScore;

    public HighscoreData CurrentScore { get { return _currentScore; } }

    public List<HighscoreData> getHighscores() { return _highscoreData; }

    public HighscoreController(int newScore)
    {
        _jsonFileHandler = new JsonController();
        _highscoreData = _getHighscores();

        _currentScore = new HighscoreData();
        _currentScore.Score = newScore;

        if (_highscoreData == null)
        {
            _currentScore.Rank = 1;
            _highscoreData = new List<HighscoreData>();
            _highscoreData.Add(_currentScore);
        } else
        {
            _handleHighscores();
        }
        _writeHighscores();
    }

    private void _handleHighscores()
    {
        _highscoreData.Sort(delegate (HighscoreData obj1, HighscoreData obj2) { return obj2.Score.CompareTo(obj1.Score); });

        bool added = false;
        for (int i = 0; i < _highscoreData.Count; i++)
        {
            HighscoreData score = _highscoreData[i];
            if (!added && score.Score < _currentScore.Score)
            {
                added = true;
                _currentScore.Rank = i + 1;
                _highscoreData.Insert(i, _currentScore);
            }
            _highscoreData[i].Rank = i + 1;
        }
        if (!added)
        {
            _currentScore.Rank = _highscoreData.Count + 1;
            _highscoreData.Add(_currentScore);
        }

        while (_highscoreData.Count > 10)
        {
            _highscoreData.RemoveAt(_highscoreData.Count - 1);
        }
    }

    private List<HighscoreData> _getHighscores()
    {
        string shortFileName = GlobalStorage.getInstance().TrackConfig.TrackName;
        string difficulty = GlobalStorage.getInstance().Difficulty;
        string fullFilePath = _jsonFileHandler.getFullMappingPath(JsonController.HIGHSCORE_FOLDER_PATH, shortFileName, difficulty);

        if (!File.Exists(fullFilePath)) {
            return null;
        }
        return _jsonFileHandler.readHighscoreFile(fullFilePath);
    }

    private void _writeHighscores()
    {
        string trackName = GlobalStorage.getInstance().TrackConfig.TrackName;
        string difficulty = GlobalStorage.getInstance().Difficulty;
        _jsonFileHandler.writeHighscoreFile(_highscoreData, trackName, difficulty);
    }
}
