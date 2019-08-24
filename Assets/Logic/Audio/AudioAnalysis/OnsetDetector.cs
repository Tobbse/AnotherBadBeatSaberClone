using UnityEngine;
using AudioSpectrumInfo;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;
using System;

public class OnsetDetector
{
    private AnalyzerBandConfig _currentBandCfg;
    private List<AnalyzedSpectrumConfig> _spectrumConfigs;
    private List<AnalyzerBandConfig> _bandConfigs;
    private AnalyzedSpectrumConfig _currentSpectrumCfg;
    private MappingContainer _beatMappingContainer;
    private BeatInfo _currentBeatInfo;
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

    public OnsetDetector(List<AnalyzerBandConfig> bandConfigs, List<AnalyzedSpectrumConfig> spectrumConfigs, TrackConfig config, MappingContainer beatMappingContainer)
    {
        _bandConfigs = bandConfigs;
        _spectrumConfigs = spectrumConfigs;
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
        return _spectrumConfigs;
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
            _currentSpectrumCfg.bandBeatData[_band].spectralFlux = _calcSpectralFlux();
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

            _currentBeatInfo.threshold = _getFluxThreshold();
            _currentBeatInfo.prunedSpectralFlux = _getPrunedSpectralFlux();

            if (_obstacleBlockCounter == 0)
            {
                if (UnityEngine.Random.Range(0, 100) > 95 && UnityEngine.Random.Range(0, 100) > 95) // TODO define obstacle spawn criteria
                {
                    ObstacleConfig obstacleCfg = new ObstacleConfig();
                    obstacleCfg.time = _currentSpectrumCfg.time;
                    obstacleCfg.type = UnityEngine.Random.Range(0, 3 + 1);
                    obstacleCfg.width = UnityEngine.Random.Range(2, 4) * 0.3f;
                    obstacleCfg.duration = UnityEngine.Random.Range(15, 35 + 1) * 0.05f;

                    if (_band == 0) obstacleCfg.lineIndex = 0;
                    else obstacleCfg.lineIndex = 3;

                    _beatMappingContainer.obstacleData.Add(obstacleCfg);
                    _obstacleBlockCounter = getNumIndicesFromSeconds(obstacleCfg.duration);
                    _lastObstacle = obstacleCfg;
                }
            }
            
            if (_beatBlockCounter == 0 && _isPeak())
            {
                //_beatBlockCounter = _currentBandCfg.beatBlockCounter;
                _beatBlockCounter = 20;

                _currentSpectrumCfg.hasPeak = true;
                _currentSpectrumCfg.bandBeatData[_band].isPeak = true;
                _currentSpectrumCfg.peakBands.Add(_band);

                if (UnityEngine.Random.Range(0, 100) > 50)
                {
                    EventConfig eventCfg = new EventConfig();
                    eventCfg.time = _currentSpectrumCfg.time;
                    eventCfg.type = UnityEngine.Random.Range(0, 3);
                    eventCfg.value = UnityEngine.Random.Range(0, 3);
                    _beatMappingContainer.eventData.Add(eventCfg);
                }
                NoteConfig noteCfg = new NoteConfig();
                noteCfg.time = _currentSpectrumCfg.time;
                noteCfg.type = _band == 0 ? NoteConfig.NOTE_TYPE_LEFT : NoteConfig.NOTE_TYPE_RIGHT;
                noteCfg.lineLayer = UnityEngine.Random.Range(NoteConfig.LINE_LAYER_0, NoteConfig.LINE_LAYER_3 + 1);

                int lineIndex;
                if (_obstacleBlockCounter > 0)
                {
                    if (noteCfg.type == NoteConfig.NOTE_TYPE_LEFT)
                    {
                        lineIndex = _lastObstacle.lineIndex < 2 ? NoteConfig.LINE_LAYER_2 : NoteConfig.LINE_LAYER_0;
                    } else
                    {
                        lineIndex = _lastObstacle.lineIndex < 2 ? NoteConfig.LINE_LAYER_3 : NoteConfig.LINE_LAYER_1;
                    }
                } else
                {
                    lineIndex = noteCfg.type == NoteConfig.NOTE_TYPE_LEFT ? UnityEngine.Random.Range(NoteConfig.LINE_INDEX_0, NoteConfig.LINE_INDEX_1 + 1) : UnityEngine.Random.Range(NoteConfig.LINE_INDEX_2, NoteConfig.LINE_INDEX_3 + 1);
                }
                noteCfg.lineIndex = lineIndex;


                if (noteCfg.type == NoteConfig.NOTE_TYPE_LEFT)
                {
                    int min = _lastLeftNote != null ? Math.Max(_lastLeftNote.lineLayer - 1, 0) : NoteConfig.LINE_LAYER_0;
                    int max = _lastLeftNote != null ? Math.Min(_lastLeftNote.lineLayer + 1, 3) : NoteConfig.LINE_LAYER_3;
                    noteCfg.lineLayer = UnityEngine.Random.Range(min, max + 1);

                    if (_lastLeftNote != null && noteCfg.time < _lastLeftNote.time + 0.5f)
                    {
                        int minCut = _lastLeftNote != null ? Math.Max(_lastLeftNote.cutDirection - 1, 0) : NoteConfig.CUT_DIRECTION_0;
                        int maxCut = _lastLeftNote != null ? Math.Min(_lastLeftNote.cutDirection + 1, 3) : NoteConfig.CUT_DIRECTION_270;
                        noteCfg.cutDirection = UnityEngine.Random.Range(minCut, maxCut + 1);
                    } else
                    {
                        noteCfg.cutDirection = UnityEngine.Random.Range(NoteConfig.CUT_DIRECTION_0, NoteConfig.CUT_DIRECTION_270 + 1);
                    }
                    _lastLeftNote = noteCfg;
                } else
                {
                    int min = _lastRightNote != null ? Math.Max(_lastRightNote.lineLayer - 1, 0) : NoteConfig.LINE_LAYER_0;
                    int max = _lastRightNote != null ? Math.Min(_lastRightNote.lineLayer + 1, 3) : NoteConfig.LINE_LAYER_3;
                    noteCfg.lineLayer = UnityEngine.Random.Range(min, max + 1);

                    if (_lastRightNote != null && noteCfg.time < _lastRightNote.time + 0.5f)
                    {
                        int minCut = _lastRightNote != null ? Math.Max(_lastRightNote.cutDirection - 1, 0) : NoteConfig.CUT_DIRECTION_0;
                        int maxCut = _lastRightNote != null ? Math.Min(_lastRightNote.cutDirection + 1, 3) : NoteConfig.CUT_DIRECTION_270;
                        noteCfg.cutDirection = UnityEngine.Random.Range(minCut, maxCut + 1);
                    }
                    else
                    {
                        noteCfg.cutDirection = UnityEngine.Random.Range(NoteConfig.CUT_DIRECTION_0, NoteConfig.CUT_DIRECTION_270 + 1);
                    }
                    _lastRightNote = noteCfg;
                }
                
                _beatMappingContainer.noteData.Add(noteCfg);
            }
        }
        if (_obstacleBlockCounter > 0) _obstacleBlockCounter--;
        if (_beatBlockCounter > 0) _beatBlockCounter--;

