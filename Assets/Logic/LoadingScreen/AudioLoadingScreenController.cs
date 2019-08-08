using UnityEngine;
using PAudioAnalyzer;
using PSpectrumInfo;
using UnityEngine.SceneManagement;
using System.Collections;
using PAnalyzerConfigs;
using PMappingConfigs;

public class AudioLoadingScreenController : MonoBehaviour
{
    public AudioSource testAudioSource;

    private TrackConfig _trackConfig;
    private PSpectrumAnalyzer _spectrumAnalyzer;
    private FastList<PAnalyzedSpectrumData> _spectrumDataList;
    private AudioClip _audioClip;
    private FastList<double[]> _spectrumsList;
    private float[] _monoSamples;
    private WWW _www;

    void Start()
    {
        if (SceneManager.GetActiveScene().isLoaded == false)
        {
            SceneManager.sceneLoaded += _onSceneLoaded;
        } else
        {
            _init();
        }
    }

    private void _onSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        _init();
    }

    private void _init()
    {
        string path = GlobalStorage.Instance.AudioPath;
        StartCoroutine(LoadMp3AudioClip(path));
    }

    private void _clipLoaded()
    {
        _trackConfig = new TrackConfig(_audioClip.frequency, _audioClip.name);
        PSpectrumProvider audioProvider = new PSpectrumProvider(_trackConfig.ClipSampleRate);

        _monoSamples = PAudioSampleProvider.getMonoSamples(_audioClip);
        _spectrumsList = audioProvider.getSpectrums(_monoSamples);
        _spectrumDataList = audioProvider.getSpectrumData(_spectrumsList, _trackConfig.Bands);

        _spectrumAnalyzer = new PSpectrumAnalyzer(_spectrumsList, _trackConfig, _spectrumDataList, new PMappingContainer());
        _spectrumAnalyzer.analyzeSpectrumsList(done);
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

    private void done()
    {
        // TODO Now we have to write this shit into a json file instead of saving it into the static storage.
        PJsonMappingHandler handler = new PJsonMappingHandler();
        PMappingContainer mappingContainer = _spectrumAnalyzer.getBeatMappingContainer();
        handler.writeFile(mappingContainer, _trackConfig);

        _spectrumDataList = _spectrumAnalyzer.getAnalyzedSpectrumData();

        GlobalStorage.Instance.AudioClip = _audioClip;
        GlobalStorage.Instance.SpectrumInfo = _spectrumDataList;
        GlobalStorage.Instance.AnalyzerConfig = _trackConfig;
        GlobalStorage.Instance.SpectrumsList = _spectrumsList;

        SceneManager.LoadScene("Game");
    }
}
