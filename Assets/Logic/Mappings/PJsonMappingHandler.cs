using UnityEngine;
using PMappingConfigs;
using PJsonIOHandler;
using PAnalyzerConfigs;

public class PJsonMappingHandler : ScriptableObject
{
    public PJsonMappingHandler()
    {

    }

    public void writeFile(PMappingContainer beatMappingContainer, TrackConfig trackCfg)
    {
        string version = "2.0.0";
        string folderPath = "BeatMappings/" + trackCfg.TrackName;

        PMappingJsonStringBuilder jsonBuilder = new PMappingJsonStringBuilder();
        jsonBuilder.setData(beatMappingContainer, version);
        string json = jsonBuilder.getJsonString();
        string info = jsonBuilder.getInfoJsonString(version, trackCfg.TrackName);

        PMappingJsonFileWriter.writeFile(json, folderPath, "/Easy.dat");
        PMappingJsonFileWriter.writeFile(info, folderPath, "/info.dat");
    }

    public PMappingContainer readFile(string filePath)
    {
        return PMappingJsonFileReader.readMappingFile(filePath);
    }

}