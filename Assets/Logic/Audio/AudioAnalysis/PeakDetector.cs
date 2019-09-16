using Audio.BeatMappingConfigs;
using AudioAnalysis.SpectrumConfigs;
using AudioAnalysis.AudioAnalyzerConfigs;
using AudioProviders;
using System.Collections.Generic;
using System;

namespace AudioAnalysis
{
    /// <summary>
    /// This is the actual Onset Detection class. Audio data is analyzed and beats are found using the Spectral Flux approach.Spectral Flux
    /// is a value that gives information about how big the increase of energy is compared to the last sample.
    /// There are several frequency bands for which this analysis is applied.
    /// First, we iterate over all data to calculate the spectral flux (and the pruned spectral flux, taking the threshold into account).
    /// Second, we iterate over all spectral flux values and try to determine beats by compared the current flux value to a window of
    /// previous flux levels. If a certain threshold is exceeded and some conditions are met, there is a peak.
    ///
    /// See the documentation for further information.
    /// </summary>
    public class PeakDetector
    {
        private static int MAX_BEAT_BLOCK_COUNTER = 5;

        private AnalyzerBandConfig _currentBandCfg;
        private List<AnalyzedSpectrumConfig> _spectrumData;
        private List<AnalyzerBandConfig> _bandConfigs;
        private AnalyzedSpectrumConfig _currentSpectrumCfg;
        private MappingContainer _beatMappingContainer;
        private BeatConfig _currentBeatInfo;
        private ObstacleConfig _lastObstacle;
        private Dictionary<int, float[]> _currentBandSpectrums;
        private Dictionary<int, float[]> _previousBandSpectrums;
        private int _band;
        private int _beatBlockCounter;
        private int _obstacleBlockCounter;
        private int _index;
        private int _maxIndex;
        private int _minIndex;
        private float _lastTime;
        private float _timerPerSpectrum;
        private int _sampleRate;
        private NoteConfig _lastLeftNote;
        private NoteConfig _lastRightNote;
        private Random _rand;

        public PeakDetector(List<AnalyzerBandConfig> bandConfigs, List<AnalyzedSpectrumConfig> spectrumData, TrackConfig config, MappingContainer beatMappingContainer)
        {
            _bandConfigs = bandConfigs;
            _spectrumData = spectrumData;
            _beatMappingContainer = beatMappingContainer;
            _sampleRate = config.ClipSampleRate;

            _currentBandSpectrums = new Dictionary<int, float[]>();
            _previousBandSpectrums = new Dictionary<int, float[]>();
            for (int i = 0; i < _bandConfigs.Count; i++)
            {
                _currentBandSpectrums[i] = new float[SpectrumProvider.NUM_BINS];
                _previousBandSpectrums[i] = new float[SpectrumProvider.NUM_BINS];
            }

            float timePerSample = 1f / _sampleRate;
            _timerPerSpectrum = timePerSample * SpectrumProvider.SAMPLE_SIZE;

            _rand = new Random();
        }

        public void analyze()
        {
            // Preparing flux values.
            for (int i = 0; i <= _maxIndex; i++)
            {
                _prepareFluxValues(i);
            }

            // Calculating threshold, peaks and onsets.
            for (int j = 0; j <= _maxIndex; j++)
            {
                _analyzeNextSpectrum();
            }
        }

        public List<AnalyzedSpectrumConfig> getSpectrumDataList()
        {
            return _spectrumData;
        }

        public MappingContainer getBeatMappingContainer()
        {
            return _beatMappingContainer;
        }

        private void _prepareFluxValues(int index)
        {
            for (int band = 0; band < _bandConfigs.Count; band++)
            {
                _setCurrentSpectrum(index, band);
                _currentSpectrumCfg.BandBeatData[_band].SpectralFlux = _calcSpectralFlux();
            }
        }

