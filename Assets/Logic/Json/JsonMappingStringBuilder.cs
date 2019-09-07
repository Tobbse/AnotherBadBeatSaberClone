using BeatMappingConfigs;
using System.Globalization;
using System.Collections.Generic;

/**
 * Builds a json string from a mapping container so that it can be written to a json file.
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

        for (int i = 0; i < _events.Count; i++) {
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