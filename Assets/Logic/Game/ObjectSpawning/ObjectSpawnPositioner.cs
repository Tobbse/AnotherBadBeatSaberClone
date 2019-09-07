using System.Collections.Generic;
using BeatMappingConfigs;

/**
 * Static class containing mappings for the line layers, line inidces and cut directions
 * for the positioning of spawned objects.
 **/
public static class ObjectSpawnPositionProvider
{
    private static Dictionary<int, float> _horizontalMapping;
    private static Dictionary<int, float> _verticalMapping;
    private static Dictionary<int, int> _cutDirectionMapping = new Dictionary<int, int>();

    public static float getHorizontalPosition(int lineIndex)
    {
        if (_horizontalMapping == null)
        {
            _setupMappings();
        }
        return _horizontalMapping[lineIndex];
    }

    public static float getVerticalPosition(int lineLayer)
    {
        if (_verticalMapping == null)
        {
            _setupMappings();
        }
        return _verticalMapping[lineLayer];
    }

    public static int getCutDirection(int cutDirection)
    {
        if (_cutDirectionMapping == null)
        {
            _setupMappings();
        }
        return _cutDirectionMapping[cutDirection];
    }

    private static void _setupMappings()
    {
        _horizontalMapping = new Dictionary<int, float>();
        _verticalMapping = new Dictionary<int, float>();

        float step = 0.58f;
        float baseY = 0.1f;
        _verticalMapping[NoteConfig.LINE_LAYER_0] = baseY;
        _verticalMapping[NoteConfig.LINE_LAYER_1] = baseY + step;
        _verticalMapping[NoteConfig.LINE_LAYER_2] = baseY + step * 2;
        _verticalMapping[NoteConfig.LINE_LAYER_3] = baseY + step * 3;

        float baseX = 0;
        _horizontalMapping[NoteConfig.LINE_INDEX_0] = baseX - step * 1.5f;
        _horizontalMapping[NoteConfig.LINE_INDEX_1] = baseX - step / 2;
        _horizontalMapping[NoteConfig.LINE_INDEX_2] = baseX + step / 2;
        _horizontalMapping[NoteConfig.LINE_INDEX_3] = baseX + step * 1.5f;

        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_0] = 0;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_90] = 90;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_180] = 180;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_270] = 270;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_45] = 45;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_135] = 135;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_225] = 225;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_315] = 315;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_NONE] = -1;
    }
}