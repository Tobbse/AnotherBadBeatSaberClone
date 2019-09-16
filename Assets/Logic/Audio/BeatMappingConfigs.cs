using System.Collections.Generic;

namespace Audio
{
    /**
     * This namespace contains configs for beat mappings, including events, obstacles and notes.
     **/
    namespace BeatMappingConfigs
    {
        /// <summary>
        /// Container for all beat mapping information.
        /// This contains lists of event configs, obstacle configs, note configs etc. - Basically all relevant
        /// informatio needed to display a beat mapping in the game.
        ///
        /// The mapping have to be sorted, because the audio data is analyzed in different bands, so the timing
        /// of the created configs is not nessecarily consecutive.
        /// The configs are sorted based on the timing values during the song.
        /// </summary>
        public class MappingContainer
        {
            public List<EventConfig> EventData { get; set; } = new List<EventConfig>();
            public List<NoteConfig> NoteData { get; set; } = new List<NoteConfig>();
            public List<ObstacleConfig> ObstacleData { get; set; } = new List<ObstacleConfig>();
            public List<BookmarkConfig> BookmarkData { get; set; } = new List<BookmarkConfig>();
            public MappingInfo MappingInfo { get; set; }

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
            public enum TYPES
            {
                BLINK_LASERS = 0, ROTATE_LIGHTS_1 = 1, ROTATE_LIGHTS_2 = 2, ROTATE_LIGHTS_3 = 3,
                BLINK_LASERS_1 = 4, BLINK_LASERS_2 = 5, CHANGE_SPINNER_DIRECTION = 6, CHANGE_SPINNER_SPEED = 7
            }

            public float Time { get; set; }
            public int Type { get; set; }
            public int Value { get; set; }
        }


        /**
         * Contains information about a note, like its cut direction and positioning.
         **/
        public class NoteConfig
        {
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

            public float Time { get; set; }
            public int LineIndex { get; set; }
            public int LineLayer { get; set; }
            public int Type { get; set; }
            public int CutDirection { get; set; }
            public bool BelongsToDoubleNote { get; set; }
            public int ObstacleLineIndex { get; set; } = -1;
        }


        /**
         * Contains information about obstacles, like their width and duration. The duration has to be translated
         * to world coordinates later by using the obstacle speed and the bpm.
         **/
        public class ObstacleConfig
        {
            public float Time { get; set; }
            public int LineIndex { get; set; }
            public int Type { get; set; }
            public float Duration { get; set; }
            public float Width { get; set; }
        }


        /**
         * Contains bookmark information. This is not currently supported.
         **/
        public class BookmarkConfig
        {
            public float Time { get; set; }
            public string Name { get; set; }
        }


        /**
         * Contains some general info about the mapping, like the bpm. This is extracted from the 'info.dat' files. 
         **/
        public class MappingInfo
        {
            // TODO low priority, but this could be extended massively (trackname, artist, bpm change, etc.).
            // The info files can contain a lot of data, not all of it is needed.
            public float Bpm { get; set; }
        }

    }

}
