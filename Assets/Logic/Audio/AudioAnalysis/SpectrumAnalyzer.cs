using AudioSpectrumInfo;
using System;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;

namespace PAudioAnalyzer
{
    public class SpectrumAnalyzer
    {
        private List<AnalyzedSpectrumConfig> _analyzedSpectrumConfigs;
        private TrackConfig _trackConfig;
        private PPostAudioAnalyzer _postAudioAnalyzer;
        private MappingContainer _beatMappingContainer;

        public SpectrumAnalyzer(TrackConfig trackConfig, List<AnalyzedSpectrumConfig> spectrumDataList, MappingContainer beatMappingContainer)
        {
            _trackConfig = trackConfig;
            _analyzedSpectrumConfigs = spectrumDataList;
            _beatMappingContainer = beatMappingContainer;

            // Bpm is only important for external mappings, because it influences the tempo. If we set it to a fixed value and don't change it, we'll be fine.
            _beatMappingContainer.mappingInfo = new MappingInfo();
            _beatMappingContainer.mappingInfo.bpm = 1;
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

            OnsetDetector beatDetector = new OnsetDetector(beatConfigs, _analyzedSpectrumConfigs, _trackConfig, _beatMappingContainer);
            beatDetector.analyze();
            _analyzedSpectrumConfigs = beatDetector.getSpectrumDataList();
            callback();
        }
    }

}
