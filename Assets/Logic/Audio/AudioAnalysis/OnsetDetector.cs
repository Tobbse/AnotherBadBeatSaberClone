using UnityEngine;
using AudioSpectrumInfo;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;
using System;

public class OnsetDetector
{
    private const float DETECTION_MULT_BEFORE = 1.5f;
    private const float DETECTION_MULT_AFTER = 0.5f;
    
    private AnalyzerBandConfig _analyzerBandConfig;
    private List<AnalyzedSpectrumConfig> _spectrumConfigs;
    private AnalyzedSpectrumConfig _currentSpectrumCfg;
    private MappingContainer _beatMappingContainer;
    private BeatInfo _currentBeatInfo;
    private float[] _currentSpectrum;
    private float[] _previousSpectrum;
    private int _band;
    private int _beatBlockCounter;
    private int _obstacleBlockCounter;
    private int _index;
    private int _maxIndex;
    private int _minIndex;
    private float _lastTime;
    private float _timerPerSpectrum;
    private int _sampleRate;

    public OnsetDetector(AnalyzerBandConfig beatConfig, List<AnalyzedSpectrumConfig> spectrumConfigs, TrackConfig config, MappingContainer beatMappingContainer)
    {
        _analyzerBandConfig = beatConfig;
        _spectrumConfigs = spectrumConfigs;
        _beatMappingContainer = beatMappingContainer;
        _sampleRate = config.ClipSampleRate;

        _currentSpectrum = new float[SpectrumProvider.NUM_BINS];
        _previousSpectrum = new float[SpectrumProvider.NUM_BINS];

        _minIndex = beatConfig.thresholdSize;
        _maxIndex = _spectrumConfigs.Count - 1;
        _band = _analyzerBandConfig.band;

        float timePerSample = 1f / _sampleRate;
        _timerPerSpectrum = timePerSample * SpectrumProvider.SAMPLE_SIZE;
    }

