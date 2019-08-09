using System.IO;
using Newtonsoft.Json.Linq;
using PMappingConfigs;

namespace PJsonIOHandler {

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
        public static PMappingContainer readMappingFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string text = reader.ReadToEnd();
            reader.Close();

            JObject obj = JObject.Parse(text);

            FastList<PEventConfig> eventData = _getEventData(obj["_events"]);
            FastList<PNoteConfig> noteData = _getNoteData(obj["_notes"]);
            FastList<PObstacleConfig> obstacleData = _getObstacleData(obj["_obstacles"]);
            FastList<PBookmarkConfig> bookmarkData = _getBookmarkData(obj["_bookmarks"]);

            PMappingContainer container = new PMappingContainer();
            container.eventData = eventData;
            container.noteData = noteData;
            container.obstacleData = obstacleData;
            container.bookmarkData = bookmarkData;

            return container;
        }

        private static FastList<PEventConfig> _getEventData(JToken eventToken)
        {
            FastList<PEventConfig> eventData = new FastList<PEventConfig>(); 
            foreach (JToken child in eventToken.Children())
            {
                PEventConfig eventConfig = new PEventConfig();
                eventConfig.time = child["_time"].Value<float>();
                eventConfig.type = child["_type"].Value<int>();
                eventConfig.value = child["_value"].Value<int>();
                eventData.Add(eventConfig);
            }
            return eventData;
        }

        private static FastList<PNoteConfig> _getNoteData(JToken noteToken)
        {
            FastList<PNoteConfig> noteData = new FastList<PNoteConfig>();
            foreach (JToken child in noteToken.Children())
            {
                PNoteConfig noteConfig = new PNoteConfig();
                noteConfig.time = child["_time"].Value<float>();
                noteConfig.lineIndex = child["_lineIndex"].Value<int>();
                noteConfig.lineLayer = child["_lineLayer"].Value<int>();
                noteConfig.type = child["_type"].Value<int>();
                noteConfig.cutDirection = child["_cutDirection"].Value<int>();
                noteData.Add(noteConfig);
            }
            return noteData;
        }

        private static FastList<PObstacleConfig> _getObstacleData(JToken obstacleToken)
        {
            FastList<PObstacleConfig> obstacleData = new FastList<PObstacleConfig>();
            foreach (JToken child in obstacleToken.Children())
            {
                PObstacleConfig obstacleConfig = new PObstacleConfig();
                obstacleConfig.time = child["_time"].Value<float>();
                obstacleConfig.lineIndex = child["_lineIndex"].Value<int>();
                obstacleConfig.type = child["_type"].Value<int>();
                obstacleConfig.duration = child["_duration"].Value<int>();
                obstacleConfig.width = child["_width"].Value<float>();
                obstacleData.Add(obstacleConfig);
            }
            return obstacleData;
        }

        private static FastList<PBookmarkConfig> _getBookmarkData(JToken bookmarkToken)
        {
            FastList<PBookmarkConfig> bookmarkData = new FastList<PBookmarkConfig>();
            foreach (JToken child in bookmarkToken.Children())
            {
                PBookmarkConfig newConfig = new PBookmarkConfig();
                newConfig.time = child["_time"].Value<float>();
                newConfig.name = child["_name"].Value<string>();
                bookmarkData.Add(newConfig);
            }
            return bookmarkData;
        }
    } 

}