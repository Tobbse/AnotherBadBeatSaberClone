using UnityEngine;
using PAudioAnalyzer;
using PSpectrumInfo;
using PAnalyzerConfigs;
using PMappingConfigs;

public class MPlotDemoAudioAnalyzerController : MonoBehaviour
{
    private TrackConfig _analyzerConfig;
    private PSpectrumAnalyzer _spectrumAnalyzer;
    private MSpectrumPlotter _spectrumPlotter;
    private FastList<PAnalyzedSpectrumData> _spectrumDataList;
    private bool _started;

    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        _analyzerConfig = new TrackConfig(audioSource.clip.frequency, audioSource.clip.name);
        PSpectrumProvider audioProvider = new PSpectrumProvider(_analyzerConfig.ClipSampleRate);

        float[] samples = PAudioSampleProvider.getMonoSamples(audioSource.clip);
        FastList<double[]> spectrumsList = audioProvider.getSpectrums(samples);
        _spectrumDataList = audioProvider.getSpectrumData(spectrumsList, _analyzerConfig.Bands);

        _spectrumAnalyzer = new PSpectrumAnalyzer(spectrumsList, _analyzerConfig, _spectrumDataList, new PMappingContainer());
        _spectrumAnalyzer.analyzeSpectrumsList(done);
    }

    private void done()
    {
        enabled = false;
        _spectrumDataList = _spectrumAnalyzer.getAnalyzedSpectrumData();
        _spectrumPlotter = GetComponent<MSpectrumPlotter>();
        _spectrumPlotter.setDataAndStart(_spectrumDataList, MSpectrumPlotter.SHOW_PEAKS);

        TimedBlocksGameController timedBlocksGameController = GameObject.Find("TimedBlocksGameController").GetComponent< TimedBlocksGameController>();
        timedBlocksGameController.setSpectrumData(_spectrumDataList);
    }
}
