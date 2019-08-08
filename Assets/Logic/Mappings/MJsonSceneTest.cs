using UnityEngine;
using PMappingConfigs;

public class MJsonSceneTest : MonoBehaviour
{
    void Start()
    {
        PJsonMappingHandler handler = new PJsonMappingHandler();
        PMappingContainer mappingContainer = handler.readFile("BeatMappings/mapping_example/Easy.dat");
        int i = 0;
    }
}