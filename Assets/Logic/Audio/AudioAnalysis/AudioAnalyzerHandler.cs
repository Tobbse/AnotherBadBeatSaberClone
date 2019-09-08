using SpectrumConfigs;
using System;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;

/**
* Starts the spectrum analysis of a given audio file. Creates and handles the nessecary objects for the initial and post analysis.
* Saves the generated mapping data. Performs a callback when the analysis is done.
**/
public class AudioAnalyzerHandler
{
    private List<AnalyzedSpectrumConfig> _analyzedSpectrumConfigs;
    private TrackConfig _trackConfig;
    private PPostAudioAnalyzer _postAudioAnalyzer;
    private MappingContainer _beatMappingContainer;

    public AudioAnalyzerHandler(TrackConfig trackConfig, List<AnalyzedSpectrumConfig> spectrumDataList, MappingContainer beatMappingContainer)
    {
        _trackConfig = trackConfig;
        _analyzedSpectrumConfigs = spectrumDataList;
        _beatMappingContainer = beatMappingContainer;

        // Bpm is only important for external mappings, because it influences the tempo.
        // If we set it to a fixed value and don't change it, we'll be fine.
        _beatMappingContainer.MappingInfo = new MappingInfo();
        _beatMappingContainer.MappingInfo.Bpm = 1f;
    }

    public List<AnalyzedSpectrumConfig> getAnalyzedSpectrumData()
    {
        return _analyzedSpectrumConfigs;
    }

    public MappingContainer getBeatMappingContainer()
    {
        return _beatMappingContainer;
    }

    public void analyzeSpectrumsList(Action callback)
    {
        List<AnalyzerBandConfig> beatConfigs = _trackConfig.AnalyzerConfigs;

        PeakDetector beatDetector = new PeakDetector(beatConfigs, _analyzedSpectrumConfigs, _trackConfig, _beatMappingContainer);
        beatDetector.analyze();
        _beatMappingContainer = beatDetector.getBeatMappingContainer();

        PostOnsetAudioAnalyzer postAnalyzer = new PostOnsetAudioAnalyzer(_beatMappingContainer);
        postAnalyzer.analyze();
        _beatMappingContainer = postAnalyzer.getBeatMappingContainer();

        callback();
    }
}
