using UnityEngine;
using System.Collections.Generic;
using BeatMappingConfigs;

/**
 * This spawns a bunch of random notes that can be used in a Saber Test Scene, to evaluate the cutting and slicing functionality.
 **/
public class SabreTest : MonoBehaviour
{
    public GameObject leftTimedBlock;
    public GameObject rightTimedBlock;

    private Dictionary<int, int> _cutDirectionMapping = new Dictionary<int, int>();
    private Dictionary<int, GameObject> _blockTypeMapping = new Dictionary<int, GameObject>();

    private void Awake()
    {
        ScoreTracker.getInstance().NumBeats = 1;
        GlobalSettings.OVERRIDE_BLOCK_DESPAWN = true;
        GlobalSettings.USE_SABRE_DEBUG_RAYS = true;
    }

    void Start()
    {
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_0] = 0;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_90] = 90;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_180] = 180;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_270] = 270;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_45] = 45;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_135] = 135;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_225] = 225;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_315] = 315;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_NONE] = -1;

        _blockTypeMapping[NoteConfig.NOTE_TYPE_LEFT] = leftTimedBlock;
        _blockTypeMapping[NoteConfig.NOTE_TYPE_RIGHT] = rightTimedBlock;
        

        for (int i = 0; i < 100; i++)
        {
            NoteConfig cfg = createRandomNote();
            GameObject block = Instantiate(_blockTypeMapping[cfg.Type]);

            block.transform.position = new Vector3(Random.Range(0, 7f), Random.Range(0.2f, 3), Random.Range(0, 7));
            int angle = _cutDirectionMapping[cfg.CutDirection];
            block.transform.Rotate(new Vector3(angle, 0, 0));
        }
    }

    private NoteConfig createRandomNote()
    {
        NoteConfig cfg = new NoteConfig();
        cfg.CutDirection = Random.Range(NoteConfig.CUT_DIRECTION_0, NoteConfig.CUT_DIRECTION_NONE + 1);
        cfg.LineIndex = Random.Range(NoteConfig.LINE_INDEX_0, NoteConfig.LINE_INDEX_3 + 1);
        cfg.LineLayer = Random.Range(NoteConfig.LINE_LAYER_0, NoteConfig.LINE_LAYER_3 + 1);
        cfg.Type = Random.Range(NoteConfig.NOTE_TYPE_LEFT, NoteConfig.NOTE_TYPE_RIGHT + 1);
        return cfg;
    }
}
