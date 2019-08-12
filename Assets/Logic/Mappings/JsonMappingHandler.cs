using UnityEngine;
using MappingConfigs;
using JsonIOHandler;
using AnalyzerConfigs;
using System.IO;

public class JsonMappingHandler : ScriptableObject
{
    private const string BASE_FOLDER_PATH = "BeatMappings/";

    public void writeFile(MappingContainer beatMappingContainer, TrackConfig trackCfg, string difficulty)
    {
        string folderPath = _getFolderPath(trackCfg);

        MappingJsonStringBuilder jsonBuilder = new MappingJsonStringBuilder();
        jsonBuilder.setData(beatMappingContainer);
        string json = jsonBuilder.getJsonString();
        string info = jsonBuilder.getInfoJsonString(trackCfg.TrackName);

        PMappingJsonFileWriter.writeFile(json, folderPath, difficulty + ".dat");
        PMappingJsonFileWriter.writeFile(info, folderPath, "/info.dat");
    }

    public MappingContainer readFile(string filePath)
    {
        return PMappingJsonFileReader.readMappingFile(filePath);
    }

    public bool mappingExists(TrackConfig trackCfg, string difficulty)
    {
        string filePath = getFullPath(trackCfg, difficulty);
        return File.Exists(filePath);
    }

    public string getFullPath(TrackConfig trackCfg, string difficulty)
    {
        return _getFolderPath(trackCfg) + difficulty + ".dat";
    }

    private string _getFolderPath(TrackConfig trackCfg)
    {
        return BASE_FOLDER_PATH + trackCfg.TrackName + "/";
    }
}