using Audio.BeatMappingConfigs;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AudioAnalysis
{
    /// <summary>
    /// After the initial analysis of the audio data and the creation of the note data, we iterate over those created blocks again.
    /// Some rule based behavior is used to create double notes and influence the positioning of the blocks, according to variables
    /// like their cut direction or obstacles next to them.
    /// In this process, also the timing of notes is changed and notes are deleted if nessecary.
    /// </summary>
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
        private System.Random _rand;

        public PostOnsetAudioAnalyzer(MappingContainer beatMappingContainer)
        {
            _beatMappingContainer = beatMappingContainer;
            _rand = new System.Random();
        }

        public void analyze()
        {
            _createDoubleBlocks();
            _polishDoubleBlocks();
            _finalPolish();
            _checkBlockDensity();
        }

        public MappingContainer getBeatMappingContainer()
        {
            return _beatMappingContainer;
        }

        // This will take all blocks of the same type (right, left) that are too close to each other
        // and create double blocks from them (two blocks next to each other).
        private void _createDoubleBlocks()
        {
            NoteConfig lastNote = null;
            NoteConfig note;
            List<NoteConfig> noteData = _beatMappingContainer.NoteData;

            for (int i = 0; i < noteData.Count; i++)
            {
                note = noteData[i];

                if (lastNote == null)
                {
                    lastNote = note;
                    continue;
                }

                float distance = Math.Abs(note.Time - lastNote.Time);

                if (distance < MIN_DOUBLE_BLOCK_NOTE_TIME_INTERVAL)
                {
                    _doubleNoteIndices.Add(new int[] { i - 1, i });

                    NoteConfig[] newNotes = _getDoubleNotes(lastNote.Time, lastNote.ObstacleLineIndex);
                    noteData[i - 1] = newNotes[0];
                    noteData[i] = newNotes[1];

                    i += 1; // Don't use the same note again.
                }
                if (i >= noteData.Count) break;
                lastNote = noteData[i];
            }
        }

        // Create a semi random double block at a given position. The cut direction depends on the position of
        // the blocks, because with certain positions some cut directions are not really possible.
        private NoteConfig[] _getDoubleNotes(float time, int obstacleLineIndex)
        {
            int layerSeed = _getRandomLayerSeed();
            int indexSeed = _getRandomIndexSeed();
            int leftLine = _getLine(indexSeed, true);
            int rightLine = _getLine(indexSeed, false);
            int leftLayer = _getLayer(layerSeed, true);
            int rightLayer = _getLayer(layerSeed, false);

            int blockLineDistance = Math.Abs(leftLine - rightLine);
            int blockLayerDistance = Math.Abs(leftLayer - rightLayer);

            int[] cutDirections = _getRandomCutDirection(blockLineDistance, blockLayerDistance);

            NoteConfig leftDoubleNoteCfg = new NoteConfig()
            {
                BelongsToDoubleNote = true,
                Type = NoteConfig.NOTE_TYPE_LEFT,
                Time = time,
                LineLayer = leftLayer,
                LineIndex = leftLine,
                CutDirection = cutDirections[0],
                ObstacleLineIndex = obstacleLineIndex,
            };

            NoteConfig rightDoubleNoteCfg = new NoteConfig()
            {
                BelongsToDoubleNote = true,
                Type = NoteConfig.NOTE_TYPE_RIGHT,
                Time = time,
                LineLayer = rightLayer,
                LineIndex = rightLine,
                CutDirection = cutDirections[1],
                ObstacleLineIndex = obstacleLineIndex,
            };

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

        // Returns a semi random cut direction, depending on some input positions.
        // Certain cut directions are not possible with some doubel block positioning combinations.
        private int[] _getRandomCutDirection(int lineDistance, int layerDistance)
        {
            int rand = _rand.Next(0, 100);
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
                else randomCutDirection = NoteConfig.CUT_DIRECTION_180;  // 50.0 % Bottom
            }
            else if (!shouldUseDiagonal)
            {
                if (rand > 80f) randomCutDirection = NoteConfig.CUT_DIRECTION_270;  // 20.0 % Left
                else if (rand > 60f) randomCutDirection = NoteConfig.CUT_DIRECTION_90;   // 20.0 % Right
                else if (rand > 30f) randomCutDirection = NoteConfig.CUT_DIRECTION_0;    // 30.0 % Top
                else randomCutDirection = NoteConfig.CUT_DIRECTION_180;  // 30.0 % Bottom
            }
            else if (!shouldUseHorizontal)
            {
                if (rand > 92.5f) randomCutDirection = NoteConfig.CUT_DIRECTION_315;  //  7.5 % TopLeft
                else if (rand > 85f) randomCutDirection = NoteConfig.CUT_DIRECTION_45;   //  7.5 % TopRight
                else if (rand > 77.5f) randomCutDirection = NoteConfig.CUT_DIRECTION_225;  //  7.5 % BottomLeft
                else if (rand > 70f) randomCutDirection = NoteConfig.CUT_DIRECTION_135;  //  7.5 % BottomRight
                else if (rand > 35f) randomCutDirection = NoteConfig.CUT_DIRECTION_0;    // 35.0 % Top
                else randomCutDirection = NoteConfig.CUT_DIRECTION_180;  // 35.0 % Bottom
            }
            else
            {
                if (rand > 95f) randomCutDirection = NoteConfig.CUT_DIRECTION_315;  //  5.0 % TopLeft
                else if (rand > 90f) randomCutDirection = NoteConfig.CUT_DIRECTION_45;   //  5.0 % TopRight
                else if (rand > 85f) randomCutDirection = NoteConfig.CUT_DIRECTION_225;  //  5.0 % BottomLeft
                else if (rand > 80f) randomCutDirection = NoteConfig.CUT_DIRECTION_135;  //  5.0 % BottomRight
                else if (rand > 65f) randomCutDirection = NoteConfig.CUT_DIRECTION_270;  // 15.0 % Left
                else if (rand > 50f) randomCutDirection = NoteConfig.CUT_DIRECTION_90;   // 15.0 % Right
                else if (rand > 25f) randomCutDirection = NoteConfig.CUT_DIRECTION_0;    // 25.0 % Top
                else randomCutDirection = NoteConfig.CUT_DIRECTION_180;  // 25.0 % Bottom
            }

            int[] cutDirections = new int[] { randomCutDirection, randomCutDirection };
            if (_rand.Next(0, 100) > 90 || shouldInvert)
            {
                cutDirections[1] = _getOppositeCutDirection(cutDirections[0]);
            }
            return cutDirections;
        }

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

        // Iterates over the created double blocks and does some polishing. Creates some variance (random combinations) .
        private void _polishDoubleBlocks()
        {
            List<NoteConfig> noteData = _beatMappingContainer.NoteData;

            NoteConfig lastLeftBlock = null;
            NoteConfig lastRightBlock = null;

            foreach (int[] doubleBlockIndices in _doubleNoteIndices)
            {
                NoteConfig leftBlock = noteData[doubleBlockIndices[0]];
                NoteConfig rightBlock = noteData[doubleBlockIndices[1]];
                if (lastLeftBlock == null) // It's okay to check only one of them, for performance reasons.
                {
                    lastLeftBlock = leftBlock;
                    lastRightBlock = rightBlock;
                    continue;
                }

                if (lastLeftBlock.Time + DOUBLE_BLOCK_CONNECTION_NOTE_TIME_INTERVAL > leftBlock.Time)
                {
                    leftBlock.CutDirection = _getOppositeCutDirection(lastLeftBlock.CutDirection);
                    rightBlock.CutDirection = _getOppositeCutDirection(lastRightBlock.CutDirection);

                    int rand = _rand.Next(0, 100);
                    if (rand > 95)
                    {
                        leftBlock.LineLayer = Math.Max(NoteConfig.LINE_LAYER_0, lastLeftBlock.LineLayer - 1);
                        rightBlock.LineLayer = Math.Max(NoteConfig.LINE_LAYER_0, lastRightBlock.LineLayer - 1);
                    }
                    else if (rand > 90)
                    {
                        leftBlock.LineLayer = Math.Min(NoteConfig.LINE_LAYER_3, lastLeftBlock.LineLayer + 1);
                        rightBlock.LineLayer = Math.Min(NoteConfig.LINE_LAYER_3, lastRightBlock.LineLayer + 1);
                    }
                    else
                    {
                        leftBlock.LineLayer = lastLeftBlock.LineLayer;
                        rightBlock.LineLayer = lastRightBlock.LineLayer;
                    }

                    rand = _rand.Next(0, 100);
                    if (rand > 92.5f)
                    {
                        leftBlock.LineIndex = Math.Max(NoteConfig.LINE_INDEX_0, lastLeftBlock.LineIndex - 1);
                        rightBlock.LineIndex = Math.Max(NoteConfig.LINE_INDEX_0, lastRightBlock.LineIndex - 1);
                    }
                    else if (rand > 87.5f)
                    {
                        leftBlock.LineIndex = Math.Min(NoteConfig.LINE_INDEX_3, lastLeftBlock.LineIndex + 1);
                        rightBlock.LineIndex = Math.Min(NoteConfig.LINE_INDEX_3, lastRightBlock.LineIndex + 1);
                    }
                    else
                    {
                        leftBlock.LineIndex = lastLeftBlock.LineIndex;
                        rightBlock.LineIndex = lastRightBlock.LineIndex;
                    }
                }

                if (leftBlock.ObstacleLineIndex != -1 || rightBlock.ObstacleLineIndex != -1)
                {
                    leftBlock.LineIndex = _getLineIndexNextToObstacle(leftBlock);
                    rightBlock.LineIndex = _getLineIndexNextToObstacle(rightBlock);
                    leftBlock.CutDirection = NoteConfig.CUT_DIRECTION_NONE;
                    rightBlock.CutDirection = NoteConfig.CUT_DIRECTION_NONE;
                }
            }
        }

        // TODO some parts of those 2 loops seem unnessecary. Check what is really needed here.
        // Iterates over all blocks and makes sure for example that they are positioned properly next to obstacles.
        private void _finalPolish()
        {
            NoteConfig lastBlock = null;
            for (int i = 0; i < _beatMappingContainer.NoteData.Count; i++)
            {
                NoteConfig block = _beatMappingContainer.NoteData[i];
                if (lastBlock == null || lastBlock.BelongsToDoubleNote) // Should not move note to a double note, can't hit 3 at the same time.
                {
                    lastBlock = block;
                    continue;
                }
                if (lastBlock.Time + MIN_SINGLE_BLOCK_NOTE_TIME_INVERVAL > block.Time)
                {
                    block.Time = lastBlock.Time;
                    block.ObstacleLineIndex = lastBlock.ObstacleLineIndex;
                    block.Type = lastBlock.Type == NoteConfig.NOTE_TYPE_LEFT ? NoteConfig.NOTE_TYPE_RIGHT : NoteConfig.NOTE_TYPE_LEFT;

                    if (block.ObstacleLineIndex != -1)
                    {
                        block.LineIndex = _getLineIndexNextToObstacle(block);
                        block.CutDirection = NoteConfig.CUT_DIRECTION_NONE;
                    }
                    if (block.LineIndex == lastBlock.LineIndex && block.LineLayer == lastBlock.LineLayer)
                    {
                        _beatMappingContainer.NoteData.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
                lastBlock = block;
            }

            lastBlock = null;
            foreach (NoteConfig block in _beatMappingContainer.NoteData)
            {
                if (lastBlock == null) // Should be okay to check only one of them, for performance reasons.
                {
                    lastBlock = block;
                    continue;
                }

                if (lastBlock.Time + SINGLE_BLOCK_CONNECTION_NOTE_TIME_INTERVAL > block.Time)
                {
                    block.CutDirection = _getOppositeCutDirection(lastBlock.CutDirection);

                    int rand = _rand.Next(0, 100);
                    if (rand > 95)
                    {
                        block.LineLayer = Math.Max(NoteConfig.LINE_LAYER_0, lastBlock.LineLayer - 1);
                    }
                    else if (rand > 90)
                    {
                        block.LineLayer = Math.Min(NoteConfig.LINE_LAYER_3, lastBlock.LineLayer + 1);
                    }
                    else
                    {
                        block.LineLayer = lastBlock.LineLayer;
                    }

                    rand = _rand.Next(0, 100);
                    if (rand > 92.5f)
                    {
                        block.LineIndex = Math.Max(NoteConfig.LINE_INDEX_0, lastBlock.LineIndex - 1);
                    }
                    else if (rand > 87.5f)
                    {
                        block.LineIndex = Math.Min(NoteConfig.LINE_INDEX_3, lastBlock.LineIndex + 1);
                    }
                    else
                    {
                        block.LineIndex = lastBlock.LineIndex;
                    }
                }
                if (block.ObstacleLineIndex != -1)
                {
                    block.LineIndex = _getLineIndexNextToObstacle(block);
                    block.CutDirection = NoteConfig.CUT_DIRECTION_NONE;
                }
            }
        }

        private int _getLineIndexNextToObstacle(NoteConfig block)
        {

            if (block.Type == NoteConfig.NOTE_TYPE_LEFT)
            {
                return block.ObstacleLineIndex < 2 ? NoteConfig.LINE_INDEX_2 : NoteConfig.LINE_INDEX_0;
            }
            else
            {
                return block.ObstacleLineIndex < 2 ? NoteConfig.LINE_INDEX_3 : NoteConfig.LINE_INDEX_1;
            }
        }

        // After polishing all blocks, checks again if there are some places with a block density that is too high,
        // but not looking at the type of the block at all. Removes blocks in places where there are too many.
        private void _checkBlockDensity()
        {
            NoteConfig lastBlock = null;
            NoteConfig block;

            for (int i = 0; i < _beatMappingContainer.NoteData.Count; i++)
            {
                block = _beatMappingContainer.NoteData[i];
                if (lastBlock == null)
                {
                    lastBlock = block;
                    continue;
                }
                if (lastBlock.Time + MIN_NOTE_DISTANCE > block.Time && !(block.BelongsToDoubleNote || lastBlock.BelongsToDoubleNote))
                {
                    _beatMappingContainer.NoteData.RemoveAt(i);
                    i--;
                    continue;
                }
                lastBlock = block;
            }
        }

        private int _getOtherNoteType(int type)
        {
            return type == NoteConfig.NOTE_TYPE_LEFT ? NoteConfig.NOTE_TYPE_RIGHT : NoteConfig.NOTE_TYPE_LEFT;
        }

        private int _getRandomLayerSeed()
        {
            int rand = _rand.Next(0, 100);
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
            int rand = _rand.Next(0, 100);
            if (rand > 90) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_4;  // 10 %
            if (rand > 80) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_3;  // 10 %
            if (rand > 40) return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_2;  // 40 %
            else return (int)DOUBLE_NOTE_INDEX_TYPES.INDEX_1;  // 40 %
        }
    }

}
