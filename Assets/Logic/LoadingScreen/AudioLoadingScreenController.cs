using UnityEngine;
using PAudioAnalyzer;
using PSpectrumData;
using UnityEngine.SceneManagement;
//using NAudio.Wave;
//using NAudio.Wave.SampleProviders;
using System.Collections;

public class AudioLoadingScreenController : MonoBehaviour
{
    public AudioSource testAudioSource;

    private PAnalyzerConfig _analyzerConfig;
    private PSpectrumAnalyzer _spectrumAnalyzer;
    private FastList<PSpectrumInfo> _spectrumDataList;
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
        _analyzerConfig = new PAnalyzerConfig(_audioClip.frequency);
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

    // This currently does not work!
    /*private AudioClip _loadResampledAudioData(string path)
    {
        AudioFileReader reader = new AudioFileReader(path);
        int channels = reader.WaveFormat.Channels;
        int systemSampleRate = AudioSettings.outputSampleRate;
        int numSamples = (int)(reader.Length / channels / (reader.WaveFormat.BitsPerSample / 8));
        float[] audioData = new float[numSamples];

        WdlResamplingSampleProvider resampler = new WdlResamplingSampleProvider(reader, systemSampleRate);
        resampler.Read(audioData, 0, numSamples);

        AudioClip audioClip = AudioClip.Create(path, numSamples, channels, systemSampleRate, false);
        audioClip.SetData(audioData, 0);
        return audioClip;
    }*/

    private void done()
    {
        _spectrumDataList = _spectrumAnalyzer.getSpectrumDataList();

        GlobalStorage.Instance.AudioClip = _audioClip;
        GlobalStorage.Instance.SpectrumInfo = _spectrumDataList;
        GlobalStorage.Instance.AnalyzerConfig = _analyzerConfig;
        GlobalStorage.Instance.SpectrumsList = _spectrumsList;

        SceneManager.LoadScene("Game");
    }
}
