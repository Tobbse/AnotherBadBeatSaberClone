using System.Collections.Generic;

/**
 * This namespace contains configs for beat mappings, including events, obstacles and notes.
 **/
// TODO USE ENUMS HERE!!!
namespace BeatMappingConfigs {

    /**
     * Container for all beat mapping information.
     * This contains lists of event configs, obstacle configs, note configs etc. - Basically all relevant
     * informatio needed to display a beat mapping in the game.
     * 
     * The mapping have to be sorted, because the audio data is analyzed in different bands, so the timing
     * of the created configs is not nessecarily consecutive.
     * The configs are sorted based on the timing values during the song.
     **/
    public class MappingContainer
    {
        private List<EventConfig> _eventData = new List<EventConfig>();
        private List<NoteConfig> _noteData = new List<NoteConfig>();
        private List<ObstacleConfig> _obstacleData = new List<ObstacleConfig>();
        private List<BookmarkConfig> _bookmarkData = new List<BookmarkConfig>();
        private MappingInfo _mappingInfo;

        public List<EventConfig> EventData { get { return _eventData; } set { _eventData = value; } }
        public List<NoteConfig> NoteData { get { return _noteData; } set { _noteData = value; } }
        public List<ObstacleConfig> ObstacleData { get { return _obstacleData; } set { _obstacleData = value; } }
        public List<BookmarkConfig> BookmarkData { get { return _bookmarkData; } set { _bookmarkData = value; } }
        public MappingInfo MappingInfo { get { return _mappingInfo; } set { _mappingInfo = value; } }

        public void sortMappings()
        {
            EventData.Sort((System.Comparison<EventConfig>)delegate (EventConfig obj1, EventConfig obj2) { return obj1.Time.CompareTo((float)obj2.Time); });
            NoteData.Sort((System.Comparison<NoteConfig>)delegate (NoteConfig obj1, NoteConfig obj2) { return obj1.Time.CompareTo((float)obj2.Time); });
            ObstacleData.Sort((System.Comparison<ObstacleConfig>)delegate (ObstacleConfig obj1, ObstacleConfig obj2) { return obj1.Time.CompareTo((float)obj2.Time); });
            BookmarkData.Sort(delegate (BookmarkConfig obj1, BookmarkConfig obj2) { return obj1.Time.CompareTo(obj2.Time); });
        }
    }


    /**
     * Contains information about an event that triggers lights, fog and movements to change or be displayed.
     **/
    public class EventConfig
    {
        public enum TYPES { BLINK_LASERS = 0, ROTATE_LIGHTS_1 = 1, ROTATE_LIGHTS_2 = 2, ROTATE_LIGHTS_3 = 3,
            BLINK_LASERS_1 = 4, BLINK_LASERS_2 = 5, CHANGE_SPINNER_DIRECTION = 6, CHANGE_SPINNER_SPEED = 7 }

        private float _time;
        private int _type;
        private int _value;

        public float Time { get { return _time; } set { _time = value; } }
        public int Type { get { return _type; } set { _type = value; } }
        public int Value { get { return _value; } set { _value = value; } }
    }


    /**
     * Contains information about a note, like its cut direction and positioning.
     **/
    public class NoteConfig {
        public const int CUT_DIRECTION_180 = 0;
        public const int CUT_DIRECTION_0 = 1;
        public const int CUT_DIRECTION_90 = 2;
        public const int CUT_DIRECTION_270 = 3;

        public const int CUT_DIRECTION_135 = 4;
        public const int CUT_DIRECTION_225 = 5;  
        public const int CUT_DIRECTION_45 = 6;
        public const int CUT_DIRECTION_315 = 7;

        public const int CUT_DIRECTION_NONE = 8;

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

        private float _time;
        private int _lineIndex;
        private int _lineLayer;
        private int _type;
        private int _cutDirection;
        private bool _belongsToDoubleNote;
        private int _obstacleLineIndex = -1;

        public float Time { get { return _time; } set { _time = value; } }
        public int LineIndex { get { return _lineIndex; } set { _lineIndex = value; } }
        public int LineLayer { get { return _lineLayer; } set { _lineLayer = value; } }
        public int Type { get { return _type; } set { _type = value; } }
        public int CutDirection { get { return _cutDirection; } set { _cutDirection = value; } }
        public bool BelongsToDoubleNote { get { return _belongsToDoubleNote; } set { _belongsToDoubleNote = value; } }
        public int ObstacleLineIndex { get { return _obstacleLineIndex; } set { _obstacleLineIndex = value; } }
    }


    /**
     * Contains information about obstacles, like their width and duration. The duration has to be translated
     * to world coordinates later by using the obstacle speed and the bpm.
     **/
    public class ObstacleConfig
    {
        private float _time;
        private int _lineIndex;
        private int _type;
        private float _duration;
        private float _width;

        public float Time { get { return _time; } set { _time = value; } }
        public int LineIndex { get { return _lineIndex; } set { _lineIndex = value; } }
        public int Type { get { return _type; } set { _type = value; } }
        public float Duration { get { return _duration; } set { _duration = value; } }
        public float Width { get { return _width; } set { _width = value; } }
    }


    /**
     * Contains bookmark information. This is not currently supported.
     **/
    public class BookmarkConfig
    {
        private float _time;
        private string _name;

        public float Time { get { return _time; } set { _time = value; } }
        public string Name { get { return _name; } set { _name = value; } }
    }


    /**
     * Contains some general info about the mapping, like the bpm. This is extracted from the 'info.dat' files. 
     **/
    public class MappingInfo
    {
        // TODO low priority, but this could be extended massively (trackname, artist, bpm change, etc.).
        // The info files can contain a lot of data, not all of it is needed.
        private float _bpm;

        public float Bpm { get { return _bpm; } set { _bpm = value; } }
    }

}
