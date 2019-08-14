using UnityEngine;
using System.Collections.Generic;
using BeatMappingConfigs;

public class ObstacleSpawner : ScriptableObject
{
    // TODO investigate: constant travel time and distance, or dynamic based on the bpm or something?
    public const float OBSTACLE_DISTANCE = 40;
    public const float OBSTACLE_TRAVEL_TIME = 5.0f;

    private GameObject _obstacle;
    private List<ObstacleConfig> _obstacleData;
    private GameObject _obj;
    private ObstacleConfig _cfg;
    private float _obstacleTime;

    public ObstacleSpawner(List<ObstacleConfig> obstacleData, GameObject obstacle)
    {
        _obstacleData = obstacleData;
        _obstacle = obstacle;
    }

    public void checkBlocksSpawnable(float timePassed)
    {
        _obstacleTime = timePassed - OBSTACLE_TRAVEL_TIME;

        while (_obstacleData.Count > 0)
        {
            _cfg = _obstacleData[0];
            if (_cfg.time <= _obstacleTime)
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

        float length = _durationToWidth(obstacleConfig.duration);
        float xPos = (OBSTACLE_DISTANCE * -1) - (length / 2);
        float yPos = 0;
        float zPos = ObjectSpawnPositioner.getHorizontalPosition(obstacleConfig.lineIndex);

        _obj = Instantiate(_obstacle, new Vector3(xPos, yPos, zPos), Quaternion.identity);
        _obj.layer = 11;
        _obj.transform.localScale = new Vector3(length, 7.0f, obstacleConfig.width);

        _obj.GetComponent<Rigidbody>().velocity = new Vector3(OBSTACLE_DISTANCE / OBSTACLE_TRAVEL_TIME, 0, 0);
    }

    private float _durationToWidth(float duration)
    {
        return OBSTACLE_DISTANCE / OBSTACLE_TRAVEL_TIME * duration;
    }
}