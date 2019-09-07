using UnityEngine;
using UnityEditor;
using System.Globalization;

public class JsonInfoStringBuilder
{
    // TODO really not implemented yet. Most important part if just to get the BPM right as that will influence the tempo of the song.
    public string getJsonString(string trackName, float bpm)
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