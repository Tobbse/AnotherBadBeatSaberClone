using BeatMappingConfigs;
using UnityEngine;
using System.Collections.Generic;

public class PostOnsetAudioAnalyzer
{
    private const float MIN_DOUBLE_BLOCK_NOTE_TIME_INTERVAL = 0.25f;
    private const float DOUBLE_BLOCK_CONNECTION_NOTE_TIME_INTERVAL = 4 * MIN_DOUBLE_BLOCK_NOTE_TIME_INTERVAL;

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
        _handleCloseBlocks();
        _polishDoubleBlocks();
    }

    public MappingContainer getBeatMappingContainer()
    {
        return _beatMappingContainer;
    }

    private void _handleCloseBlocks()
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

            if (lastNote == null)
            {
                lastNote = note;
                continue;
            }

            float distance = Mathf.Abs(note.time - lastNote.time);

            if (distance < MIN_DOUBLE_BLOCK_NOTE_TIME_INTERVAL)
            {
                _doubleNoteIndices.Add(new int[]{ i - 1, i });

                NoteConfig[] newNotes = _getDoubleNotes(lastNote.time);
                noteData[i - 1] = newNotes[0];
                noteData[i] = newNotes[1];

                i += 1; // Don't use the same note again.
            }
            if (i >= noteData.Count) break;
            lastNote = noteData[i];
        }
    }

    private NoteConfig[] _getDoubleNotes(float time)
    {
        int layerSeed = _getRandomLayerSeed();
        int indexSeed = _getRandomIndexSeed();
        int leftIndex = _getIndex(indexSeed, true);
        int rightIndex = _getIndex(indexSeed, false);
        bool areBlocksAdjacent = Mathf.Abs(leftIndex - rightIndex) <= 1;
        int leftCutDirection = _getRandomCutDirection(areBlocksAdjacent);
        int rightCutDirection = Random.Range(0, 100) < 85 ? leftCutDirection : _getOppositeCutDirection(leftCutDirection);

        NoteConfig leftDoubleNoteCfg = new NoteConfig();
        leftDoubleNoteCfg.belongsToDoubleNote = true;
        leftDoubleNoteCfg.type = NoteConfig.NOTE_TYPE_LEFT;
        leftDoubleNoteCfg.time = time;
        leftDoubleNoteCfg.lineLayer = _getLayer(layerSeed, true);
        leftDoubleNoteCfg.lineIndex = leftIndex;
        leftDoubleNoteCfg.cutDirection = leftCutDirection;

        NoteConfig rightDoubleNoteCfg = new NoteConfig();
        rightDoubleNoteCfg.belongsToDoubleNote = true;
        rightDoubleNoteCfg.type = NoteConfig.NOTE_TYPE_RIGHT;
        rightDoubleNoteCfg.time = time;
        rightDoubleNoteCfg.lineLayer = _getLayer(layerSeed, false);
        rightDoubleNoteCfg.lineIndex = rightIndex;
        rightDoubleNoteCfg.cutDirection = rightCutDirection;


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

    private int _getRandomCutDirection(bool areBlocksAdjacent)
    {
        // TODO implement this!
        // TODO also after implementing this, you probably have to iterate over every double note again, check how close they are,
        // and if they are close change the cut direction accordingly.
        /*switch (layer)
        {
            case NoteConfig.LINE_LAYER_0:
        }*/

        // ALSO we have to iterate over all blocks again and calculate some sort of block_height_difference
        // and block_horizontal_difference and if the distribution is spread out too much, align blocks better.
        // OR basically if a double block is close to another one, just make the second one exactly the same but
        // with inverted slicing direction

        // When the blocks are adjacent, the cut direction cannot be horizontal as that's hardly possible to cut.
        // Same with diagonal blocks, because they can also face each other.
        // We then have to adjust the percentage steps.
        int rand = Random.Range(0, 100);
        if (areBlocksAdjacent)
        {
            if (rand > 92.5f) return NoteConfig.CUT_DIRECTION_315;  // 7.5 %
            if (rand >   85f) return NoteConfig.CUT_DIRECTION_45;   // 7.5 %
            if (rand > 77.5f) return NoteConfig.CUT_DIRECTION_225;  // 7.5 %
            if (rand >   70f) return NoteConfig.CUT_DIRECTION_135;  // 7.5 %
            if (rand >   35f) return NoteConfig.CUT_DIRECTION_0;    //  35 ‬%
            else              return NoteConfig.CUT_DIRECTION_180;  //  35 %
        } else
        {
            if (rand > 95) return NoteConfig.CUT_DIRECTION_315;  //  5 %
            if (rand > 90) return NoteConfig.CUT_DIRECTION_45;   //  5 %
            if (rand > 85) return NoteConfig.CUT_DIRECTION_225;  //  5 %
            if (rand > 80) return NoteConfig.CUT_DIRECTION_135;  //  5 %
            if (rand > 60) return NoteConfig.CUT_DIRECTION_270;  // 20 %
            if (rand > 40) return NoteConfig.CUT_DIRECTION_90;   // 20 %
            if (rand > 20) return NoteConfig.CUT_DIRECTION_0;    // 20 %
            else           return NoteConfig.CUT_DIRECTION_180;  // 20 %
        }
    }


    // Meh, doesn't work like this unfortunately. Values have to be 0 to 8, because of BSaber. Can't use angles.
    /*private int _getOppositeCutDirection(int cutDirection)
    {
        return Mathf.Abs(cutDirection - 180);
    }*/

    private int _getOppositeCutDirection(int cutDirection)
    {
        switch (cutDirection)
        {
            case NoteConfig.CUT_DIRECTION_0:
                return NoteConfig.CUT_DIRECTION_180;

            case NoteConfig.CUT_DIRECTION_45:
                return NoteConfig.CUT_DIRECTION_225;

            case NoteConfig.CUT_DIRECTION_90:
                return NoteConfig.CUT_DIRECTION_270;

            case NoteConfig.CUT_DIRECTION_135:
                return NoteConfig.CUT_DIRECTION_315;

            case NoteConfig.CUT_DIRECTION_180:
                return NoteConfig.CUT_DIRECTION_0;

            case NoteConfig.CUT_DIRECTION_225:
                return NoteConfig.CUT_DIRECTION_45;

            case NoteConfig.CUT_DIRECTION_270:
                return NoteConfig.CUT_DIRECTION_90;

            case NoteConfig.CUT_DIRECTION_315:
                return NoteConfig.CUT_DIRECTION_135;
        }
        Debug.LogError("Incorrect Cut Direction!");
        return -1;
    }

    private void createRandomStuff()
    {

    }

    private void _polishDoubleBlocks()
    {
        List<NoteConfig> noteData = _beatMappingContainer.noteData;

        NoteConfig lastLeftBlock = null;
        NoteConfig lastRightBlock = null;

        foreach (int[] doubleBlockIndices in _doubleNoteIndices)
        {
            NoteConfig leftBlock = noteData[doubleBlockIndices[0]];
            NoteConfig rightBlock = noteData[doubleBlockIndices[1]];
            if (lastLeftBlock == null) // Should be okay to check only one of them, for performance reasons.
            {
                lastLeftBlock = leftBlock;
                lastRightBlock = rightBlock;
                continue;
            }

            if (lastLeftBlock.time + DOUBLE_BLOCK_CONNECTION_NOTE_TIME_INTERVAL > leftBlock.time)
            {
                leftBlock.cutDirection = _getOppositeCutDirection(lastLeftBlock.cutDirection);
                rightBlock.cutDirection = _getOppositeCutDirection(lastRightBlock.cutDirection);

                int rand = Random.Range(0, 100);
                if (rand > 85)
                {
                    leftBlock.lineLayer  = Mathf.Max(NoteConfig.LINE_LAYER_0, lastLeftBlock.lineLayer  - 1);
                    rightBlock.lineLayer = Mathf.Max(NoteConfig.LINE_LAYER_0, lastRightBlock.lineLayer - 1);
                } else if (rand > 70) {
                    leftBlock.lineLayer  = Mathf.Min(NoteConfig.LINE_LAYER_3, lastLeftBlock.lineLayer  + 1);
                    rightBlock.lineLayer = Mathf.Min(NoteConfig.LINE_LAYER_3, lastRightBlock.lineLayer + 1);
                } else
                {
                    leftBlock.lineLayer  = lastLeftBlock.lineLayer;
                    rightBlock.lineLayer = lastRightBlock.lineLayer;
                }


                rand = Random.Range(0, 100);
                if (rand > 85)
                {
                    leftBlock.lineIndex = Mathf.Max(NoteConfig.LINE_INDEX_0, lastLeftBlock.lineIndex - 1);
                    rightBlock.lineIndex = Mathf.Max(NoteConfig.LINE_INDEX_0, lastRightBlock.lineIndex - 1);
                }
                else if (rand > 70)
                {
                    leftBlock.lineIndex = Mathf.Min(NoteConfig.LINE_INDEX_3, lastLeftBlock.lineIndex + 1);
                    rightBlock.lineIndex = Mathf.Min(NoteConfig.LINE_INDEX_3, lastRightBlock.lineIndex + 1);
                }
                else
                {
                    leftBlock.lineIndex = lastLeftBlock.lineIndex;
                    rightBlock.lineIndex = lastRightBlock.lineIndex;
                }

            }

        }
    }

    /*private int _getAdjacentOrSameLayer(int layer)
    {
        int newLayer = Mathf.CeilToInt(Random.Range(layer - 2, layer + 1));

        if (newLayer < NoteConfig.LINE_LAYER_0) return NoteConfig.LINE_LAYER_0;
        if (newLayer > NoteConfig.LINE_LAYER_3) return NoteConfig.LINE_LAYER_3;
        return newLayer;
    }*/

    private int _getOtherNoteType(int type)
    {
        return type == NoteConfig.NOTE_TYPE_LEFT ? NoteConfig.NOTE_TYPE_RIGHT : NoteConfig.NOTE_TYPE_LEFT;
    }

    private int _getRandomLayerSeed()
    {
        int rand = Random.Range(0, 100);
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
        int rand = Random.Range(0, 100);
        if (rand > 90) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_4;  // 10%
        if (rand > 80) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_3;  // 10%
        if (rand > 40) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_2;  // 40%
        else return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_1;  // 40%
    }
}
