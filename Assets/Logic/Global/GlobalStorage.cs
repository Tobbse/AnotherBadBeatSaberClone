using UnityEngine;
using AudioSpectrumInfo;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;

public class GlobalStorage : ScriptableObject
{
    private static GlobalStorage Instance;

    // Audio Loading Storage
    private AudioClip _audioClip;
    private List<AnalyzedSpectrumConfig> _spectrumInfo;
    private TrackConfig _trackConfig;
    private string _audioPath;
    private MappingContainer _mappingContainer;
    private string _difficulty;
    private string _mappingPath;

    public AudioClip AudioClip { get => _audioClip; set => _audioClip = value; }
    public List<AnalyzedSpectrumConfig> SpectrumInfo { get => _spectrumInfo; set => _spectrumInfo = value; }
    public TrackConfig TrackConfig { get => _trackConfig; set => _trackConfig = value; }
    public string AudioPath { get => _audioPath; set => _audioPath = value; }
    public MappingContainer MappingContainer { get => _mappingContainer; set => _mappingContainer = value; }
    public string Difficulty { get => _difficulty; set => _difficulty = value; }
    public string MappingPath { get => _mappingPath; set => _mappingPath = value; }

    public static GlobalStorage getInstance()
    {
        if (Instance == null)
        {
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
}
