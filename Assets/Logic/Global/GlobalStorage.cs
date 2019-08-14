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
    private List<AnalyzedSpectrumData> _spectrumInfo;
    private TrackConfig _trackConfig;
    private List<double[]> _spectrumsList;
    private string _audioPath;
    private MappingContainer _mappingContainer;
    private string _difficulty;
    private string _mappingPath;

    public AudioClip AudioClip { get => _audioClip; set => _audioClip = value; }
    public List<AnalyzedSpectrumData> SpectrumInfo { get => _spectrumInfo; set => _spectrumInfo = value; }
    public TrackConfig TrackConfig { get => _trackConfig; set => _trackConfig = value; }
    public List<double[]> SpectrumsList { get => _spectrumsList; set => _spectrumsList = value; }
    public string AudioPath { get => _audioPath; set => _audioPath = value; }
    public MappingContainer MappingContainer { get => _mappingContainer; set => _mappingContainer = value; }
    public string Difficulty { get => _difficulty; set => _difficulty = value; }
    public string MappingPath { get => _mappingPath; set => _mappingPath = value; }

    public static GlobalStorage getInstance()
    {
        if (Instance == null)
        {
            Instance = new GlobalStorage();
        }
        return Instance;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