        _index++;
    }

    private void _setCurrentSpectrum(int index, int band)
    {
        _band = band;
        _currentBandCfg = _bandConfigs[_band];
        _minIndex = _currentBandCfg.thresholdSize;
        _maxIndex = _spectrumConfigs.Count - 1;
        _currentSpectrumCfg = _spectrumConfigs[index];
        _currentBandSpectrums[_band].CopyTo(_previousBandSpectrums[_band], 0);
        _currentSpectrumCfg.spectrum.CopyTo(_currentBandSpectrums[_band], 0);
        _currentBeatInfo = _currentSpectrumCfg.bandBeatData[_band];
    }

    // Calculates the rectified spectral flux. Aggregates positive changes in spectrum data
    private float _calcSpectralFlux()
    {
        float flux = 0f;
        int firstBin = _currentBandCfg.startIndex;
        int secondBin = _currentBandCfg.endIndex;

        for (int i = firstBin; i <= secondBin; i++)
        {
            flux += Mathf.Max(0f, _currentBandSpectrums[_band][i] - _previousBandSpectrums[_band][i]);
        }
        return flux;
    }

    private float _getFluxThreshold()
    {
        int start = Mathf.Max(0, _minIndex); // Amount of past and future samples for the average
        int end = Mathf.Min(_maxIndex, _index + _currentBandCfg.thresholdSize / 2);

        float threshold = 0.0f;
        for (int i = start; i <= end; i++)
        {
            threshold += _spectrumConfigs[i].bandBeatData[_band].spectralFlux; // Add spectral flux over the window
        }

        // Threshold is average flux multiplied by sensitivity constant.
        threshold /= (float)(end - start);
        return threshold * _currentBandCfg.tresholdMult;
    }

    // Pruned Spectral Flux is 0 when the threshhold has not been reached.
    private float _getPrunedSpectralFlux() 
    {
        return Mathf.Max(0f, _currentBeatInfo.spectralFlux - _currentBeatInfo.threshold);
    }

    // TODO this could be optimized. Does it make sense to use pruned flux? Change multiplier level?
    private bool _isPeak()
    {
        const int LEFT_WINDOW_SIZE = 20;
        const int RIGHT_WINDOW_SIZE = 10;

        if (_index - LEFT_WINDOW_SIZE < _minIndex || _index + RIGHT_WINDOW_SIZE > _maxIndex)
        {
            return false;
        }

        // Assumption: When the current pruned value is > the last && > the next, we have a peak.
        float currentPrunedFlux = _currentBeatInfo.prunedSpectralFlux;
        for (int i = _index - LEFT_WINDOW_SIZE; i <= _index + RIGHT_WINDOW_SIZE; i++)
        {
            if (currentPrunedFlux < _spectrumConfigs[i].bandBeatData[_band].prunedSpectralFlux)
            {
                return false;
            }
        }
        return true;
    }

    private int getNumIndicesFromSeconds(float duration)
    {
        return Mathf.CeilToInt(duration / _timerPerSpectrum);
    }
}
