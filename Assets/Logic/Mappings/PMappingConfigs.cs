namespace PMappingConfigs {

    public class PMappingContainer
    {
        public FastList<PEventConfig> eventData = new FastList<PEventConfig>();
        public FastList<PNoteConfig> noteData = new FastList<PNoteConfig>();
        public FastList<PObstacleConfig> obstacleData = new FastList<PObstacleConfig>();
        public FastList<PBookmarkConfig> bookmarkData = new FastList<PBookmarkConfig>();
    }


    public class PEventConfig
    {
        public float time;
        public int type;
        public int value;
    }


    public class PNoteConfig {
        public const int CUT_DIRECTION_TOP = 0;
        public const int CUT_DIRECTION_RIGHT = 1;
        public const int CUT_DIRECTION_BOTTOM = 2;
        public const int CUT_DIRECTION_LEFT = 3;
        public const int LINE_INDEX_0 = 0;
        public const int LINE_INDEX_1 = 1;
        public const int LINE_INDEX_2 = 2;
        public const int LINE_INDEX_3 = 3;
        public const int LINE_LAYER_0 = 0;
        public const int LINE_LAYER_1 = 1;
        public const int LINE_LAYER_2 = 2;
        public const int LINE_LAYER_3 = 3;
        public const int NOTE_TYPE_LEFT = 0;
        public const int NOTE_TYPE_RIGHT = 1;

        public float time;
        public int lineIndex;
        public int lineLayer;
        public int type;
        public int cutDirection;
    }


    public class PObstacleConfig
    {
        public float time;
        public float lineIndex;
        public float type;
        public float duration;
        public float width;
    }


    public class PBookmarkConfig
    {
        public float time;
        public string name;
    }

}
