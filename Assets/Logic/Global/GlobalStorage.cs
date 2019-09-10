using UnityEngine;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;

/*
 * Global storage for data. This singleton data container can be accessed from all scenes.
 **/
public class GlobalStorage : ScriptableObject
{
    private static GlobalStorage Instance;

    private AudioClip _audioClip;
    private TrackConfig _trackConfig;
    private string _audioPath;
    private MappingContainer _mappingContainer;
    private string _difficulty;

    public AudioClip AudioClip { get => _audioClip; set => _audioClip = value; }
    public TrackConfig TrackConfig { get => _trackConfig; set => _trackConfig = value; }
    public string AudioPath { get => _audioPath; set => _audioPath = value; }
    public MappingContainer MappingContainer { get => _mappingContainer; set => _mappingContainer = value; }
    public string Difficulty { get => _difficulty; set => _difficulty = value; }

    public static GlobalStorage getInstance()
    {
        if (Instance == null)
        {
            // This should normally be initialized using ScriptableObject.CreateInstance<GlobalStorage>();
            // Hwoever, this creation is not executed on the main thread, so we can't use that functionality.
            return new GlobalStorage();
        }
        return Instance;
    }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    public void destroy()
    {
        Instance = null;
    }
}
