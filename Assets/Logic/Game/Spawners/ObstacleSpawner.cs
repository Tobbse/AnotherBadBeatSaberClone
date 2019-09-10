using UnityEngine;
using System.Collections.Generic;
using BeatMappingConfigs;

public class ObstacleSpawner : ScriptableObject
{
    public const float OBSTACLE_DISTANCE = 40;
    public const float OBSTACLE_TRAVEL_TIME = 3.0f;

    private GameObject _obstacle;
    private List<ObstacleConfig> _obstacleData;
    private GameObject _obj;
    private ObstacleConfig _cfg;
    private float _speed;
    private float _bps;
    private float _relativeTravelTime;

    /**
    * Contains note mapping data and spawns obstacles at the correct time. The updating is triggered from the 'Game' object,
    * which contains the main loop updating the spawner objects.
     **/
    public ObstacleSpawner(List<ObstacleConfig> obstacleData, float bps, GameObject obstacle)
    {
        _obstacleData = obstacleData;
        _obstacle = obstacle;
        _bps = bps;

        _relativeTravelTime = OBSTACLE_TRAVEL_TIME;
        _speed = OBSTACLE_DISTANCE / _relativeTravelTime;
    }

    public void checkObstaclesSpawnable(float timePassed)
    {
        while (_obstacleData.Count > 0)
        {
            _cfg = _obstacleData[0];
            if (_cfg.Time <= timePassed)
            {
                _spawnObstacle(_cfg);
                _obstacleData.RemoveAt(0);
            }
            else break;
        }
    }

    public float getRelativeTravelTime()
    {
        return _relativeTravelTime;
    }

    // Spawns an obstacles object and sets the values of that object, according to the note config.
    private void _spawnObstacle(ObstacleConfig obstacleConfig)
    {
        if (!DevSettings.USE_OBSTACLES) return;

        float obstacleLength = Mathf.Max(_secondsToScale(obstacleConfig.Duration), 0.1f);
        Vector3 position = new Vector3(
            OBSTACLE_DISTANCE * -1 - obstacleLength / 2,
            3f,
            ObjectSpawnPositionProvider.getHorizontalPosition(obstacleConfig.LineIndex)
        );

        _obj = Instantiate(_obstacle, position, Quaternion.identity);
        _obj.layer = 11;
        _obj.transform.localScale = new Vector3(obstacleLength, 3.0f, obstacleConfig.Width);
        _obj.GetComponent<Rigidbody>().velocity = new Vector3(_speed, 0, 0);
    }

    private float _secondsToScale(float duration)
    {
        return _speed * duration / _bps;
    }
}