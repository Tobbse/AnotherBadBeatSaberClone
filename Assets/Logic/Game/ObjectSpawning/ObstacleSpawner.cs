using UnityEngine;
using System.Collections.Generic;
using BeatMappingConfigs;

public class ObstacleSpawner : ScriptableObject
{
    // TODO investigate: constant travel time and distance, or dynamic based on the bpm or something?
    public const float OBSTACLE_DISTANCE = 20f;
    public const float OBSTACLE_TRAVEL_TIME = 2.5f;

    private GameObject _obstacle;
    private List<ObstacleConfig> _obstacleData;
    private GameObject _obj;
    private ObstacleConfig _cfg;
    private float _speed;

    public ObstacleSpawner(List<ObstacleConfig> obstacleData, GameObject obstacle)
    {
        _obstacleData = obstacleData;
        _obstacle = obstacle;
        _speed = OBSTACLE_DISTANCE / OBSTACLE_TRAVEL_TIME;
    }

    public void checkBlocksSpawnable(float timePassed)
    {
        while (_obstacleData.Count > 0)
        {
            _cfg = _obstacleData[0];
            if (_cfg.time <= timePassed)
            {
                _handleObstacle(_cfg);
                _obstacleData.RemoveAt(0);
            }
            else break;
        }
    }

    private void _handleObstacle(ObstacleConfig obstacleConfig)
    {
        // TODO implement properly before using this. Currently the obstacles get destroyed when colliding with the player.
        if (!GlobalStaticSettings.USE_OBSTACLES) return;

        float obstacleLength = _secondsToScale(obstacleConfig.duration);
        Vector3 position = new Vector3(
            OBSTACLE_DISTANCE * -1 - obstacleLength,
            3f,
            ObjectSpawnPositionProvider.getHorizontalPosition(obstacleConfig.lineIndex)
        );

        _obj = Instantiate(_obstacle, position, Quaternion.identity);
        _obj.layer = 11;
        _obj.transform.localScale = new Vector3(obstacleLength, 3.0f, obstacleConfig.width);

        _obj.GetComponent<Rigidbody>().velocity = new Vector3(_speed, 0, 0);
    }

    private float _secondsToScale(float duration)
    {
        return _speed * duration;
    }
}