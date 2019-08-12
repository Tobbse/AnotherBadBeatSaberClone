using UnityEngine;
using MappingConfigs;

public class JsonSceneTest : MonoBehaviour
{
    void Start()
    {
        JsonMappingHandler handler = new JsonMappingHandler();
        MappingContainer mappingContainer = handler.readFile("BeatMappings/mapping_example/Easy.dat");
        int i = 0;
    }
}