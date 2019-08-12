using System.IO;
using Newtonsoft.Json.Linq;
using MappingConfigs;

namespace JsonIOHandler {

    public static class PMappingJsonFileWriter
    {
        public static void writeFile(string json, string folderPath, string fileName)
        {
            string fullPath = folderPath + fileName;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (!File.Exists(fullPath))
            {
                FileStream stream = File.Create(fullPath);
                stream.Close();
            }
            File.WriteAllText(fullPath, string.Empty);

            StreamWriter writer = new StreamWriter(fullPath, true);
            writer.WriteLine(json);
            writer.Close();
        }
    }

    public static class PMappingJsonFileReader
    {
        public static MappingContainer readMappingFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string text = reader.ReadToEnd();
            reader.Close();

            JObject obj = JObject.Parse(text);

            FastList<EventConfig> eventData = _getEventData(obj["_events"]);
            FastList<NoteConfig> noteData = _getNoteData(obj["_notes"]);
            FastList<ObstacleConfig> obstacleData = _getObstacleData(obj["_obstacles"]);
            FastList<BookmarkConfig> bookmarkData = _getBookmarkData(obj["_bookmarks"]);

            MappingContainer container = new MappingContainer();
            container.eventData = eventData;
            container.noteData = noteData;
            container.obstacleData = obstacleData;
            container.bookmarkData = bookmarkData;

            return container;
        }

        private static FastList<EventConfig> _getEventData(JToken eventToken)
        {
            FastList<EventConfig> eventData = new FastList<EventConfig>(); 
            foreach (JToken child in eventToken.Children())
            {
                EventConfig eventConfig = new EventConfig();
                eventConfig.time = child["_time"].Value<float>();
                eventConfig.type = child["_type"].Value<int>();
                eventConfig.value = child["_value"].Value<int>();
                eventData.Add(eventConfig);
            }
            return eventData;
        }

        private static FastList<NoteConfig> _getNoteData(JToken noteToken)
        {
            FastList<NoteConfig> noteData = new FastList<NoteConfig>();
            foreach (JToken child in noteToken.Children())
            {
                NoteConfig noteConfig = new NoteConfig();
                noteConfig.time = child["_time"].Value<float>();
                noteConfig.lineIndex = child["_lineIndex"].Value<int>();
                noteConfig.lineLayer = child["_lineLayer"].Value<int>();
                noteConfig.type = child["_type"].Value<int>();
                noteConfig.cutDirection = child["_cutDirection"].Value<int>();
                noteData.Add(noteConfig);
            }
            return noteData;
        }

        private static FastList<ObstacleConfig> _getObstacleData(JToken obstacleToken)
        {
            FastList<ObstacleConfig> obstacleData = new FastList<ObstacleConfig>();
            foreach (JToken child in obstacleToken.Children())
            {
                ObstacleConfig obstacleConfig = new ObstacleConfig();
                obstacleConfig.time = child["_time"].Value<float>();
                obstacleConfig.lineIndex = child["_lineIndex"].Value<int>();
                obstacleConfig.type = child["_type"].Value<int>();
                obstacleConfig.duration = child["_duration"].Value<int>();
                obstacleConfig.width = child["_width"].Value<float>();
                obstacleData.Add(obstacleConfig);
            }
            return obstacleData;
        }

        private static FastList<BookmarkConfig> _getBookmarkData(JToken bookmarkToken)
        {
            FastList<BookmarkConfig> bookmarkData = new FastList<BookmarkConfig>();
            foreach (JToken child in bookmarkToken.Children())
            {
                BookmarkConfig newConfig = new BookmarkConfig();
                newConfig.time = child["_time"].Value<float>();
                newConfig.name = child["_name"].Value<string>();
                bookmarkData.Add(newConfig);
            }
            return bookmarkData;
        }
    } 

}