    public void analyze()
    {
        // Preparing flux values.
        for (int i = 0; i <= _maxIndex; i++)
        {
            _setCurrentSpectrum(i);
            _currentSpectrumCfg.beatData[_band].spectralFlux = _calcSpectralFlux();
        }

        // Calculating threshold, peaks and onsets.
        for (int i = 0; i <= _maxIndex; i++)
        {
            analyzeNextSpectrum();
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

    private void analyzeNextSpectrum()
    {
        if (_index < _minIndex)
        {
            _index++;
            return;
        }

        _setCurrentSpectrum(_index);

        _currentBeatInfo.threshold = _getFluxThreshold();
        _currentBeatInfo.prunedSpectralFlux = _getPrunedSpectralFlux();

        if (_obstacleBlockCounter > 0)
        {
            _obstacleBlockCounter--;
        } else
        {
            if (UnityEngine.Random.Range(0, 100) > 90 && UnityEngine.Random.Range(0, 100) > 90) // TODO define obstacle spawn criteria
            {
                ObstacleConfig obstacleCfg = new ObstacleConfig();
                obstacleCfg.time = _currentSpectrumCfg.time;
                if (_band == 0)
                {
                    obstacleCfg.lineIndex = 0;
                } else
                {
                    obstacleCfg.lineIndex = 3;
                }
                
                obstacleCfg.type = UnityEngine.Random.Range(0, 3 + 1);
                obstacleCfg.width = UnityEngine.Random.Range(2, 4) * 0.3f;
                obstacleCfg.duration = UnityEngine.Random.Range(15, 35 + 1) * 0.05f;

                _beatMappingContainer.obstacleData.Add(obstacleCfg);
                _obstacleBlockCounter = getNumIndicesFromSeconds(obstacleCfg.duration + 2);
                int y = 0;
            }
        }

        if (_beatBlockCounter > 0)
        {
            _beatBlockCounter--;
        }
        else if (_isPeak())
        {
            _beatBlockCounter = _analyzerBandConfig.beatBlockCounter;
            _currentSpectrumCfg.hasPeak = true;
            _currentSpectrumCfg.beatData[_band].isPeak = true;
            _currentSpectrumCfg.peakBands.Add(_band);

            if (UnityEngine.Random.Range(0, 100) > 50) {
                EventConfig eventCfg = new EventConfig();
                eventCfg.time = _currentSpectrumCfg.time;
                eventCfg.type = UnityEngine.Random.Range(0, 3);
                eventCfg.value = UnityEngine.Random.Range(0, 3);
                _beatMappingContainer.eventData.Add(eventCfg);
            }
            NoteConfig noteCfg = new NoteConfig();
            noteCfg.time = _currentSpectrumCfg.time;
            noteCfg.type = UnityEngine.Random.Range(NoteConfig.NOTE_TYPE_LEFT, NoteConfig.NOTE_TYPE_RIGHT + 1);
            noteCfg.lineIndex = noteCfg.type == NoteConfig.NOTE_TYPE_LEFT ? UnityEngine.Random.Range(NoteConfig.LINE_INDEX_0, NoteConfig.LINE_INDEX_1 + 1) : UnityEngine.Random.Range(NoteConfig.LINE_INDEX_2, NoteConfig.LINE_INDEX_3 + 1);

            if (_band == 0)
            {
                noteCfg.lineLayer = UnityEngine.Random.Range(NoteConfig.LINE_LAYER_0, NoteConfig.LINE_LAYER_1 + 1);
            }
            if (_band == 1)
            {
                noteCfg.lineLayer = UnityEngine.Random.Range(NoteConfig.LINE_LAYER_2, NoteConfig.LINE_LAYER_3 + 1);
            }
            noteCfg.cutDirection = UnityEngine.Random.Range(NoteConfig.CUT_DIRECTION_TOP, NoteConfig.CUT_DIRECTION_LEFT + 1);
            _beatMappingContainer.noteData.Add(noteCfg);
        }
        _index++;
    }

    private void _setCurrentSpectrum(int index)
    {
        _currentSpectrumCfg = _spectrumConfigs[index];
        _currentSpectrum.CopyTo(_previousSpectrum, 0);
        _currentSpectrumCfg.spectrum.CopyTo(_currentSpectrum, 0);
        _currentBeatInfo = _currentSpectrumCfg.beatData[_band];
    }

    // Calculates the rectified spectral flux. Aggregates positive changes in spectrum data
    private float _calcSpectralFlux()
    {
        float flux = 0f;
        int firstBin = _analyzerBandConfig.startIndex;
        int secondBin = _analyzerBandConfig.endIndex;

        for (int i = firstBin; i <= secondBin; i++)
        {
            flux += Mathf.Max(0f, _currentSpectrum[i] - _previousSpectrum[i]);
        }
        return flux;
    }

    private float _getFluxThreshold()
    {
        int start = Mathf.Max(0, _minIndex); // Amount of past and future samples for the average
        int end = Mathf.Min(_maxIndex, _index + _analyzerBandConfig.thresholdSize / 2);

        float threshold = 0.0f;
        for (int i = start; i <= end; i++)
        {
            threshold += _spectrumConfigs[i].beatData[_band].spectralFlux; // Add spectral flux over the window
        }

        // Threshold is average flux multiplied by sensitivity constant.
        threshold /= (float)(end - start);
        return threshold * _analyzerBandConfig.tresholdMult;
    }

    // Pruned Spectral Flux is 0 when the threshhold has not been reached.
    private float _getPrunedSpectralFlux() 
    {
        return Mathf.Max(0f, _currentBeatInfo.spectralFlux - _currentBeatInfo.threshold);
    }

    // TODO this could be optimized. Does it make sense to use pruned flux? Change multiplier level?
    private bool _isPeak()
    {
        const int LEFT_WINDOW_SIZE = 10;
        const int RIGHT_WINDOW_SIZE = 5;

        if (_index - LEFT_WINDOW_SIZE < _minIndex || _index + RIGHT_WINDOW_SIZE > _maxIndex)
        {
            return false;
        }

        // Assumption: When the current pruned value is > the last && > the next, we have a peak.
        float currentPrunedFlux = _currentBeatInfo.prunedSpectralFlux;
        for (int i = _index - LEFT_WINDOW_SIZE; i <= _index + RIGHT_WINDOW_SIZE; i++)
        {
            if (currentPrunedFlux < _spectrumConfigs[i].beatData[_band].prunedSpectralFlux)
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
