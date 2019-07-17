using UnityEngine;
using AudioAnalyzerPlain;
using PSpectrumData;

public class MAudioAnalyzerController : MonoBehaviour
{
    public const int NUM_BANDS = 12;

    private AudioSource _audioSource;
    private PAnalyzerConfig _analyzerConfig;
    private float[] _samples;
    private FastList<double[]> _spectrumsList;
    private PSpectrumAnalyzer _spectrumAnalyzer;
    private MSpectrumPlotter _spectrumPlotter;
    private FastList<SpectrumInfo> _spectrumDataList;
    private bool _started;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _analyzerConfig = new PAnalyzerConfig(NUM_BANDS);
        _samples = PAudioSampleProvider.getMonoSamples(_audioSource);
        _spectrumsList = PSpectrumProvider.getSpectrums(_samples);
        _spectrumAnalyzer = new PSpectrumAnalyzer(_spectrumsList, _analyzerConfig);

        // Starts analysis.
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
