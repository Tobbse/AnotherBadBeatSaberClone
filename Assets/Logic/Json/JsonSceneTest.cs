using UnityEngine;
using BeatMappingConfigs;

public class JsonSceneTest : MonoBehaviour
{
    void Start()
    {
        JsonController handler = new JsonController();
        MappingContainer mappingContainer = handler.readMappingFile("Assets/Resources/SongData/BeatMappings/mapping_example/Easy.dat");
        int i = 0;
    }
}