using UnityEngine;
using System.Collections.Generic;
using BeatMappingConfigs;

public class NoteSpawner : ScriptableObject
{
    // TODO investigate: constant travel time and distance, or dynamic based on the bpm or something?
    public const float BLOCK_DISTANCE = 20;
    public const float BLOCK_TRAVEL_TIME = 1.5f;

    private GameObject _leftTimedBlock;
    private GameObject _rightTimedBlock;
    private GameObject _leftTimedBlockNoDirection;
    private GameObject _rightTimedBlockNoDirection;
    private Dictionary<int, int> _cutDirectionMapping = new Dictionary<int, int>();
    private Dictionary<int, GameObject> _blockTypeMapping = new Dictionary<int, GameObject>();
    private List<NoteConfig> _noteData;
    private List<GameObject> _blocks = new List<GameObject>();
    private GameObject _obj;
    private NoteConfig _cfg;
    private float _bps;
    private float _relativeTravelTime;
    private float _speed;

    public NoteSpawner(List<NoteConfig> noteData, float bps, GameObject leftTimedBlock, GameObject rightTimedBlock, GameObject leftTimedBlockNoDirection, GameObject rightTimedBlockNoDirection)
    {
        _noteData = noteData;
        _bps = bps;
        _leftTimedBlock = leftTimedBlock;
        _rightTimedBlock = rightTimedBlock;
        _leftTimedBlockNoDirection = leftTimedBlockNoDirection;
        _rightTimedBlockNoDirection = rightTimedBlockNoDirection;

        _relativeTravelTime = BLOCK_TRAVEL_TIME /*/ _bps*/;
        _speed = BLOCK_DISTANCE / _relativeTravelTime;

        _setupMappings();
    }

    public void checkBlocksSpawnable(float timePassed)
    {
        while (_noteData.Count > 0)
        {
            _cfg = _noteData[0];
            if (_cfg.time <= timePassed)
            {
                _handleNote(_cfg);
                _noteData.RemoveAt(0);
            }
            else break;
        }
    }

    public List<GameObject> getBlocks()
    {
        return _blocks;
    }

    public float getRelativeTravelTime()
    {
        return _relativeTravelTime;
    }

    private void _handleNote(NoteConfig noteConfig)
    {
        GameObject prefab = _blockTypeMapping[noteConfig.type];
        Vector3 position = new Vector3(
            BLOCK_DISTANCE * -1,
            2.0f + ObjectSpawnPositionProvider.getVerticalPosition(noteConfig.lineLayer),
            ObjectSpawnPositionProvider.getHorizontalPosition(noteConfig.lineIndex)
        );

        int cutDirection = _cutDirectionMapping[noteConfig.cutDirection];
        if (cutDirection == _cutDirectionMapping[NoteConfig.CUT_DIRECTION_NONE])
        {
            prefab = prefab == _leftTimedBlock ? _leftTimedBlockNoDirection : _rightTimedBlockNoDirection;
            cutDirection = 0;
        }

        _obj = Instantiate(prefab, position, Quaternion.identity);
        _obj.transform.Rotate(new Vector3(cutDirection, 0, 0));
        _obj.GetComponent<Rigidbody>().velocity = new Vector3(_speed, 0, 0);
        _blocks.Add(_obj);
    }

    private void _setupMappings()
    {
        // TODO use enum
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_0] = 0;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_90] = 90;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_180] = 180;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_270] = 270;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_45] = 45;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_135] = 135;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_225] = 225;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_315] = 315;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_NONE] = -1;

        _blockTypeMapping[NoteConfig.NOTE_TYPE_LEFT] = _leftTimedBlock;
        _blockTypeMapping[NoteConfig.NOTE_TYPE_RIGHT] = _rightTimedBlock;
    }
}