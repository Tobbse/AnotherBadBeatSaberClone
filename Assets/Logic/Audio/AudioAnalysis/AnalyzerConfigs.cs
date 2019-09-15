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

            /**
             * These configs determine directly how peaks will be detected in the PeakDetector.
             * 
             * Each AnalyzerConfig created represents one frequency band that will be used for peak detection with the set parameters.
             * The startIndex and endIndex represent the range of frequency bins. Assuming a sample rate of 44.100 Hz this results in a
             * per-bin frequency range of 42,982 Hz. The start and end indices have been chosen in a way so that low frequency bass
             * instruments like kickdrums can be detected in the first and higher frequency instruments like snare drums can be detected
             * in the second frequency band. The first one has a frequency range of 0 to 258 Hz, the second one has a range of 1289 to 17193 Hz.
             * Currently the audio analysis is pretty much hardcoded to using exactly 2 bands, so there should always be 2 bands created.
             * 
             * Take a look at the documentation for further information about the audio analysis using the spectral flux approach.
             **/
            _analyzerConfigs.Add(_makeConfig(0, 0, 6, 20, 25, 3.5f * difficultyMult));
            _analyzerConfigs.Add(_makeConfig(1, 30, 400, 20, 25, 2.5f * difficultyMult));
        }

        private AnalyzerBandConfig _makeConfig(int band, int startIndex, int endIndex, int thresholdSize, int beatTime, float tresholdMult)
        {
            return new AnalyzerBandConfig()
            {
                Band = band, StartIndex = startIndex, EndIndex = endIndex, StartFrequency = _hzPerBin * startIndex,
                EndFrequency = _hzPerBin * endIndex, ThresholdSize = thresholdSize, BeatBlockCounter = beatTime, TresholdMult = tresholdMult
            };
        }
    }


    /**
     * Config that contains instructions for the audio analysis, like which bands to use (frequency
     * ranges are defined), or what the threshold levels should be.
     **/
    public class AnalyzerBandConfig
    {
        public int Band { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; } // End index is NOT included
        public int ThresholdSize { get; set; }
        public int BeatBlockCounter { get; set; }
        public float StartFrequency { get; set; }
        public float EndFrequency { get; set; }
        public float TresholdMult { get; set; }
    }
}
