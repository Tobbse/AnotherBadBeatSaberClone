using UnityEngine;
using PAudioAnalyzer;
using PSpectrumData;

public class MAudioAnalyzerController : MonoBehaviour
{
    private PAnalyzerConfig _analyzerConfig;
    private PSpectrumAnalyzer _spectrumAnalyzer;
    private MSpectrumPlotter _spectrumPlotter;
    private FastList<PSpectrumInfo> _spectrumDataList;
    private bool _started;

    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        _analyzerConfig = new PAnalyzerConfig(audioSource.clip.frequency);
        PSpectrumProvider audioProvider = new PSpectrumProvider(_analyzerConfig.ClipSampleRate);

        float[] samples = PAudioSampleProvider.getMonoSamples(audioSource.clip);
        FastList<double[]> spectrumsList = audioProvider.getSpectrums(samples);
        _spectrumDataList = audioProvider.getSpectrumData(spectrumsList, _analyzerConfig.Bands);

        _spectrumAnalyzer = new PSpectrumAnalyzer(spectrumsList, _analyzerConfig, _spectrumDataList);
        _spectrumAnalyzer.analyzeSpectrumsList(done);
    }

    // TODO would be nicer to pass a callback instead of checking isReady ever frame.
    private void done()
    {
        enabled = false;
        _spectrumDataList = _spectrumAnalyzer.getSpectrumDataList();
        _spectrumPlotter = GetComponent<MSpectrumPlotter>();
        _spectrumPlotter.setDataAndStart(_spectrumDataList, MSpectrumPlotter.SHOW_PEAKS);

        TimedBlocksGameController timedBlocksGameController = GameObject.Find("TimedBlocksGameController").GetComponent< TimedBlocksGameController>();
        timedBlocksGameController.setSpectrumData(_spectrumDataList);
    }
}
