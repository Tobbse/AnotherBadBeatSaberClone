using BeatMappingConfigs;
using UnityEngine;
using System.Collections.Generic;

public class PostOnsetAudioAnalyzer
{
    private const float MIN_DOUBLE_BLOCK_NOTE_TIME_INTERVAL = 0.25f;
    private const float DOUBLE_BLOCK_CONNECTION_NOTE_TIME_INTERVAL = 2.0f;
    private const float MIN_SINGLE_BLOCK_NOTE_TIME_INVERVAL = 0.15f;
    private const float SINGLE_BLOCK_CONNECTION_NOTE_TIME_INTERVAL = 1.0f;
    private const float MIN_NOTE_DISTANCE = 0.05f;

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
        _polishSingleBlocks();
        _checkBlockDensity();
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

                NoteConfig[] newNotes = _getDoubleNotes(lastNote.time, lastNote.obstacleLineIndex);
                noteData[i - 1] = newNotes[0];
                noteData[i] = newNotes[1];

                i += 1; // Don't use the same note again.
            }
            if (i >= noteData.Count) break;
            lastNote = noteData[i];
        }
    }

    private NoteConfig[] _getDoubleNotes(float time, int obstacleLineIndex)
    {
        int layerSeed = _getRandomLayerSeed();
        int indexSeed = _getRandomIndexSeed();
        int leftLine = _getLine(indexSeed, true);
        int rightLine = _getLine(indexSeed, false);
        int leftLayer = _getLayer(layerSeed, true);
        int rightLayer = _getLayer(layerSeed, false);

        int blockLineDistance = Mathf.Abs(leftLine - rightLine);
        int blockLayerDistance = Mathf.Abs(leftLayer - rightLayer);

        int[] cutDirections = _getRandomCutDirection(blockLineDistance, blockLayerDistance);

        NoteConfig leftDoubleNoteCfg = new NoteConfig();
        leftDoubleNoteCfg.belongsToDoubleNote = true;
        leftDoubleNoteCfg.type = NoteConfig.NOTE_TYPE_LEFT;
        leftDoubleNoteCfg.time = time;
        leftDoubleNoteCfg.lineLayer = leftLayer;
        leftDoubleNoteCfg.lineIndex = leftLine;
        leftDoubleNoteCfg.cutDirection = cutDirections[0];
        leftDoubleNoteCfg.obstacleLineIndex = obstacleLineIndex;

        NoteConfig rightDoubleNoteCfg = new NoteConfig();
        rightDoubleNoteCfg.belongsToDoubleNote = true;
        rightDoubleNoteCfg.type = NoteConfig.NOTE_TYPE_RIGHT;
        rightDoubleNoteCfg.time = time;
        rightDoubleNoteCfg.lineLayer = rightLayer;
        rightDoubleNoteCfg.lineIndex = rightLine;
        rightDoubleNoteCfg.cutDirection = cutDirections[1];
        rightDoubleNoteCfg.obstacleLineIndex = obstacleLineIndex;

        NoteConfig[] newNotes = { leftDoubleNoteCfg, rightDoubleNoteCfg };
        return newNotes;

    }

    private int _getLine(int indexType, bool isLeftNote)
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

    private int[] _getRandomCutDirection(int lineDistance, int layerDistance)
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

        bool shouldUseHorizontal = true;
        bool shouldUseDiagonal = true;
        bool shouldInvert = false;

        if (lineDistance == 1 && layerDistance == 0)
        {
            shouldUseHorizontal = false;
        }
        if (lineDistance == 1 && layerDistance >= 1)
        {
            shouldUseDiagonal = false;
        }
        if (lineDistance > 1 && layerDistance > 1)
        {
            shouldInvert = true;
        }
        int randomCutDirection;


        if (!shouldUseDiagonal && !shouldUseHorizontal)
        {
            if (rand > 50f) randomCutDirection = NoteConfig.CUT_DIRECTION_0;    // 50.0 ‬% Top
            else            randomCutDirection = NoteConfig.CUT_DIRECTION_180;  // 50.0 % Bottom
        } else if (!shouldUseDiagonal)
        {
            if      (rand > 80f) randomCutDirection = NoteConfig.CUT_DIRECTION_270;  // 20.0 % Left
            else if (rand > 60f) randomCutDirection = NoteConfig.CUT_DIRECTION_90;   // 20.0 % Right
            else if (rand > 30f) randomCutDirection = NoteConfig.CUT_DIRECTION_0;    // 30.0 % Top
            else                 randomCutDirection = NoteConfig.CUT_DIRECTION_180;  // 30.0 % Bottom
        } else if (!shouldUseHorizontal)
        {
            if      (rand > 92.5f) randomCutDirection = NoteConfig.CUT_DIRECTION_315;  //  7.5 % TopLeft
            else if (rand > 85f)   randomCutDirection = NoteConfig.CUT_DIRECTION_45;   //  7.5 % TopRight
            else if (rand > 77.5f) randomCutDirection = NoteConfig.CUT_DIRECTION_225;  //  7.5 % BottomLeft
            else if (rand > 70f)   randomCutDirection = NoteConfig.CUT_DIRECTION_135;  //  7.5 % BottomRight
            else if (rand > 35f)   randomCutDirection = NoteConfig.CUT_DIRECTION_0;    // 35.0 % Top
            else                   randomCutDirection = NoteConfig.CUT_DIRECTION_180;  // 35.0 % Bottom
        } else
        {
            if      (rand > 95f) randomCutDirection = NoteConfig.CUT_DIRECTION_315;  //  5.0 % TopLeft
            else if (rand > 90f) randomCutDirection = NoteConfig.CUT_DIRECTION_45;   //  5.0 % TopRight
            else if (rand > 85f) randomCutDirection = NoteConfig.CUT_DIRECTION_225;  //  5.0 % BottomLeft
            else if (rand > 80f) randomCutDirection = NoteConfig.CUT_DIRECTION_135;  //  5.0 % BottomRight
            else if (rand > 65f) randomCutDirection = NoteConfig.CUT_DIRECTION_270;  // 15.0 % Left
            else if (rand > 50f) randomCutDirection = NoteConfig.CUT_DIRECTION_90;   // 15.0 % Right
            else if (rand > 25f) randomCutDirection = NoteConfig.CUT_DIRECTION_0;    // 25.0 % Top
            else                 randomCutDirection = NoteConfig.CUT_DIRECTION_180;  // 25.0 % Bottom
        }

        int[] cutDirections = new int[] { randomCutDirection, randomCutDirection };
        if (Random.Range(0, 100) > 90 || shouldInvert)
        {
            cutDirections[1] = _getOppositeCutDirection(cutDirections[0]);
        }
        return cutDirections;
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
                if (rand > 95)
                {
                    leftBlock.lineLayer  = Mathf.Max(NoteConfig.LINE_LAYER_0, lastLeftBlock.lineLayer  - 1);
                    rightBlock.lineLayer = Mathf.Max(NoteConfig.LINE_LAYER_0, lastRightBlock.lineLayer - 1);
                } else if (rand > 90) {
                    leftBlock.lineLayer  = Mathf.Min(NoteConfig.LINE_LAYER_3, lastLeftBlock.lineLayer  + 1);
                    rightBlock.lineLayer = Mathf.Min(NoteConfig.LINE_LAYER_3, lastRightBlock.lineLayer + 1);
                } else
                {
                    leftBlock.lineLayer  = lastLeftBlock.lineLayer;
                    rightBlock.lineLayer = lastRightBlock.lineLayer;
                }

                rand = Random.Range(0, 100);
                if (rand > 92.5f)
                {
                    leftBlock.lineIndex = Mathf.Max(NoteConfig.LINE_INDEX_0, lastLeftBlock.lineIndex - 1);
                    rightBlock.lineIndex = Mathf.Max(NoteConfig.LINE_INDEX_0, lastRightBlock.lineIndex - 1);
                }
                else if (rand > 87.5f)
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

            if (leftBlock.obstacleLineIndex != -1 || rightBlock.obstacleLineIndex != -1)
            {
                leftBlock.lineIndex = _getLineIndexNextToObstacle(leftBlock);
                rightBlock.lineIndex = _getLineIndexNextToObstacle(rightBlock);
                leftBlock.cutDirection = NoteConfig.CUT_DIRECTION_NONE;
                rightBlock.cutDirection = NoteConfig.CUT_DIRECTION_NONE;
            }
        }
    }

    private void _polishSingleBlocks()
    {
        NoteConfig lastBlock = null;
        for (int i = 0; i < _beatMappingContainer.noteData.Count; i++)
        {
            NoteConfig block = _beatMappingContainer.noteData[i];
            if (lastBlock == null || lastBlock.belongsToDoubleNote) // Should not move note to a double note, can't hit 3 at the same time.
            {
                lastBlock = block;
                continue;
            }
            if (lastBlock.time + MIN_SINGLE_BLOCK_NOTE_TIME_INVERVAL > block.time)
            {
                block.time = lastBlock.time;
                block.obstacleLineIndex = lastBlock.obstacleLineIndex;
                block.type = lastBlock.type == NoteConfig.NOTE_TYPE_LEFT ? NoteConfig.NOTE_TYPE_RIGHT : NoteConfig.NOTE_TYPE_LEFT;

                if (block.obstacleLineIndex != -1)
                {
                    block.lineIndex = _getLineIndexNextToObstacle(block);
                    block.cutDirection = NoteConfig.CUT_DIRECTION_NONE;
                }
                if (block.lineIndex == lastBlock.lineIndex && block.lineLayer == lastBlock.lineLayer)
                {
                    _beatMappingContainer.noteData.RemoveAt(i);
                    i--;
                    continue;
                }
            }
            lastBlock = block;
        }

        lastBlock = null;
        foreach (NoteConfig block in _beatMappingContainer.noteData)
        {
            if (lastBlock == null) // Should be okay to check only one of them, for performance reasons.
            {
                lastBlock = block;
                continue;
            }

            if (lastBlock.time + SINGLE_BLOCK_CONNECTION_NOTE_TIME_INTERVAL > block.time)
            {
                block.cutDirection = _getOppositeCutDirection(lastBlock.cutDirection);

                int rand = Random.Range(0, 100);
                if (rand > 95)
                {
                    block.lineLayer = Mathf.Max(NoteConfig.LINE_LAYER_0, lastBlock.lineLayer - 1);
                }
                else if (rand > 90)
                {
                    block.lineLayer = Mathf.Min(NoteConfig.LINE_LAYER_3, lastBlock.lineLayer + 1);
                }
                else
                {
                    block.lineLayer = lastBlock.lineLayer;
                }

                rand = Random.Range(0, 100);
                if (rand > 92.5f)
                {
                    block.lineIndex = Mathf.Max(NoteConfig.LINE_INDEX_0, lastBlock.lineIndex - 1);
                }
                else if (rand > 87.5f)
                {
                    block.lineIndex = Mathf.Min(NoteConfig.LINE_INDEX_3, lastBlock.lineIndex + 1);
                }
                else
                {
                    block.lineIndex = lastBlock.lineIndex;
                }
            }
            if (block.obstacleLineIndex != -1)
            {
                block.lineIndex = _getLineIndexNextToObstacle(block);
                block.cutDirection = NoteConfig.CUT_DIRECTION_NONE;
            }
        }
    }

    private int _getLineIndexNextToObstacle(NoteConfig block)
    {
        
        if (block.type == NoteConfig.NOTE_TYPE_LEFT)
        {
            return block.obstacleLineIndex < 2 ? NoteConfig.LINE_INDEX_2 : NoteConfig.LINE_INDEX_0;
        }
        else
        {
            return block.obstacleLineIndex < 2 ? NoteConfig.LINE_INDEX_3 : NoteConfig.LINE_INDEX_1;
        }
    }

    private void _checkBlockDensity()
    {
        NoteConfig lastBlock = null;
        NoteConfig block;

        for (int i = 0; i < _beatMappingContainer.noteData.Count; i++)
        {
            block = _beatMappingContainer.noteData[i];
            if (lastBlock == null)
            {
                lastBlock = block;
                continue;
            }
            if (lastBlock.time + MIN_NOTE_DISTANCE > block.time && !(block.belongsToDoubleNote || lastBlock.belongsToDoubleNote))
            {
                _beatMappingContainer.noteData.RemoveAt(i);
                i--;
                continue;
            }
            lastBlock = block;
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
        if (rand > 95) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_8;  //  5 %
        if (rand > 90) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_7;  //  5 %
        if (rand > 85) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_6;  //  5 %
        if (rand > 80) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_5;  //  5 %
        if (rand > 65) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_4;  // 15 %
        if (rand > 50) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_3;  // 15 %
        if (rand > 25) return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_2;  // 25 %
        else return (int)DOUBLE_NOTE_LAYER_TYPES.LAYER_1;  // 25%
    }

    private int _getRandomIndexSeed()
    {
        int rand = Random.Range(0, 100);
        if (rand > 90) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_4;  // 10 %
        if (rand > 80) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_3;  // 10 %
        if (rand > 40) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_2;  // 40 %
        else           return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_1;  // 40 %
    }
}
