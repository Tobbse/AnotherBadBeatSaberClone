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

        _verticalMapping[NoteConfig.LINE_LAYER_0] = 0.0f;
        _verticalMapping[NoteConfig.LINE_LAYER_1] = 0.5f;
        _verticalMapping[NoteConfig.LINE_LAYER_2] = 1.0f;
        _verticalMapping[NoteConfig.LINE_LAYER_3] = 1.5f;

        _horizontalMapping[NoteConfig.LINE_INDEX_0] = -0.75f;
        _horizontalMapping[NoteConfig.LINE_INDEX_1] = -0.25f;
        _horizontalMapping[NoteConfig.LINE_INDEX_2] = 0.25f;
        _horizontalMapping[NoteConfig.LINE_INDEX_3] = 0.75f;
    }
}