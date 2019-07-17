using UnityEngine;
using PSpectrumData;

namespace AudioAnalyzerPlain
{
    public class PSpectrumAnalyzer
    {
        private const int SPECTRUM_SAMPLE_SIZE = PSpectrumProvider.SAMPLE_SIZE;

        private FastList<SpectrumInfo> _spectrumDataList;
        private FastList<double[]> _spectrumsList;
        private PAnalyzerConfig _config;
        private PPostAudioAnalyzer _postAudioAnalyzer;
        private bool _isReady = false;
        private float _sampleRate = (float)AudioSettings.outputSampleRate;
        private float _timePerSample;
        private float _timePerSpectrumData;

        public PSpectrumAnalyzer(FastList<double[]> spectrumsList, PAnalyzerConfig config)
        {
            _spectrumsList = spectrumsList;
            _config = config;
            _timePerSample = 1f / _sampleRate;
            _timePerSpectrumData = (1.0f / _sampleRate) * SPECTRUM_SAMPLE_SIZE; // Duration per sample * amount of samples per spectrum.

            _initSpectrumData();
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
                PBeatDetector beatDetector = new PBeatDetector(beatConfigs[i], _spectrumDataList);
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

        private void _initSpectrumData()
        {
            _spectrumDataList = new FastList<SpectrumInfo>();

            for (int i = 0; i < _spectrumsList.Count; i++)
            {
                SpectrumInfo data = new SpectrumInfo();
                data.time = _getTimeFromIndex(i);
                data.hasPeak = false;
                data.spectrum = System.Array.ConvertAll(_spectrumsList[i], doubleVal => (float)doubleVal);

                for (int j = 0; j < _config.Bands; j++)
                {
                    SpectrumBandData bandData = new SpectrumBandData();
                    data.bandData.Add(bandData);
                }
                _spectrumDataList.Add(data);
            }
        }

        private float _getTimeFromIndex(int spectrumDataIndex)
        {
            return _timePerSpectrumData * spectrumDataIndex;
        }
    }

}
