using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HighscoreHandler
{
    private JsonFileHandler _jsonFileHandler;
    private FastList<HighscoreData> _highscoreData;
    private HighscoreData _currentScore;

    public HighscoreHandler(int newScore)
    {
        _jsonFileHandler = new JsonFileHandler();
        _highscoreData = _getHighscores();

        _currentScore = new HighscoreData();
        _currentScore.score = newScore;

        if (_highscoreData == null)
        {
            _currentScore.rank = 1;
            _highscoreData = new FastList<HighscoreData>();
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

    public FastList<HighscoreData> getHighscores()
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

    private FastList<HighscoreData> _getHighscores()
    {
        string shortFileName = GlobalStorage.Instance.TrackConfig.TrackName;
        string difficulty = GlobalStorage.Instance.Difficulty;
        string fullFilePath = _jsonFileHandler.getFullFilePath(JsonFileHandler.HIGHSCORE_FOLDER_PATH, shortFileName, difficulty);

        if (!File.Exists(fullFilePath)) {
            return null;
        }
        return _jsonFileHandler.readHighscoreFile(fullFilePath);
    }

    private void _writeHighscores()
    {
        string trackName = GlobalStorage.Instance.TrackConfig.TrackName;
        string difficulty = GlobalStorage.Instance.Difficulty;
        _jsonFileHandler.writeHighscoreFile(_highscoreData, trackName, difficulty);
    }
}
