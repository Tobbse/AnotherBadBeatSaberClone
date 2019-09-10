using UnityEngine;
using System.Collections.Generic;

/**
 * Classes in this namespace contain parameters for the audio analysis.
 **/
namespace AudioAnalyzerConfigs {

    /**
     * Config that contains track data for the song that is being analyzed.
     **/
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

        public List<AnalyzerBandConfig> AnalyzerConfigs { get { return _analyzerConfigs; } }
        public int Bands { get { return _bands; } }
        public int ClipSampleRate { get { return _clipSampleRate; } }
        public string TrackName { get { return _trackName; } }

        // This defines the behavior of the Onset set Detector. Changing those values will determine what and how many frequency bands will be used.
        // Currently the code is hardcoded to work with 2 frequency bands, mostly considering the positioning and choice of notes.
        // The threshold levels will directly influence the amount of beats detected.
        // This was setup in a generic way but currently has to use exactly 2 bands in order to work.
        private void _createAnalyzerConfigs()
        {
            float difficultyMult = 0f;
            switch (GlobalStorage.getInstance().Difficulty)
            {
                case Game.DIFFICULTY_EASY:
                    difficultyMult = 5.0f;
                    break;

                case Game.DIFFICULTY_NORMAL:
                    difficultyMult = 4.0f;
                    break;

                case Game.DIFFICULTY_HARD:
                    difficultyMult = 3.0f;
                    break;

                case Game.DIFFICULTY_EXPERT:
                    difficultyMult = 2.0f;
                    break;

                case Game.DIFFICULTY_EXPERT_PLUS:
                    difficultyMult = 1.0f;
                    break;

                default:
                    Debug.LogError("UNKNOWN DIFFICULTY LEVEL.");
                    break;
            }

            _analyzerConfigs.Add(_makeConfig(0, 0, 6, 20, 25, 3.5f * difficultyMult));
            _analyzerConfigs.Add(_makeConfig(1, 30, 450, 20, 25, 2.5f * difficultyMult));
        }

        private AnalyzerBandConfig _makeConfig(int band, int startIndex, int endIndex, int thresholdSize, int beatTime, float tresholdMult)
        {
            return new AnalyzerBandConfig(band, startIndex, endIndex, _hzPerBin * startIndex, _hzPerBin * endIndex, thresholdSize, beatTime, tresholdMult);
        }
    }


    /**
     * Config that contains instructions for the audio analysis, like which bands to use (frequency
     * ranges are defined), or what the threshold levels should be.
     **/
    public class AnalyzerBandConfig
    {
        public int _band;
        public int _startIndex;
        public int _endIndex; // End index is NOT included
        public int _thresholdSize;
        public int _beatBlockCounter;
        public float _startFrequency;
        public float _endFrequency;
        public float _tresholdMult;

        public int Band { get { return _band; } set { _band = value; } }
        public int StartIndex { get { return _startIndex; } set { _startIndex = value; } }
        public int EndIndex { get { return _endIndex; } set { _endIndex = value; } }
        public int ThresholdSize { get { return _thresholdSize; } set { _thresholdSize = value; } }
        public int BeatBlockCounter { get { return _beatBlockCounter; } set { _beatBlockCounter = value; } }
        public float StartFrequency { get { return _startFrequency; } set { _startFrequency = value; } }
        public float EndFrequency { get { return _endFrequency; } set { _endFrequency = value; } }
        public float TresholdMult { get { return _tresholdMult; } set { _tresholdMult = value; } }

        public AnalyzerBandConfig(int bandP = 0, int startIndexP = 0, int endIndexP = 0, float startFrequencyP = 0, float endFrequencyP = 0, int thresholdSizeP = 0, int beatTimeP = 0, float tresholdMultP = 0)
        {
            _band = bandP;
            StartIndex = startIndexP;
            EndIndex = endIndexP;
            StartFrequency = startFrequencyP;
            EndFrequency = endFrequencyP;
            ThresholdSize = thresholdSizeP;
            BeatBlockCounter = beatTimeP;
            TresholdMult = tresholdMultP;
        }
    }
}
