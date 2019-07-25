using UnityEngine;
using PSpectrumData;

public class GlobalStorage : ScriptableObject
{
    public static GlobalStorage Instance;

    private AudioClip audioClip;
    private FastList<SpectrumInfo> spectrumInfo;
    private PAnalyzerConfig analyzerConfig;
    private FastList<double[]> spectrumsList;
    private string audioPath;

    public AudioClip AudioClip { get => audioClip; set => audioClip = value; }
    public FastList<SpectrumInfo> SpectrumInfo { get => spectrumInfo; set => spectrumInfo = value; }
    public PAnalyzerConfig AnalyzerConfig { get => analyzerConfig; set => analyzerConfig = value; }
    public FastList<double[]> SpectrumsList { get => spectrumsList; set => spectrumsList = value; }
    public string AudioPath { get => audioPath; set => audioPath = value; }

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
