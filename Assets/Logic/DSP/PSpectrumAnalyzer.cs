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

            for (int i = 0; i < beatConfigs.Count; i++)
            {
                PBeatDetector beatDetector = new PBeatDetector(beatConfigs[i], _spectrumDataList, _config);
                for (int j = 0; j < _spectrumsList.Count; j++)
                {
                    beatDetector.getFluxValues();
                }
                beatDetector.resetIndex();

                for (int j = 0; j < _spectrumsList.Count; j++)
                {
                    beatDetector.analyzeSpectrum();
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
