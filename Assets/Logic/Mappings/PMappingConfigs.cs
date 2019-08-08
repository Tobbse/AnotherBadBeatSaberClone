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
