using UnityEngine;
using System.Collections.Generic;

namespace AudioAnalyzerConfigs {

    public class TrackConfig
    {
        private List<AnalyzerBandConfig> _analyzerConfigs = new List<AnalyzerBandConfig>();
        private int _bands;
        private int _clipSampleRate;
        private float _sampleRate = (float)AudioSettings.outputSampleRate;
        private float _maxFreq;
        private float _hzPerBin;
        private string _trackName;

        public TrackConfig(int clipSampleRate, string trackName)
        {
            _maxFreq = _sampleRate / 2;
            _hzPerBin = _maxFreq / SpectrumProvider.NUM_BINS;
            _clipSampleRate = clipSampleRate;
            _trackName = trackName;

            _createAnalyzerConfigs();
            _bands = _analyzerConfigs.Count;
        }

        public List<AnalyzerBandConfig> AnalyzerConfigs
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

        // This defines the behavior of the Onset Detector.
        private void _createAnalyzerConfigs() // TODO maybe also pass a parameter for pre- and after pruned flux multipliers to determine beats?
        {
            _analyzerConfigs.Add(_makeConfig(0, 0, 6, 20, 25, 4.5f * GlobalStaticSettings.ONSET_SENSITIVITY_MULT));
            _analyzerConfigs.Add(_makeConfig(1, 30, 450, 20, 25, 6.0f * GlobalStaticSettings.ONSET_SENSITIVITY_MULT));
        }

        private AnalyzerBandConfig _makeConfig(int band, int startIndex, int endIndex, int thresholdSize, int beatTime, float tresholdMult)
        {
            return new AnalyzerBandConfig(band, startIndex, endIndex, _hzPerBin * startIndex, _hzPerBin * endIndex, thresholdSize, beatTime, tresholdMult);
        }
    }


    public class AnalyzerBandConfig
    {
        public int band;
        public int startIndex;
        public int endIndex; // End index is NOT included
        public int thresholdSize;
        public int beatBlockCounter;
        public float startFrequency;
        public float endFrequency;
        public float tresholdMult;

        public AnalyzerBandConfig(int bandP = 0, int startIndexP = 0, int endIndexP = 0, float startFrequencyP = 0, float endFrequencyP = 0, int thresholdSizeP = 0, int beatTimeP = 0, float tresholdMultP = 0)
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
