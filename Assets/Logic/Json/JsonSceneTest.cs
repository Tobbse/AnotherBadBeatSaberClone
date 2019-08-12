using UnityEngine;
using BeatMappingConfigs;

public class JsonSceneTest : MonoBehaviour
{
    void Start()
    {
        JsonFileHandler handler = new JsonFileHandler();
        MappingContainer mappingContainer = handler.readMappingFile("Assets/Resources/SongData/BeatMappings/mapping_example/Easy.dat");
        int i = 0;
    }
}