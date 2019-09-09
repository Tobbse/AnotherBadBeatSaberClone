using UnityEngine;
using BeatMappingConfigs;

/**
 * This was used in a test scene, just to quickly test out whether the parsing of a json mapping worked correctly,
 * as there was quite some debugging necessary.
 **/
public class JsonSceneTest : MonoBehaviour
{
    void Start()
    {
        JsonController handler = new JsonController();
        MappingContainer mappingContainer = handler.readMappingFile("Assets/Resources/SongData/BeatMappings/mapping_example/Easy.dat");
        int i = 0;
    }
}