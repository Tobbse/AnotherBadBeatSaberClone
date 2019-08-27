using UnityEngine;
using AudioSpectrumInfo;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;

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
            return ScriptableObject.CreateInstance<GlobalStorage>();
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
