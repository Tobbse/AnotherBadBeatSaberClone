using UnityEngine;
using PAudioAnalyzer;
using AudioSpectrumInfo;
using UnityEngine.SceneManagement;
using System.Collections;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;

public class AudioAnalyzerLoader : MonoBehaviour
{
    public AudioSource testAudioSource;

    private TrackConfig _trackConfig;
    private SpectrumAnalyzer _spectrumAnalyzer;
    private FastList<AnalyzedSpectrumData> _spectrumDataList;
    private AudioClip _audioClip;
    private FastList<double[]> _spectrumsList;
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
        _difficulty = GlobalStorage.Instance.Difficulty;

        string path = GlobalStorage.Instance.AudioPath;
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
        _spectrumsList = audioProvider.getSpectrums(_monoSamples);
        _spectrumDataList = audioProvider.getSpectrumData(_spectrumsList, _trackConfig.Bands);
        _spectrumAnalyzer = new SpectrumAnalyzer(_spectrumsList, _trackConfig, _spectrumDataList, new MappingContainer());
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

        GlobalStorage.Instance.AudioClip = _audioClip;
        GlobalStorage.Instance.SpectrumInfo = _spectrumDataList;
        GlobalStorage.Instance.TrackConfig = _trackConfig;
        GlobalStorage.Instance.SpectrumsList = _spectrumsList;
        GlobalStorage.Instance.MappingContainer = mappingContainer;

        SceneManager.LoadScene("Game");
    }

    private void loadMappingFromCache()
    {
        string mappingPath = _jsonFileHandler.getFullFilePath(JsonFileHandler.MAPPING_FOLDER_PATH, _trackConfig.TrackName, GlobalStorage.Instance.Difficulty);

        MappingContainer mappingContainer = _jsonFileHandler.readMappingFile(mappingPath);

        GlobalStorage.Instance.MappingPath = mappingPath;
        GlobalStorage.Instance.AudioClip = _audioClip;
        GlobalStorage.Instance.TrackConfig = _trackConfig;
        GlobalStorage.Instance.MappingContainer = mappingContainer;

        SceneManager.LoadScene("Game");
    }
}
