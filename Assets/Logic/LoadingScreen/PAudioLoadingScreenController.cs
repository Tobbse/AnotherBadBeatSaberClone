using UnityEngine;
using PAudioAnalyzer;
using PSpectrumInfo;
using UnityEngine.SceneManagement;
using System.Collections;
using PAnalyzerConfigs;
using PMappingConfigs;

public class PAudioLoadingScreenController : MonoBehaviour
{
    public AudioSource testAudioSource;

    private TrackConfig _trackConfig;
    private PSpectrumAnalyzer _spectrumAnalyzer;
    private FastList<PAnalyzedSpectrumData> _spectrumDataList;
    private AudioClip _audioClip;
    private FastList<double[]> _spectrumsList;
    private float[] _monoSamples;
    private PJsonMappingHandler _jsonMappingHandler;
    private string _difficulty;

    void Start()
    {
        _jsonMappingHandler = new PJsonMappingHandler();
        _difficulty = GlobalStorage.Instance.Difficulty;

        string path = GlobalStorage.Instance.AudioPath;
        StartCoroutine(LoadMp3AudioClip(path));
    }

    private void _clipLoaded()
    {
        _trackConfig = new TrackConfig(_audioClip.frequency, _audioClip.name);

        if (_jsonMappingHandler.mappingExists(_trackConfig, _difficulty))
        {
            loadMappingFromCache();
            Debug.Log("Loading track mapping from cache.");
            return;
        }

        PSpectrumProvider audioProvider = new PSpectrumProvider(_trackConfig.ClipSampleRate);
        _monoSamples = PAudioSampleProvider.getMonoSamples(_audioClip);
        _spectrumsList = audioProvider.getSpectrums(_monoSamples);
        _spectrumDataList = audioProvider.getSpectrumData(_spectrumsList, _trackConfig.Bands);
        _spectrumAnalyzer = new PSpectrumAnalyzer(_spectrumsList, _trackConfig, _spectrumDataList, new PMappingContainer());
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
        PMappingContainer mappingContainer = _spectrumAnalyzer.getBeatMappingContainer();
        _jsonMappingHandler.writeFile(mappingContainer, _trackConfig, _difficulty);

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
        string mappingPath = _jsonMappingHandler.getFullPath(_trackConfig, GlobalStorage.Instance.Difficulty);

        PMappingContainer mappingContainer = _jsonMappingHandler.readFile(mappingPath);

        GlobalStorage.Instance.MappingPath = mappingPath;
        GlobalStorage.Instance.AudioClip = _audioClip;
        GlobalStorage.Instance.TrackConfig = _trackConfig;
        GlobalStorage.Instance.MappingContainer = mappingContainer;

        SceneManager.LoadScene("Game");
    }
}
