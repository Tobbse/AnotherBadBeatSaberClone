using UnityEngine;
using PSpectrumInfo;
using PAnalyzerConfigs;
using PMappingConfigs;

public class GlobalStorage : ScriptableObject
{
    public static GlobalStorage Instance;

    // Audio Loading Storage
    private AudioClip _audioClip;
    private FastList<PAnalyzedSpectrumData> _spectrumInfo;
    private TrackConfig _trackConfig;
    private FastList<double[]> _spectrumsList;
    private string _audioPath;
    private PMappingContainer _mappingContainer;
    private string _difficulty;
    private string _mappingPath;

    public AudioClip AudioClip { get => _audioClip; set => _audioClip = value; }
    public FastList<PAnalyzedSpectrumData> SpectrumInfo { get => _spectrumInfo; set => _spectrumInfo = value; }
    public TrackConfig TrackConfig { get => _trackConfig; set => _trackConfig = value; }
    public FastList<double[]> SpectrumsList { get => _spectrumsList; set => _spectrumsList = value; }
    public string AudioPath { get => _audioPath; set => _audioPath = value; }
    public PMappingContainer MappingContainer { get => _mappingContainer; set => _mappingContainer = value; }
    public string Difficulty { get => _difficulty; set => _difficulty = value; }
    public string MappingPath { get => _mappingPath; set => _mappingPath = value; }

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
}