        private void _analyzeNextSpectrum()
        {
            if (_index < _minIndex)
            {
                _index++;
                return;
            }

            // Loop over the defined frequency bands.
            for (int i = 0; i < _bandConfigs.Count; i++)
            {
                _setCurrentSpectrum(_index, i);

                _currentBeatInfo.Threshold = _getFluxThreshold();
                _currentBeatInfo.PrunedSpectralFlux = _getPrunedSpectralFlux();

                // Check if obstacle can be created.
                if (_obstacleBlockCounter == 0)
                {
                    if (_rand.Next(0, 100) > 95 && _rand.Next(0, 100) > 95) // TODO define obstacle spawn criteria, currently this is random.
                    {
                        ObstacleConfig obstacleCfg = _createRandomObstacle();
                        _beatMappingContainer.ObstacleData.Add(obstacleCfg);
                        _obstacleBlockCounter = _getNumIndicesFromSeconds(obstacleCfg.Duration + 0.1f);
                        _lastObstacle = obstacleCfg;
                    }
                }

                // Create note when there's a peak.
                if (_beatBlockCounter == 0 && _isPeak())
                {
                    _beatBlockCounter = MAX_BEAT_BLOCK_COUNTER;

                    _currentSpectrumCfg.HasPeak = true;
                    _currentSpectrumCfg.BandBeatData[_band].IsPeak = true;
                    _currentSpectrumCfg.PeakBands.Add(_band);

                    if (_rand.Next(0, 100) > 50)
                    {
                        _beatMappingContainer.EventData.Add(_createRandomEvent());
                    }

                    NoteConfig noteCfg = _createNote();

                    // If there is an obstacle at the same time as the note, position the note next to it.
                    int lineIndex;
                    if (_obstacleBlockCounter > 0)
                    {
                        if (noteCfg.Type == NoteConfig.NOTE_TYPE_LEFT)
                        {
                            lineIndex = _lastObstacle.LineIndex < 2 ? NoteConfig.LINE_INDEX_2 : NoteConfig.LINE_INDEX_0;
                        }
                        else
                        {
                            lineIndex = _lastObstacle.LineIndex < 2 ? NoteConfig.LINE_INDEX_3 : NoteConfig.LINE_INDEX_1;
                        }
                        noteCfg.ObstacleLineIndex = _lastObstacle.LineIndex;
                    }
                    else
                    {
                        lineIndex = noteCfg.Type == NoteConfig.NOTE_TYPE_LEFT ? _rand.Next(NoteConfig.LINE_INDEX_0, NoteConfig.LINE_INDEX_1 + 1) : _rand.Next(NoteConfig.LINE_INDEX_2, NoteConfig.LINE_INDEX_3 + 1);
                    }
                    noteCfg.LineIndex = lineIndex;

                    // The note type depends on the band that is currently handled. The positioning of the note depends on the type and the previous notes.
                    if (noteCfg.Type == NoteConfig.NOTE_TYPE_LEFT)
                    {
                        int min = _lastLeftNote != null ? Math.Max(_lastLeftNote.LineLayer - 1, 0) : NoteConfig.LINE_LAYER_0;
                        int max = _lastLeftNote != null ? Math.Min(_lastLeftNote.LineLayer + 1, 3) : NoteConfig.LINE_LAYER_3;
                        noteCfg.LineLayer = _rand.Next(min, max + 1);

                        // If the last note was close to the current one, position the current one accordingly.
                        if (_lastLeftNote != null && noteCfg.Time < _lastLeftNote.Time + 0.5f)
                        {
                            int minCut = _lastLeftNote != null ? Math.Max(_lastLeftNote.CutDirection - 1, 0) : NoteConfig.CUT_DIRECTION_0;
                            int maxCut = _lastLeftNote != null ? Math.Min(_lastLeftNote.CutDirection + 1, 3) : NoteConfig.CUT_DIRECTION_270;
                            noteCfg.CutDirection = _rand.Next(minCut, maxCut + 1);
                        }
                        else
                        {
                            noteCfg.CutDirection = _rand.Next(NoteConfig.CUT_DIRECTION_0, NoteConfig.CUT_DIRECTION_270 + 1);
                        }
                        _lastLeftNote = noteCfg;
                    }
                    else
                    {
                        int min = _lastRightNote != null ? Math.Max(_lastRightNote.LineLayer - 1, 0) : NoteConfig.LINE_LAYER_0;
                        int max = _lastRightNote != null ? Math.Min(_lastRightNote.LineLayer + 1, 3) : NoteConfig.LINE_LAYER_3;
                        noteCfg.LineLayer = _rand.Next(min, max + 1);

                        // If the last note was close to the current one, position the current one accordingly.
                        if (_lastRightNote != null && noteCfg.Time < _lastRightNote.Time + 0.5f)
                        {
                            int minCut = _lastRightNote != null ? Math.Max(_lastRightNote.CutDirection - 1, 0) : NoteConfig.CUT_DIRECTION_0;
                            int maxCut = _lastRightNote != null ? Math.Min(_lastRightNote.CutDirection + 1, 3) : NoteConfig.CUT_DIRECTION_270;
                            noteCfg.CutDirection = _rand.Next(minCut, maxCut + 1);
                        }
                        else
                        {
                            noteCfg.CutDirection = _rand.Next(NoteConfig.CUT_DIRECTION_0, NoteConfig.CUT_DIRECTION_270 + 1);
                        }
                        _lastRightNote = noteCfg;
                    }

                    _beatMappingContainer.NoteData.Add(noteCfg);
                }
            }
            // Reset the blocking values. Essentially what those counters do is prevent spawning of a block/obstacle for a short amount of time after one has been created.
            if (_obstacleBlockCounter > 0) _obstacleBlockCounter--;
            if (_beatBlockCounter > 0) _beatBlockCounter--;

            _index++;
        }

