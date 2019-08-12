using UnityEngine;
using PAudioAnalyzer;
using AudioSpectrumInfo;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;

public class PlotDemoAudioAnalyzerController : MonoBehaviour
{
    private TrackConfig _analyzerConfig;
    private SpectrumAnalyzer _spectrumAnalyzer;
    private SpectrumPlotter _spectrumPlotter;
    private FastList<AnalyzedSpectrumData> _spectrumDataList;
    private bool _started;

    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        _analyzerConfig = new TrackConfig(audioSource.clip.frequency, audioSource.clip.name);
        SpectrumProvider audioProvider = new SpectrumProvider(_analyzerConfig.ClipSampleRate);

        float[] samples = AudioSampleProvider.getMonoSamples(audioSource.clip);
        FastList<double[]> spectrumsList = audioProvider.getSpectrums(samples);
        _spectrumDataList = audioProvider.getSpectrumData(spectrumsList, _analyzerConfig.Bands);

        _spectrumAnalyzer = new SpectrumAnalyzer(spectrumsList, _analyzerConfig, _spectrumDataList, new MappingContainer());
        _spectrumAnalyzer.analyzeSpectrumsList(done);
    }

    private void done()
    {
        enabled = false;
        _spectrumDataList = _spectrumAnalyzer.getAnalyzedSpectrumData();
        _spectrumPlotter = GetComponent<SpectrumPlotter>();
        _spectrumPlotter.setDataAndStart(_spectrumDataList, SpectrumPlotter.SHOW_PEAKS);

        TimedBlocksGameController timedBlocksGameController = GameObject.Find("TimedBlocksGameController").GetComponent< TimedBlocksGameController>();
        timedBlocksGameController.setSpectrumData(_spectrumDataList);
    }
}
