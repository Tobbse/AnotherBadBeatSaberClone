using System.Collections.Generic;

namespace BeatMappingConfigs {

    public class MappingContainer
    {
        public List<EventConfig> eventData = new List<EventConfig>();
        public List<NoteConfig> noteData = new List<NoteConfig>();
        public List<ObstacleConfig> obstacleData = new List<ObstacleConfig>();
        public List<BookmarkConfig> bookmarkData = new List<BookmarkConfig>();
        public MappingInfo mappingInfo;

        public void sortMappings()
        {
            eventData.Sort(delegate (EventConfig obj1, EventConfig obj2) { return obj1.time.CompareTo(obj2.time); });
            noteData.Sort(delegate (NoteConfig obj1, NoteConfig obj2) { return obj1.time.CompareTo(obj2.time); });
            obstacleData.Sort(delegate (ObstacleConfig obj1, ObstacleConfig obj2) { return obj1.time.CompareTo(obj2.time); });
            bookmarkData.Sort(delegate (BookmarkConfig obj1, BookmarkConfig obj2) { return obj1.time.CompareTo(obj2.time); });
        }
    }


    public class EventConfig
    {
        // TODO actually find out what the correct types are here and use it.
        public enum TYPES { BLINK_LASERS = 0, ROTATE_LIGHTS_1 = 1, ROTATE_LIGHTS_2 = 2, ROTATE_LIGHTS_3 = 3,
            BLINK_LASERS_1 = 4, BLINK_LASERS_2 = 5, CHANGE_SPINNER_DIRECTION = 6, CHANGE_SPINNER_SPEED = 7 }

        public float time;
        public int type;
        public int value;
    }


    public class NoteConfig {
        public const int CUT_DIRECTION_180 = 0; // Correct
        public const int CUT_DIRECTION_0 = 1; // Correct
        public const int CUT_DIRECTION_90 = 2; // Unknown
        public const int CUT_DIRECTION_270 = 3; // Correct

        public const int CUT_DIRECTION_135 = 4; // CORRECT
        public const int CUT_DIRECTION_225 = 5; // CORRECT        
        public const int CUT_DIRECTION_45 = 6; // Unknown
        public const int CUT_DIRECTION_315 = 7; // Unknown

        public const int CUT_DIRECTION_NONE = 8; // Unknown

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
        public bool belongsToDoubleNote;
        public int obstacleLineIndex = -1;
    }


    public class ObstacleConfig
    {
        public float time;
        public int lineIndex;
        public int type;
        public float duration;
        public float width;
    }


    public class BookmarkConfig
    {
        public float time;
        public string name;
    }

    // TODO if enough time left: add trackname, artist etc.
    public class MappingInfo
    {
        public float bpm;
    }

}
