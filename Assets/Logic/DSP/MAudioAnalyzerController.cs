using UnityEngine;
using AudioAnalyzerPlain;

public class MAudioAnalyzerController : MonoBehaviour
{
    private AudioSource _audioSource;
    private float[] _samples;
    private FastList<double[]> _spectrumsList;
    private PSpectrumAnalyzer _spectrumAnalyzer;
    private MSpectrumPlotter _spectrumPlotter;
    private FastList<PSpectrumData> _spectrumDataList;
    private bool _analysisStarted;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _samples = PAudioSampleProvider.getMonoSamples(_audioSource);
        _spectrumsList = PSpectrumProvider.getSpectrums(_samples);
        _spectrumAnalyzer = new PSpectrumAnalyzer(_spectrumsList);

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
            _spectrumPlotter = GetComponent<MSpectrumPlotter>();
            _spectrumPlotter.setSpectrumData(_spectrumDataList);

            TimedBlocksGameController timedBlocksGameController = GameObject.Find("TimedBlocksGameController").GetComponent< TimedBlocksGameController>();
            timedBlocksGameController.setSpectrumData(_spectrumDataList);
            enabled = false;
        }
    }

}
