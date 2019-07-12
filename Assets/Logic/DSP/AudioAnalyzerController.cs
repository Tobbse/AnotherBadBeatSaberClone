using UnityEngine;
using AudioAnalyzerPlain;

public class AudioAnalyzerController : MonoBehaviour
{
    private AudioSource _audioSource;
    private float[] _samples;
    private FastList<double[]> _spectrumsList;
    private SpectrumAnalyzer _spectrumAnalyzer;
    private SpectrumPlotter _spectrumPlotter;
    private FastList<SpectrumData> _spectrumDataList;
    private bool _analysisStarted;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _samples = AudioSampleProviderPlain.getMonoSamples(_audioSource);
        _spectrumsList = SpectrumProviderPlain.getSpectrums(_samples);
        _spectrumAnalyzer = new SpectrumAnalyzer(_spectrumsList);

        // Starts analysis.
        _spectrumAnalyzer.analyzeSpectrumsList();
    }

    // Update is called once per frame
    void Update()
    {
        if (_spectrumAnalyzer == null) return;

        if (!_analysisStarted && _spectrumAnalyzer.isReady())
        {
            _analysisStarted = true;
            _spectrumDataList = _spectrumAnalyzer.getSpectrumDataList();
            _spectrumPlotter = GetComponent<SpectrumPlotter>();
            _spectrumPlotter.setSpectrumData(_spectrumDataList);

            TimedBlocksGameController timedBlocksGameController = GameObject.Find("TimedBlocksGameController").GetComponent< TimedBlocksGameController>();
            timedBlocksGameController.setSpectrumData(_spectrumDataList);
            enabled = false;
        }
    }

}
