using AudioSpectrumInfo;
using System;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;

namespace PAudioAnalyzer
{
    public class SpectrumAnalyzer
    {
        private List<AnalyzedSpectrumData> _analyzedSpectrumData;
        private List<double[]> _spectrumData;
        private TrackConfig _trackConfig;
        private PPostAudioAnalyzer _postAudioAnalyzer;
        private MappingContainer _beatMappingContainer;

        public SpectrumAnalyzer(List<double[]> spectrumsList, TrackConfig trackConfig, List<AnalyzedSpectrumData> spectrumDataList, MappingContainer beatMappingContainer)
        {
            _spectrumData = spectrumsList;
            _trackConfig = trackConfig;
            _analyzedSpectrumData = spectrumDataList;
            _beatMappingContainer = beatMappingContainer;
        }

        public List<AnalyzedSpectrumData> getAnalyzedSpectrumData()
        {
            return _analyzedSpectrumData;
        }

        public MappingContainer getBeatMappingContainer()
        {
            return _beatMappingContainer;
        }

        public void analyzeSpectrumsList(Action callback)
        {
            List<AnalyzerBandConfig> beatConfigs = _trackConfig.AnalyzerConfigs;

            // Loop over the defined frequency bands.
            for (int i = 0; i < beatConfigs.Count; i++)
            {
                OnsetDetector beatDetector = new OnsetDetector(beatConfigs[i], _analyzedSpectrumData, _trackConfig, _beatMappingContainer);

                // Loop over the spectrums to get spectral flux.
                for (int j = 0; j < _spectrumData.Count; j++)
                {
                    beatDetector.getNextFluxValue();
                }
                beatDetector.resetIndex();

                // Analyze spectrums by looping over the flux values to get the threshold.
                for (int z = 0; z < _spectrumData.Count; z++)
                {
                    beatDetector.analyzeNextSpectrum();
                }
                _analyzedSpectrumData = beatDetector.getSpectrumDataList();
            }
            callback();
        }
    }

}
