using BeatMappingConfigs;
using JsonIOHandler;
using System.IO;
using System.Collections.Generic;

public class JsonFileHandler
{
    public const string MAPPING_FOLDER_PATH = "BeatMappings/";
    public const string HIGHSCORE_FOLDER_PATH = "Highscores/";

    public void writeMappingFile(MappingContainer beatMappingContainer, string trackName, string difficulty)
    {
        string folderPath = FileUtils.getFolderPath(MAPPING_FOLDER_PATH, trackName);
        string mappingFilePath = FileUtils.getFullPath(folderPath, difficulty);
        string infoFilePath = FileUtils.getFullPath(folderPath, "info");

        JsonMappingStringBuilder jsonBuilder = new JsonMappingStringBuilder();
        jsonBuilder.setData(beatMappingContainer);
        string json = jsonBuilder.getJsonString();
        string info = jsonBuilder.getInfoJsonString(trackName);

        JsonFileWriter.writeFile(json, new FileInfo(mappingFilePath));
        JsonFileWriter.writeFile(info, new FileInfo(infoFilePath));
    }

    public void writeHighscoreFile(List<HighscoreData> highscoreData, string trackName, string difficulty)
    {
        string folderPath = FileUtils.getFolderPath(HIGHSCORE_FOLDER_PATH, trackName);
        string highscoreFilePath = FileUtils.getFullPath(folderPath, difficulty);

        JsonHighscoreStringBuilder jsonBuilder = new JsonHighscoreStringBuilder();
        jsonBuilder.setData(highscoreData);
        string json = jsonBuilder.getJsonString();

        JsonFileWriter.writeFile(json, new FileInfo(highscoreFilePath));
    }

    public MappingContainer readMappingFile(string fullFilePath)
    {
        return JsonMappingFileReader.readMappingFile(fullFilePath);
    }

    public List<HighscoreData> readHighscoreFile(string fullFilePath)
    {
        return JsonMappingFileReader.readHighscoreFile(fullFilePath);
    }

    public bool fileExists(string fullFilePath)
    {
        return File.Exists(fullFilePath);
    }

    public string getFullFilePath(string subFolder, string trackName, string difficulty)
    {
        string folderPath = FileUtils.getFolderPath(subFolder, trackName);
        return FileUtils.getFullPath(folderPath, difficulty);
    }
}
