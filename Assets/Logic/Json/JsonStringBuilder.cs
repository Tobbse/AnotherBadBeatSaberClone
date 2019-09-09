using System.Globalization;
using BeatMappingConfigs;
using System.Collections.Generic;

/**
 * This namespace contains classes that create Json strings for Beat Mappings, Highscore Data and Info Files.
 **/

namespace JsonStringBuilders
{
    /**
    * Provides a Json string for an info file.
    **/
    public class JsonHighscoreStringBuilder
    {
        private List<HighscoreData> _highscoreData;
        private string _json;

        public void setData(List<HighscoreData> highscoreData)
        {
            _highscoreData = highscoreData;
        }

        public string getJsonString()
        {
            _json = "{\"_highscores\": [";
            string score = "\"_score\":";
            string rank = "\"_rank\":";

            HighscoreData highscore;
            bool isLast;
            string temp;

            for (int i = 0; i < _highscoreData.Count; i++)
            {
                highscore = _highscoreData[i];
                isLast = i >= _highscoreData.Count - 1;

                temp = score + highscore.Score.ToString() + ",";
                temp += rank + highscore.Rank.ToString();
                temp = _addBrackets(temp, isLast);
                _json += temp;
            }
            _json += "]}";
            return _json;
        }

        private string _addBrackets(string json, bool isLast = false)
        {
            json = "{" + json + "}";
            return isLast ? json : json + ",";
        }
    }


    /**
    * Provides a Json string for an info file.
    **/
    public class JsonMappingStringBuilder
    {
        private List<EventConfig> _events;
        private List<NoteConfig> _notes;
        private List<ObstacleConfig> _obstacles;
        private List<BookmarkConfig> _bookmarks;
        private string _json = "";
        private string _eventStr;
        private string _noteStr;
        private string _obstacleStr;
        private string _bookmarkStr;
        private CultureInfo usa = new CultureInfo("en-US");

        public void setData(MappingContainer beatMappingContainer)
        {
            _events = beatMappingContainer.EventData;
            _notes = beatMappingContainer.NoteData;
            _obstacles = beatMappingContainer.ObstacleData;
            _bookmarks = beatMappingContainer.BookmarkData;
        }

        public string getJsonString()
        {
            _addEventData();
            _addNoteData();
            _addObstacleData();
            _addBookmarkData();

            string start = "{\"_version\":\"" + JsonController.MAPPING_VERSION + "\",\"_BPMChanges\": [],";
            _json = start + _json + "}";

            return _json;
        }

        private void _addEventData()
        {
            _json += "\"_events\":[";

            string time = "\"_time\":";
            string type = "\"_type\":";
            string value = "\"_value\":";

            string eventStr;
            bool isLast;
            EventConfig eventCfg;

            for (int i = 0; i < _events.Count; i++)
            {
                isLast = i >= _events.Count - 1;
                eventCfg = _events[i];

                eventStr = time + eventCfg.Time.ToString(usa) + ",";
                eventStr += type + eventCfg.Type.ToString(usa) + ",";
                eventStr += value + eventCfg.Value.ToString(usa) + ",";

                eventStr = _addBrackets(eventStr, isLast);
                _json += eventStr;
            }
            _json += "]";
        }

        private void _addNoteData()
        {
            _json += ",\"_notes\":[";

            string time = "\"_time\":";
            string lineIndex = "\"_lineIndex\":";
            string lineLayer = "\"_lineLayer\":";
            string type = "\"_type\":";
            string cutDirection = "\"_cutDirection\":";

            string notesStr;
            bool isLast;
            NoteConfig noteCfg;

            for (int i = 0; i < _notes.Count; i++)
            {
                isLast = i >= _notes.Count - 1;
                noteCfg = _notes[i];

                notesStr = time + noteCfg.Time.ToString(usa) + ",";
                notesStr += lineIndex + noteCfg.LineIndex.ToString(usa) + ",";
                notesStr += lineLayer + noteCfg.LineLayer.ToString(usa) + ",";
                notesStr += type + noteCfg.Type.ToString(usa) + ",";
                notesStr += cutDirection + noteCfg.CutDirection.ToString(usa) + ",";

                notesStr = _addBrackets(notesStr, isLast);
                _json += notesStr;
            }
            _json += "]";
        }

