using BeatMappingConfigs;
using UnityEngine;
using System.Collections.Generic;

public class PostOnsetAudioAnalyzer
{
    private const float MIN_NOTE_TIME_INTERVAL = 0.1f;
    private enum DOUBLE_NOTE_LAYER_TYPES { LAYER_1 = 1, LAYER_2 = 2, LAYER_3 = 3, LAYER_4 = 4, LAYER_5 = 5, LAYER_6 = 6, LAYER_7 = 7, LAYER_8 = 8 };
    private enum DOUBLE_NOTE_INDEX_TYPES { INDEX_1 = 1, INDEX_2 = 2, INDEX_3 = 3, INDEX_4 = 4 };
    private List<int[]> _doubleNoteIndices = new List<int[]>();

    private MappingContainer _beatMappingContainer;

    public PostOnsetAudioAnalyzer(MappingContainer beatMappingContainer)
    {
        _beatMappingContainer = beatMappingContainer;
    }

    public void analyze()
    {

    }

    public MappingContainer getBeatMappingContainer()
    {
        return _beatMappingContainer;
    }

    private void handleCloseBlocks()
    {
        // Iterate over all notes of the same type.
        // If too close, make double blocks at the first time.
        // Check if new blockades are created.


        NoteConfig lastNote = null;
        NoteConfig note;
        List<NoteConfig> noteData = _beatMappingContainer.noteData;

        for (int i = 0; i < noteData.Count; i++)
        {
            note = noteData[i];

            if (lastNote != null && note.lineLayer == lastNote.lineLayer || note.lineIndex == lastNote.lineIndex)
            {
                float distance = Mathf.Abs(note.time - lastNote.time);
                if (distance < MIN_NOTE_TIME_INTERVAL)
                {
                    _doubleNoteIndices.Add(new int[]{ i - 1, i });
                }

                i += 1; // Don't use the same note again.
                NoteConfig[] newNotes = _getDoubleNotes(lastNote.time);
                noteData[i - 1] = newNotes[0];
                noteData[i] = newNotes[1];
            }
            lastNote = note;
        }
    }

    private NoteConfig[] _getDoubleNotes(float time)
    {
        int layerSeed = _getRandomLayerSeed();
        int indexSeed = _getRandomIndexSeed();

        NoteConfig leftDoubleNoteCfg = new NoteConfig();
        leftDoubleNoteCfg.belongsToDoubleNote = true;
        leftDoubleNoteCfg.type = NoteConfig.NOTE_TYPE_LEFT;
        leftDoubleNoteCfg.time = time;
        leftDoubleNoteCfg.lineLayer = _getLayer(layerSeed, true);
        leftDoubleNoteCfg.lineIndex = _getIndex(indexSeed, true);
        leftDoubleNoteCfg.cutDirection = _getCutDirection(layerSeed, indexSeed, true);

        NoteConfig rightDoubleNoteCfg = new NoteConfig();
        rightDoubleNoteCfg.belongsToDoubleNote = true;
        rightDoubleNoteCfg.type = NoteConfig.NOTE_TYPE_RIGHT;
        rightDoubleNoteCfg.time = time;
        rightDoubleNoteCfg.lineLayer = _getLayer(layerSeed, false);
        rightDoubleNoteCfg.lineIndex = _getIndex(indexSeed, false);
        rightDoubleNoteCfg.cutDirection = _getCutDirection(layerSeed, indexSeed, false);


        NoteConfig[] newNotes = { leftDoubleNoteCfg, rightDoubleNoteCfg };
        return newNotes;

    }

    private int _getIndex(int indexType, bool isLeftNote)
    {
        switch (indexType)
        {
            case (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_1: // Left left, Right right,
                return isLeftNote ? NoteConfig.LINE_INDEX_0 : NoteConfig.LINE_INDEX_3;

            case (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_2: // Left slightly left, right slightly right.
                return isLeftNote ? NoteConfig.LINE_INDEX_1 : NoteConfig.LINE_INDEX_2;

            case (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_3: // Left right, right left.
                return isLeftNote ? NoteConfig.LINE_INDEX_3 : NoteConfig.LINE_INDEX_0;

            case (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_4: // Left slightly right, right slightly left.
                return isLeftNote ? NoteConfig.LINE_INDEX_2 : NoteConfig.LINE_INDEX_1;
        }
        Debug.LogError("Incorrect Note Index!");
        return -1;
    }

    private int _getLayer(int layerType, bool isLeftNote)
    {
        switch (layerType)
        {
            case (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_1: // Both up.
                return NoteConfig.LINE_LAYER_3;

            case (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_2: // Both down.
                return NoteConfig.LINE_LAYER_0;

            case (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_3: // Both lower middle.
                return NoteConfig.LINE_LAYER_1;

            case (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_4: // Both upper middle.
                return NoteConfig.LINE_LAYER_2;

            case (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_5: // Left down, right up.
                return isLeftNote ? NoteConfig.LINE_LAYER_0 : NoteConfig.LINE_LAYER_3;

            case (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_6: // Left up, right down.
                return isLeftNote ? NoteConfig.LINE_LAYER_3 : NoteConfig.LINE_LAYER_0;

            case (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_7: // Left slightly down, right slightly up.
                return isLeftNote ? NoteConfig.LINE_LAYER_1 : NoteConfig.LINE_LAYER_2;

            case (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_8: // Left slightly up, right slightly down.
                return isLeftNote ? NoteConfig.LINE_LAYER_2 : NoteConfig.LINE_LAYER_1;
        }
        Debug.LogError("Incorrect Note Layer!");
        return -1;
    }

    private int _getCutDirection(int layer, int index, bool isLeftNote)
    {
        // TODO implement this!
        // TODO also after implementing this, you probably have to iterate over every double note again, check how close they are,
        // and if they are close change the cut direction accordingly.
        /*switch (layer)
        {
            case NoteConfig.LINE_LAYER_0:
        }*/
        return NoteConfig.CUT_DIRECTION_0;
    }

    private void createRandomStuff()
    {

    }

    private int _getOtherNoteType(int type)
    {
        return type == NoteConfig.NOTE_TYPE_LEFT ? NoteConfig.NOTE_TYPE_RIGHT : NoteConfig.NOTE_TYPE_LEFT;
    }

    private int _getRandomLayerSeed()
    {
        int rand = Random.Range(95, 100);
        if (rand > 95) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_8;  // 5%
        if (rand > 90) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_7;  // 5%
        if (rand > 85) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_6;  // 5%
        if (rand > 80) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_5;  // 5%
        if (rand > 65) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_4;  // 15%
        if (rand > 50) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_3;  // 15%
        if (rand > 25) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_2;  // 25%
        else return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_1;  // 25%
    }

    private int _getRandomIndexSeed()
    {
        int rand = Random.Range(95, 100);
        if (rand > 90) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_4;  // 10%
        if (rand > 80) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_3;  // 10%
        if (rand > 40) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_2;  // 40%
        else return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_1;  // 40%
    }
}
