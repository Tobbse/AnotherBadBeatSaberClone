using System.Collections.Generic;
using BeatMappingConfigs;

public static class ObjectSpawnPositionProvider
{
    private static Dictionary<int, float> _horizontalMapping;
    private static Dictionary<int, float> _verticalMapping;

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

    private static void _setupMappings()
    {
        _horizontalMapping = new Dictionary<int, float>();
        _verticalMapping = new Dictionary<int, float>();

        float step = 0.45f;
        float baseY = 0.15f;
        _verticalMapping[NoteConfig.LINE_LAYER_0] = baseY;
        _verticalMapping[NoteConfig.LINE_LAYER_1] = baseY + step;
        _verticalMapping[NoteConfig.LINE_LAYER_2] = baseY + step * 2;
        _verticalMapping[NoteConfig.LINE_LAYER_3] = baseY + step * 3;

        float baseX = 0;
        _horizontalMapping[NoteConfig.LINE_INDEX_0] = baseX - step * 1.5f;
        _horizontalMapping[NoteConfig.LINE_INDEX_1] = baseX - step / 2;
        _horizontalMapping[NoteConfig.LINE_INDEX_2] = baseX + step / 2;
        _horizontalMapping[NoteConfig.LINE_INDEX_3] = baseX + step * 1.5f;
    }
}