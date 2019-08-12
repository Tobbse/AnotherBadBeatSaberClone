using PSpectrumInfo;
using System;
using AnalyzerConfigs;
using MappingConfigs;

namespace PAudioAnalyzer
{
    public class SpectrumAnalyzer
    {
        private FastList<AnalyzedSpectrumData> _analyzedSpectrumData;
        private FastList<double[]> _spectrumData;
        private TrackConfig _trackConfig;
        private PPostAudioAnalyzer _postAudioAnalyzer;
        private MappingContainer _beatMappingContainer;

        public SpectrumAnalyzer(FastList<double[]> spectrumsList, TrackConfig trackConfig, FastList<AnalyzedSpectrumData> spectrumDataList, MappingContainer beatMappingContainer)
        {
            _spectrumData = spectrumsList;
            _trackConfig = trackConfig;
            _analyzedSpectrumData = spectrumDataList;
            _beatMappingContainer = beatMappingContainer;
        }

        public FastList<AnalyzedSpectrumData> getAnalyzedSpectrumData()
        {
            return _analyzedSpectrumData;
        }

        public MappingContainer getBeatMappingContainer()
        {
            return _beatMappingContainer;
        }

        public void analyzeSpectrumsList(Action callback)
        {
            FastList<AnalyzerBandConfig> beatConfigs = _trackConfig.AnalyzerConfigs;

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
