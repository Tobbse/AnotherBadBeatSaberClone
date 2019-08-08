using UnityEngine;
using PSpectrumInfo;
using PAnalyzerConfigs;

public class GlobalStorage : ScriptableObject
{
    public static GlobalStorage Instance;

    // Audio Loading Storage
    private AudioClip _audioClip;
    private FastList<PAnalyzedSpectrumData> _spectrumInfo;
    private TrackConfig _analyzerConfig;
    private FastList<double[]> _spectrumsList;
    private string _audioPath;

    public AudioClip AudioClip { get => _audioClip; set => _audioClip = value; }
    public FastList<PAnalyzedSpectrumData> SpectrumInfo { get => _spectrumInfo; set => _spectrumInfo = value; }
    public TrackConfig AnalyzerConfig { get => _analyzerConfig; set => _analyzerConfig = value; }
    public FastList<double[]> SpectrumsList { get => _spectrumsList; set => _spectrumsList = value; }
    public string AudioPath { get => _audioPath; set => _audioPath = value; }

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
