using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HighscoreHandler
{
    private JsonController _jsonFileHandler;
    private List<HighscoreData> _highscoreData;
    private HighscoreData _currentScore;

    public HighscoreHandler(int newScore)
    {
        _jsonFileHandler = new JsonController();
        _highscoreData = _getHighscores();

        _currentScore = new HighscoreData();
        _currentScore.score = newScore;

        if (_highscoreData == null)
        {
            _currentScore.rank = 1;
            _highscoreData = new List<HighscoreData>();
            _highscoreData.Add(_currentScore);
        } else
        {
            _handleHighscores();
        }
        _writeHighscores();
    }

    public HighscoreData CurrentScore
    {
        get { return _currentScore; }
    }

    public List<HighscoreData> getHighscores()
    {
        return _highscoreData;
    }

    private void _handleHighscores()
    {
        _highscoreData.Sort(delegate (HighscoreData obj1, HighscoreData obj2) { return obj2.score.CompareTo(obj1.score); });

        bool added = false;
        for (int i = 0; i < _highscoreData.Count; i++)
        {
            HighscoreData score = _highscoreData[i];
            if (!added && score.score < _currentScore.score)
            {
                added = true;
                _currentScore.rank = i + 1;
                _highscoreData.Insert(i, _currentScore);
            }
            _highscoreData[i].rank = i + 1;
        }
        if (!added)
        {
            _currentScore.rank = _highscoreData.Count + 1;
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
