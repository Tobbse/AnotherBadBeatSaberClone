using UnityEngine;
using PAudioAnalyzer;
using PSpectrumInfo;
using UnityEngine.SceneManagement;
//using NAudio.Wave;
//using NAudio.Wave.SampleProviders;
using System.Collections;
using PAnalyzerConfigs;

public class AudioLoadingScreenController : MonoBehaviour
{
    public AudioSource testAudioSource;

    private TrackConfig _analyzerConfig;
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
        _analyzerConfig = new TrackConfig(_audioClip.frequency);
        PSpectrumProvider audioProvider = new PSpectrumProvider(_analyzerConfig.ClipSampleRate);

        _monoSamples = PAudioSampleProvider.getMonoSamples(_audioClip);
        _spectrumsList = audioProvider.getSpectrums(_monoSamples);
        _spectrumDataList = audioProvider.getSpectrumData(_spectrumsList, _analyzerConfig.Bands);

        _spectrumAnalyzer = new PSpectrumAnalyzer(_spectrumsList, _analyzerConfig, _spectrumDataList);
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


        _spectrumDataList = _spectrumAnalyzer.getAnalyzedSpectrumData();

        GlobalStorage.Instance.AudioClip = _audioClip;
        GlobalStorage.Instance.SpectrumInfo = _spectrumDataList;
        GlobalStorage.Instance.AnalyzerConfig = _analyzerConfig;
        GlobalStorage.Instance.SpectrumsList = _spectrumsList;

        SceneManager.LoadScene("Game");
    }
}
