using PSpectrumData;

namespace AudioAnalyzerPlain
{
    public class PSpectrumAnalyzer
    {
        private FastList<SpectrumInfo> _spectrumDataList;
        private FastList<double[]> _spectrumsList;
        private PAnalyzerConfig _config;
        private PPostAudioAnalyzer _postAudioAnalyzer;
        private bool _isReady = false;

        public PSpectrumAnalyzer(FastList<double[]> spectrumsList, PAnalyzerConfig config, FastList<SpectrumInfo> spectrumDataList)
        {
            _spectrumsList = spectrumsList;
            _config = config;
            _spectrumDataList = spectrumDataList;
        }

        public FastList<SpectrumInfo> getSpectrumDataList()
        {
            return _spectrumDataList;
        }

        public void analyzeSpectrumsList()
        {
            FastList<PBeatConfig> beatConfigs = _config.BeatConfigs;

            // Loop over the defined frequency bands.
            for (int i = 0; i < beatConfigs.Count; i++)
            {
                POnsetDetector beatDetector = new POnsetDetector(beatConfigs[i], _spectrumDataList, _config);

                // Loop over the spectrums to get spectral flux.
                for (int j = 0; j < _spectrumsList.Count; j++)
                {
                    beatDetector.getNextFluxValue();
                }
                beatDetector.resetIndex();

                // Analyze spectrums by looping over the flux values to get the threshold.
                for (int z = 0; z < _spectrumsList.Count; z++)
                {
                    beatDetector.analyzeNextSpectrum();
                }
                _spectrumDataList = beatDetector.getSpectrumDataList();
            }

            //_postAudioAnalyzer = new PPostAudioAnalyzer(_spectrumDataList);
            //_spectrumDataList = _postAudioAnalyzer.findExtraBeats();

            _isReady = true;
        }

        public bool isReady()
        {
            return _isReady;
        }
    }

}
