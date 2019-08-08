using PSpectrumInfo;
using System;
using PAnalyzerConfigs;
using PMappingConfigs;

namespace PAudioAnalyzer
{
    public class PSpectrumAnalyzer
    {
        private FastList<PAnalyzedSpectrumData> _analyzedSpectrumData;
        private FastList<double[]> _spectrumData;
        private TrackConfig _trackConfig;
        private PPostAudioAnalyzer _postAudioAnalyzer;
        private PMappingContainer _beatMappingContainer;

        public PSpectrumAnalyzer(FastList<double[]> spectrumsList, TrackConfig trackConfig, FastList<PAnalyzedSpectrumData> spectrumDataList, PMappingContainer beatMappingContainer)
        {
            _spectrumData = spectrumsList;
            _trackConfig = trackConfig;
            _analyzedSpectrumData = spectrumDataList;
            _beatMappingContainer = beatMappingContainer;
        }

        public FastList<PAnalyzedSpectrumData> getAnalyzedSpectrumData()
        {
            return _analyzedSpectrumData;
        }

        public PMappingContainer getBeatMappingContainer()
        {
            return _beatMappingContainer;
        }

        public void analyzeSpectrumsList(Action callback)
        {
            FastList<PAnalyzerBandConfig> beatConfigs = _trackConfig.AnalyzerConfigs;

            // Loop over the defined frequency bands.
            for (int i = 0; i < beatConfigs.Count; i++)
            {
                POnsetDetector beatDetector = new POnsetDetector(beatConfigs[i], _analyzedSpectrumData, _trackConfig, _beatMappingContainer);

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
