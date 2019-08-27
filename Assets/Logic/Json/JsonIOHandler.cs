using System.IO;
using Newtonsoft.Json.Linq;
using BeatMappingConfigs;
using System.Collections.Generic;

namespace JsonIOHandler {

    public static class JsonFileWriter
    {
        public static void writeFile(string json, FileInfo fileInfo)
        {
            string fullFilePath = fileInfo.FullName;
            fileInfo.Directory.Create();

            if (!File.Exists(fileInfo.FullName))
            {
                FileStream stream = File.Create(fullFilePath);
                stream.Close();
            }
            File.WriteAllText(fullFilePath, string.Empty);
            StreamWriter writer = new StreamWriter(fullFilePath, true);
            writer.WriteLine(json);
            writer.Close();
        }
    }

    public static class JsonFileReader
    {
        public static List<HighscoreData> readHighscoreFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string text = reader.ReadToEnd();
            reader.Close();

            JObject obj = JObject.Parse(text);
            JToken highscoreToken = obj["_highscores"];

            List<HighscoreData> highscoreData = new List<HighscoreData>();
            foreach (JToken child in highscoreToken.Children())
            {
                HighscoreData score = new HighscoreData();
                score.score = child["_score"].Value<int>();
                score.rank = child["_rank"].Value<int>();
                highscoreData.Add(score);
            }
            return highscoreData;
        }

        public static MappingInfo readInfoFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string text = reader.ReadToEnd();
            reader.Close();

            MappingInfo info = new MappingInfo();
            JObject obj = JObject.Parse(text);

            float bpm = obj["_beatsPerMinute"].Value<float>();
            if (bpm == 0f) bpm = 1;
            info.bpm = bpm;

            return info;
        }

        public static MappingContainer readMappingFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string text = reader.ReadToEnd();
            reader.Close();

            JObject obj = JObject.Parse(text);

            List<EventConfig> eventData = _getEventData(obj["_events"]);
            List<NoteConfig> noteData = _getNoteData(obj["_notes"]);
            List<ObstacleConfig> obstacleData = _getObstacleData(obj["_obstacles"]);
            List<BookmarkConfig> bookmarkData = _getBookmarkData(obj["_bookmarks"]);

            MappingContainer container = new MappingContainer();
            container.eventData = eventData;
            container.noteData = noteData;
            container.obstacleData = obstacleData;
            container.bookmarkData = bookmarkData;

            return container;
        }

        private static List<EventConfig> _getEventData(JToken eventToken)
        {
            List<EventConfig> eventData = new List<EventConfig>(); 
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

        private static List<NoteConfig> _getNoteData(JToken noteToken)
        {
            List<NoteConfig> noteData = new List<NoteConfig>();
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

        private static List<ObstacleConfig> _getObstacleData(JToken obstacleToken)
        {
            List<ObstacleConfig> obstacleData = new List<ObstacleConfig>();
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

        private static List<BookmarkConfig> _getBookmarkData(JToken bookmarkToken)
        {
            List<BookmarkConfig> bookmarkData = new List<BookmarkConfig>();
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