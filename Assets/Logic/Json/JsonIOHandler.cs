using System.IO;
using Newtonsoft.Json.Linq;
using BeatMappingConfigs;
using System.Collections.Generic;

/**
 * Classes in this namespace take care or writing out and parsing Json files.
 **/
namespace JsonIOHandler
{

    /**
     * Writes a json file.
     **/
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

    /**
     * Parses a json file.
     * Contains methods for specifically parsing either a beat mapping file returning a mapping container,
     * or parsing an info file to get an info config.
     **/
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
                score.Score = child["_score"].Value<int>();
                score.Rank = child["_rank"].Value<int>();
                highscoreData.Add(score);
            }
            return highscoreData;
        }

        // Returns Mapping Info config from info json file.
        public static MappingInfo readInfoFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string text = reader.ReadToEnd();
            reader.Close();

            MappingInfo info = new MappingInfo();
            JObject obj = JObject.Parse(text);

            float bpm = obj["_beatsPerMinute"].Value<float>();
            if (bpm == 0f) bpm = 1;
            info.Bpm = bpm;

            return info;
        }

        // Creates mapping container from a mapping json file.
        public static MappingContainer readMappingFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string text = reader.ReadToEnd();
            reader.Close();

            JObject obj = JObject.Parse(text);

            MappingContainer container = new MappingContainer()
            {
                EventData = _getEventData(obj["_events"]),
                NoteData = _getNoteData(obj["_notes"]),
                ObstacleData = _getObstacleData(obj["_obstacles"]),
                BookmarkData = _getBookmarkData(obj["_bookmarks"])
            };
            return container;
        }

        private static List<EventConfig> _getEventData(JToken eventToken)
        {
            List<EventConfig> eventData = new List<EventConfig>(); 
            foreach (JToken child in eventToken.Children())
            {
                EventConfig eventConfig = new EventConfig()
                {
                    Time = child["_time"].Value<float>(),
                    Type = child["_type"].Value<int>(),
                    Value = child["_value"].Value<int>()
                };
                eventData.Add(eventConfig);
            }
            return eventData;
        }

        private static List<NoteConfig> _getNoteData(JToken noteToken)
        {
            List<NoteConfig> noteData = new List<NoteConfig>();
            foreach (JToken child in noteToken.Children())
            {
                NoteConfig noteConfig = new NoteConfig()
                {
                    Time = child["_time"].Value<float>(),
                    LineIndex = child["_lineIndex"].Value<int>(),
                    LineLayer = child["_lineLayer"].Value<int>(),
                    Type = child["_type"].Value<int>(),
                    CutDirection = child["_cutDirection"].Value<int>()
                };
                noteData.Add(noteConfig);
            }
            return noteData;
        }

        private static List<ObstacleConfig> _getObstacleData(JToken obstacleToken)
        {
            List<ObstacleConfig> obstacleData = new List<ObstacleConfig>();
            foreach (JToken child in obstacleToken.Children())
            {
                ObstacleConfig obstacleConfig = new ObstacleConfig()
                {
                    Time = child["_time"].Value<float>(),
                    LineIndex = child["_lineIndex"].Value<int>(),
                    Type = child["_type"].Value<int>(),
                    Duration = child["_duration"].Value<int>(),
                    Width = child["_width"].Value<float>()
                };
                obstacleData.Add(obstacleConfig);
            }
            return obstacleData;
        }

        private static List<BookmarkConfig> _getBookmarkData(JToken bookmarkToken)
        {
            List<BookmarkConfig> bookmarkData = new List<BookmarkConfig>();
            foreach (JToken child in bookmarkToken.Children())
            {
                BookmarkConfig newConfig = new BookmarkConfig()
                {
                    Time = child["_time"].Value<float>(), Name = child["_name"].Value<string>()
                };
                bookmarkData.Add(newConfig);
            }
            return bookmarkData;
        }
    }

}
 