using UnityEngine;
using SpectrumConfigs;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;


/**
 * Audio analysis controller for a test scene, that is not needed anymore.
 * That scene was needed to visualize the audio analysis when developing it.
 * 
 * TODO: Check if this still works.
 **/
public class PlotDemoAudioAnalyzerController : MonoBehaviour
{
    private TrackConfig _analyzerConfig;
    private AudioAnalyzerHandler _spectrumAnalyzer;
    private SpectrumPlotter _spectrumPlotter;
    private List<AnalyzedSpectrumConfig> _spectrumData;
    private bool _started;

    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        AudioClip clip = audioSource.clip;
        _analyzerConfig = new TrackConfig(audioSource.clip.frequency, audioSource.clip.name);
        SpectrumProvider audioProvider = new SpectrumProvider(_analyzerConfig.ClipSampleRate);

        float[] samples = AudioSampleProvider.getMonoSamples(AudioSampleProvider.getSamples(clip), clip.channels);
        List<double[]> spectrumsList = audioProvider.getSpectrums(samples);
        _spectrumData = audioProvider.getSpectrumConfigs(spectrumsList, _analyzerConfig.Bands);

        _spectrumAnalyzer = new AudioAnalyzerHandler(_analyzerConfig, _spectrumData, new MappingContainer());
        _spectrumAnalyzer.analyzeSpectrumsList(done);
    }

    private void done()
    {
        enabled = false;
        _spectrumData = _spectrumAnalyzer.getAnalyzedSpectrumData();
        _spectrumPlotter = GetComponent<SpectrumPlotter>();
        _spectrumPlotter.setDataAndStart(_spectrumData, SpectrumPlotter.SHOW_PEAKS);
    }
}