        private void _addObstacleData()
        {
            _json += ",\"_obstacles\":[";

            string time = "\"_time\":";
            string lineIndex = "\"_lineIndex\":";
            string type = "\"_type\":";
            string duration = "\"_duration\":";
            string width = "\"_width\":";

            string obstacleStr;
            bool isLast;
            ObstacleConfig obstacleCfg;

            for (int i = 0; i < _obstacles.Count; i++)
            {
                isLast = i >= _obstacles.Count - 1;
                obstacleCfg = _obstacles[i];

                obstacleStr = time + obstacleCfg.Time.ToString(usa) + ",";
                obstacleStr += lineIndex + obstacleCfg.LineIndex.ToString(usa) + ",";
                obstacleStr += type + obstacleCfg.Type.ToString(usa) + ",";
                obstacleStr += duration + obstacleCfg.Duration.ToString(usa) + ",";
                obstacleStr += width + obstacleCfg.Width.ToString(usa) + ",";

                obstacleStr = _addBrackets(obstacleStr, isLast);
                _json += obstacleStr;
            }
            _json += "]";
        }

        private void _addBookmarkData()
        {
            _json += ",\"_bookmarks\":[";

            string time = "\"_time\":";
            string name = "\"_name\":";

            string bookmarkStr;
            bool isLast;
            BookmarkConfig bookmarkCfg;

            for (int i = 0; i < _bookmarks.Count; i++)
            {
                isLast = i >= _bookmarks.Count - 1;
                bookmarkCfg = _bookmarks[i];

                bookmarkStr = time + bookmarkCfg.Time.ToString(usa) + ",";
                bookmarkStr += name + bookmarkCfg.Name + ",";

                bookmarkStr = _addBrackets(bookmarkStr, isLast);
                _json += bookmarkStr;
            }
            _json += "]";
        }

        private string _addBrackets(string json, bool isLast = false)
        {
            json = "{" + json + "}";
            return isLast ? json : json + ",";
        }
    }


    /**
     * Provides a Json string for an info file.
     **/
    public static class JsonInfoStringBuilder
    {
        // TODO: Obviously, the parsing and writing the info file is not completely implemented yet.
        // The most important part is to get the BPM right as that will influence the tempo of the song.
        public static string getJsonString(string trackName, float bpm)
        {
            string start = "{" + "\"_version\":\"" + JsonController.MAPPING_VERSION + "\",";
            start += "\"_songName\":\"" + trackName + "\",";
            string rest = "\"_songSubName\":\"\",\"_songAuthorName\":\"UNKNOWN\",\"_levelAuthorName\":\"UNKNOWN\",\"_beatsPerMinute\":" +
                bpm.ToString(new CultureInfo("en-US")) +
                ",\"_songTimeOffset\":0,\"_shuffle\":0,\"_shufflePeriod\":0.0,\"_previewStartTime\":0.0,\"_previewDuration\":0.0,\"_songFilename\"" +
                ":\"UNKNOWN\",\"_coverImageFilename\":\"UNKNOWN\",\"_environmentName\":\"DefaultEnvironment\",\"_customData\":{\"_contributors\":" +
                "[],\"_customEnvironment\":\"\",\"_customEnvironmentHash\":\"\"},\"_difficultyBeatmapSets\":[{\"_beatmapCharacteristicName\":" +
                "\"Standard\",\"_difficultyBeatmaps\":[{\"_difficulty\":\"Normal\",\"_difficultyRank\":3,\"_beatmapFilename\":" +
                "\"Normal.dat\",\"_noteJumpMovementSpeed\":10.0,\"_noteJumpStartBeatOffset\":0,\"_customData\":{\"_difficultyLabel\":" +
                "\"\",\"_editorOffset\":0,\"_editorOldOffset\":0,\"_warnings\":[],\"_information\":[],\"_suggestions\":[],\"_requirements\":[]}}]}]}";
            return start + rest;
        }
    }

}