        // Prepares data for the currently handled spectrum information.
        private void _setCurrentSpectrum(int index, int band)
        {
            _band = band;
            _currentBandCfg = _bandConfigs[_band];
            _minIndex = _currentBandCfg.ThresholdSize;
            _maxIndex = _spectrumData.Count - 1;
            _currentSpectrumCfg = _spectrumData[index];
            _currentBandSpectrums[_band].CopyTo(_previousBandSpectrums[_band], 0);
            _currentSpectrumCfg.Spectrum.CopyTo(_currentBandSpectrums[_band], 0);
            _currentBeatInfo = _currentSpectrumCfg.BandBeatData[_band];
        }

        // Calculates the rectified spectral flux. Aggregates positive changes in spectrum data
        private float _calcSpectralFlux()
        {
            float flux = 0f;
            int firstBin = _currentBandCfg.StartIndex;
            int lastBin = _currentBandCfg.EndIndex;

            for (int i = firstBin; i <= lastBin; i++)
            {
                flux += Math.Max(0f, _currentBandSpectrums[_band][i] - _previousBandSpectrums[_band][i]);
            }
            return flux;
        }

        private float _getFluxThreshold()
        {
            int start = Math.Max(0, _minIndex); // Amount of past and future samples for the average
            int end = Math.Min(_maxIndex, _index + _currentBandCfg.ThresholdSize / 2);

            float threshold = 0.0f;
            for (int i = start; i <= end; i++)
            {
                threshold += _spectrumData[i].BandBeatData[_band].SpectralFlux; // Add spectral flux over the window
            }

            // Threshold is average flux multiplied by sensitivity constant.
            threshold /= (float)(end - start);
            return threshold * _currentBandCfg.TresholdMult;
        }

        // Pruned Spectral Flux is 0 when the threshhold has not been reached.
        private float _getPrunedSpectralFlux()
        {
            return Math.Max(0f, _currentBeatInfo.SpectralFlux - _currentBeatInfo.Threshold);
        }

        // Determines if there is a peak. This is the case when the previous and the next flux levels are both lower.
        private bool _isPeak()
        {
            float previousPrunedFlux = _spectrumData[_index - 1].BandBeatData[_band].PrunedSpectralFlux;
            float currentPrunedFlux = _spectrumData[_index].BandBeatData[_band].PrunedSpectralFlux;

            if (_index + 1 < _bandConfigs.Count)
            {
                float nextPrunedFlux = _spectrumData[_index + 1].BandBeatData[_band].PrunedSpectralFlux;
                return currentPrunedFlux > previousPrunedFlux &&
                    currentPrunedFlux > nextPrunedFlux;
            }
            else
            {
                return currentPrunedFlux > previousPrunedFlux;
            }
        }

        private int _getNumIndicesFromSeconds(float duration)
        {
            return (int)(duration / _timerPerSpectrum);

        }

        private EventConfig _createRandomEvent()
        {
            EventConfig eventCfg = new EventConfig();
            eventCfg.Time = _currentSpectrumCfg.Time;
            eventCfg.Type = _rand.Next(0, 3);
            eventCfg.Value = _rand.Next(0, 3);
            return eventCfg;
        }

        private NoteConfig _createNote()
        {
            NoteConfig noteCfg = new NoteConfig();
            noteCfg.Time = _currentSpectrumCfg.Time;
            noteCfg.Type = _band == 0 ? NoteConfig.NOTE_TYPE_LEFT : NoteConfig.NOTE_TYPE_RIGHT;
            noteCfg.LineLayer = _rand.Next(NoteConfig.LINE_LAYER_0, NoteConfig.LINE_LAYER_3 + 1);
            return noteCfg;
        }

        private ObstacleConfig _createRandomObstacle()
        {
            ObstacleConfig obstacleCfg = new ObstacleConfig();
            obstacleCfg.Time = _currentSpectrumCfg.Time;
            obstacleCfg.Type = _rand.Next(0, 3 + 1);
            obstacleCfg.Width = _rand.Next(2, 4) * 0.3f;
            obstacleCfg.Duration = _rand.Next(15, 35 + 1) * 0.05f;
            if (_band == 0) obstacleCfg.LineIndex = 0;
            else obstacleCfg.LineIndex = 3;
            return obstacleCfg;
        }
    }

}
