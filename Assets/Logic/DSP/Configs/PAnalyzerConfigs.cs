using UnityEngine;
namespace PAnalyzerConfigs {

    public class TrackConfig
    {
        private FastList<PAnalyzerBandConfig> _analyzerConfigs = new FastList<PAnalyzerBandConfig>();
        private int _bands;
        private int _clipSampleRate;
        private float _sampleRate = (float)AudioSettings.outputSampleRate;
        private float _maxFreq;
        private float _hzPerBin;
        private string _trackName;

        public TrackConfig(int clipSampleRate, string trackName)
        {
            _maxFreq = _sampleRate / 2;
            _hzPerBin = _maxFreq / PSpectrumProvider.NUM_BINS;
            _clipSampleRate = clipSampleRate;
            _trackName = trackName;

            _createAnalyzerConfigs();
            _bands = _analyzerConfigs.Count;
        }

        public FastList<PAnalyzerBandConfig> AnalyzerConfigs
        {
            get { return _analyzerConfigs; }
        }

        public int Bands
        {
            get { return _bands; }
        }

        public int ClipSampleRate
        {
            get { return _clipSampleRate; }
        }

        public string TrackName
        {
            get { return _trackName; }
        }

        private void _createAnalyzerConfigs() // TODO maybe also pass a parameter for pre- and after pruned flux multipliers to determine beats?
        {
            _analyzerConfigs.Add(_makeConfig(0, 0, 6, 20, 5, 4.0f));
            _analyzerConfigs.Add(_makeConfig(1, 30, 450, 20, 10, 2.5f));
            //_analyzerConfigs.Add(_makeConfig(0, 0, 6, 20, 5, 5.0f));
            //_analyzerConfigs.Add(_makeConfig(1, 30, 450, 20, 10, 3.5f));
        }

        private PAnalyzerBandConfig _makeConfig(int band, int startIndex, int endIndex, int thresholdSize, int beatTime, float tresholdMult)
        {
            return new PAnalyzerBandConfig(band, startIndex, endIndex, _hzPerBin * startIndex, _hzPerBin * endIndex, thresholdSize, beatTime, tresholdMult);
        }
    }


    public class PAnalyzerBandConfig
    {
        public int band;
        public int startIndex;
        public int endIndex; // End index is NOT included
        public int thresholdSize;
        public int beatBlockCounter;
        public float startFrequency;
        public float endFrequency;
        public float tresholdMult;

        public PAnalyzerBandConfig(int bandP = 0, int startIndexP = 0, int endIndexP = 0, float startFrequencyP = 0, float endFrequencyP = 0, int thresholdSizeP = 0, int beatTimeP = 0, float tresholdMultP = 0)
        {
            band = bandP;
            startIndex = startIndexP;
            endIndex = endIndexP;
            startFrequency = startFrequencyP;
            endFrequency = endFrequencyP;
            thresholdSize = thresholdSizeP;
            beatBlockCounter = beatTimeP;
            tresholdMult = tresholdMultP;
        }
    }

}
