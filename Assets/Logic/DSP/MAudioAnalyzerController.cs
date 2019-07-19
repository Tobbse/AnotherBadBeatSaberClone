using UnityEngine;
using AudioAnalyzerPlain;
using PSpectrumData;
using UnityEditor;

public class MAudioAnalyzerController : MonoBehaviour
{
    private PAnalyzerConfig _analyzerConfig;
    private PSpectrumAnalyzer _spectrumAnalyzer;
    private MSpectrumPlotter _spectrumPlotter;
    private FastList<SpectrumInfo> _spectrumDataList;
    private bool _started;

    // Start is called before the first frame update
    void Start()
    {
        AudioImporter importer = new AudioImporter();
        AudioSource audioSource = GetComponent<AudioSource>();
        _analyzerConfig = new PAnalyzerConfig(audioSource.clip.frequency);
        PSpectrumProvider audioProvider = new PSpectrumProvider(_analyzerConfig.ClipSampleRate);

        float[] samples = PAudioSampleProvider.getMonoSamples(audioSource);
        FastList<double[]> spectrumsList = audioProvider.getSpectrums(samples);
        _spectrumDataList = audioProvider.getSpectrumData(spectrumsList, _analyzerConfig.Bands);

        _spectrumAnalyzer = new PSpectrumAnalyzer(spectrumsList, _analyzerConfig, _spectrumDataList);
        _spectrumAnalyzer.analyzeSpectrumsList();
    }

    // Update is called once per frame
    void Update()
    {
        if (_spectrumAnalyzer == null) return;

        if (!_started && _spectrumAnalyzer.isReady())
        {
            _started = true;
            _spectrumDataList = _spectrumAnalyzer.getSpectrumDataList();
            _spectrumPlotter = GetComponent<MSpectrumPlotter>();
            _spectrumPlotter.setSpectrumData(_spectrumDataList);

            TimedBlocksGameController timedBlocksGameController = GameObject.Find("TimedBlocksGameController").GetComponent< TimedBlocksGameController>();
            timedBlocksGameController.setSpectrumData(_spectrumDataList);
            enabled = false;
        }
    }

}
