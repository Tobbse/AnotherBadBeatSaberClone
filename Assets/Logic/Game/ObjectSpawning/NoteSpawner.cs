using UnityEngine;
using System.Collections.Generic;
using BeatMappingConfigs;

public class NoteSpawner : ScriptableObject
{
    // TODO investigate: constant travel time and distance, or dynamic based on the bpm or something?
    public const float BLOCK_DISTANCE = 20f;
    public const float BLOCK_TRAVEL_TIME = 2.5f;

    private GameObject _leftTimedBlock;
    private GameObject _rightTimedBlock;
    private Dictionary<int, int> _cutDirectionMapping = new Dictionary<int, int>();
    private Dictionary<int, GameObject> _blockTypeMapping = new Dictionary<int, GameObject>();
    private List<NoteConfig> _noteData;
    private List<GameObject> _blocks = new List<GameObject>();
    private GameObject _obj;
    private NoteConfig _cfg;

    public NoteSpawner(List<NoteConfig> noteData, GameObject leftTimedBlock, GameObject rightTimedBlock)
    {
        _noteData = noteData;
        _leftTimedBlock = leftTimedBlock;
        _rightTimedBlock = rightTimedBlock;

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

    // TODO _timedBlockDistance could just be multiplied by the speed here, same with the speed!
    private void _handleNote(NoteConfig noteConfig)
    {
        Debug.Log("Note time: " + noteConfig.time.ToString());

        float xPos = BLOCK_DISTANCE * -1;
        float yPos = 2.0f + ObjectSpawnPositionProvider.getVerticalPosition(noteConfig.lineLayer);
        float zPos = ObjectSpawnPositionProvider.getHorizontalPosition(noteConfig.lineIndex);

        GameObject prefab = _blockTypeMapping[noteConfig.type];
        _obj = Instantiate(prefab, new Vector3(xPos, yPos, zPos), Quaternion.identity);

        int angle = _cutDirectionMapping[noteConfig.cutDirection];
        _obj.transform.Rotate(new Vector3(angle, 0, 0));

        _obj.GetComponent<Rigidbody>().velocity = new Vector3(BLOCK_DISTANCE / BLOCK_TRAVEL_TIME, 0, 0);
        _blocks.Add(_obj);
    }

    private void _setupMappings()
    {
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_TOP] = 0;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_RIGHT] = 90;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_BOTTOM] = 180;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_LEFT] = 270;

        _blockTypeMapping[NoteConfig.NOTE_TYPE_LEFT] = _leftTimedBlock;
        _blockTypeMapping[NoteConfig.NOTE_TYPE_RIGHT] = _rightTimedBlock;
    }
}