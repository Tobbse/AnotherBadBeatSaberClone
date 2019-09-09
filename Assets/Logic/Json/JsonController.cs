using BeatMappingConfigs;
using JsonIOHandler;
using System.IO;
using System.Collections.Generic;
using JsonStringBuilders;

/**
 * Controller for Json stuff.
 * Can write and read mapping and info files as well as highscores files by using
 * other classes in this package.
 **/
public class JsonController
{
    public const string MAPPING_VERSION = "2.0.0";
    public const string MAPPING_FOLDER_PATH = "BeatMappings/";
    public const string HIGHSCORE_FOLDER_PATH = "Highscores/";

    public void writeMappingFile(MappingContainer beatMappingContainer, string trackName, string difficulty)
    {
        string folderPath = JsonFilePathUtils.getFolderPath(MAPPING_FOLDER_PATH, trackName);
        string mappingFilePath = JsonFilePathUtils.getFullPath(folderPath, difficulty);

        JsonMappingStringBuilder jsonBuilder = new JsonMappingStringBuilder();
        jsonBuilder.setData(beatMappingContainer);
        string mappingJson = jsonBuilder.getJsonString();

        JsonFileWriter.writeFile(mappingJson, new FileInfo(mappingFilePath));
    }

    public void writeInfoFile(MappingContainer beatMappingContainer, string trackName, float bpm)
    {
        string folderPath = JsonFilePathUtils.getFolderPath(MAPPING_FOLDER_PATH, trackName);
        string infoFilePath = JsonFilePathUtils.getFullPath(folderPath, "info");
        string infoJson = JsonInfoStringBuilder.getJsonString(trackName, bpm);

        JsonFileWriter.writeFile(infoJson, new FileInfo(infoFilePath));
    }

    public void writeHighscoreFile(List<HighscoreData> highscoreData, string trackName, string difficulty)
    {
        string folderPath = JsonFilePathUtils.getFolderPath(HIGHSCORE_FOLDER_PATH, trackName);
        string highscoreFilePath = JsonFilePathUtils.getFullPath(folderPath, difficulty);

        JsonHighscoreStringBuilder jsonBuilder = new JsonHighscoreStringBuilder();
        jsonBuilder.setData(highscoreData);
        string highscoreJson = jsonBuilder.getJsonString();

        JsonFileWriter.writeFile(highscoreJson, new FileInfo(highscoreFilePath));
    }

    public MappingContainer readMappingFile(string fullFilePath)
    {
        return JsonFileReader.readMappingFile(fullFilePath);
    }

    public MappingInfo readInfoFile(string fullFilePath)
    {
        return JsonFileReader.readInfoFile(fullFilePath);
    }

    public List<HighscoreData> readHighscoreFile(string fullFilePath)
    {
        return JsonFileReader.readHighscoreFile(fullFilePath);
    }

    public bool fileExists(string fullFilePath)
    {
        return File.Exists(fullFilePath);
    }

    public string getFullMappingPath(string subFolder, string trackName, string difficulty)
    {
        string folderPath = JsonFilePathUtils.getFolderPath(subFolder, trackName);
        return JsonFilePathUtils.getFullPath(folderPath, difficulty);
    }

    public string getFullInfoPath(string subFolder, string trackName)
    {
        string folderPath = JsonFilePathUtils.getFolderPath(subFolder, trackName);
        return folderPath + "info.dat";
    }
}
