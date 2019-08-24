using UnityEngine;
using PAudioAnalyzer;
using AudioSpectrumInfo;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;

public class AudioAnalyzerLoader : MonoBehaviour
{
    public AudioSource testAudioSource;

    private TrackConfig _trackConfig;
    private SpectrumAnalyzer _spectrumAnalyzer;
    private List<AnalyzedSpectrumConfig> _spectrumDataList;
    private AudioClip _audioClip;
    private float[] _monoSamples;
    private JsonFileHandler _jsonFileHandler;
    private string _difficulty;

    private void Awake()
    {
        GameObject playerMenu = GameObject.Find("PlayerMenu");
        if (playerMenu != null) playerMenu.SetActive(false);
    }

    void Start()
    {
        _jsonFileHandler = new JsonFileHandler();
        _difficulty = GlobalStorage.getInstance().Difficulty;

        string path = GlobalStorage.getInstance().AudioPath;
        StartCoroutine(LoadMp3AudioClip(path));
    }

    private void _clipLoaded()
    {
        _trackConfig = new TrackConfig(_audioClip.frequency, _audioClip.name);

        string fullPath = _jsonFileHandler.getFullFilePath(JsonFileHandler.MAPPING_FOLDER_PATH, _trackConfig.TrackName, _difficulty);
        if (GlobalStaticSettings.USE_CACHE && _jsonFileHandler.fileExists(fullPath))
        {
            loadMappingFromCache();
            Debug.Log("Loading track mapping from cache.");
            return;
        }

        SpectrumProvider audioProvider = new SpectrumProvider(_trackConfig.ClipSampleRate);
        _monoSamples = AudioSampleProvider.getMonoSamples(_audioClip);
        List<double[]> spectrumsList = audioProvider.getSpectrums(_monoSamples);
        _spectrumDataList = audioProvider.getSpectrumData(spectrumsList, _trackConfig.Bands);
        _spectrumAnalyzer = new SpectrumAnalyzer(_trackConfig, _spectrumDataList, new MappingContainer());
        _spectrumAnalyzer.analyzeSpectrumsList(analaysisFinished);
    }

    private IEnumerator LoadMp3AudioClip(string path)
    {
        _audioClip = Mp3Loader.LoadMp3(path);
        while(!_audioClip.isReadyToPlay)
        {
            yield return _audioClip;
        }
        _audioClip.LoadAudioData();
        _clipLoaded();
    }

    private void analaysisFinished()
    {
        MappingContainer mappingContainer = _spectrumAnalyzer.getBeatMappingContainer();
        mappingContainer.sortMappings(); // Because multiple band spectrums are analyzed sequentially, we have to sort the mappings by time.

        _jsonFileHandler.writeMappingFile(mappingContainer, _trackConfig.TrackName, _difficulty);

        _spectrumDataList = _spectrumAnalyzer.getAnalyzedSpectrumData();

        GlobalStorage.getInstance().AudioClip = _audioClip;
        GlobalStorage.getInstance().SpectrumInfo = _spectrumDataList;
        GlobalStorage.getInstance().TrackConfig = _trackConfig;
        GlobalStorage.getInstance().MappingContainer = mappingContainer;

        SceneManager.LoadScene("Game");
    }

    private void loadMappingFromCache()
    {
        string mappingPath = _jsonFileHandler.getFullFilePath(JsonFileHandler.MAPPING_FOLDER_PATH, _trackConfig.TrackName, GlobalStorage.getInstance().Difficulty);

        MappingContainer mappingContainer = _jsonFileHandler.readMappingFile(mappingPath);

        GlobalStorage.getInstance().MappingPath = mappingPath;
        GlobalStorage.getInstance().AudioClip = _audioClip;
        GlobalStorage.getInstance().TrackConfig = _trackConfig;
        GlobalStorage.getInstance().MappingContainer = mappingContainer;

        SceneManager.LoadScene("Game");
    }
}
