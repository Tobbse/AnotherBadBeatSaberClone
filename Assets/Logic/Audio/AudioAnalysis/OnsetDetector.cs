using UnityEngine;
using AudioSpectrumInfo;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;

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
    private int _index;
    private int _maxIndex;
    private int _minIndex;
    private float _lastTime;

    public OnsetDetector(AnalyzerBandConfig beatConfig, List<AnalyzedSpectrumConfig> spectrumConfigs, TrackConfig config, MappingContainer beatMappingContainer)
    {
        _analyzerBandConfig = beatConfig;
        _spectrumConfigs = spectrumConfigs;
        _band = _analyzerBandConfig.band;

        _currentSpectrum = new float[SpectrumProvider.NUM_BINS];
        _previousSpectrum = new float[SpectrumProvider.NUM_BINS];

        _beatMappingContainer = beatMappingContainer;

        /*float timePerSample = 1f / config.ClipSampleRate;
        _timePerSpectrum = timePerSample * SpectrumProvider.SAMPLE_SIZE;*/

        _minIndex = beatConfig.thresholdSize;
        _maxIndex = _spectrumConfigs.Count - 1;
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

            if (Random.Range(0, 100) > 50) {
                EventConfig eventCfg = new EventConfig();
                eventCfg.time = _currentSpectrumCfg.time;
                eventCfg.type = Random.Range(0, 3);
                eventCfg.value = Random.Range(0, 3);
                _beatMappingContainer.eventData.Add(eventCfg);
            }
            NoteConfig noteCfg = new NoteConfig();
            noteCfg.time = _currentSpectrumCfg.time;
            noteCfg.type = Random.Range(NoteConfig.NOTE_TYPE_LEFT, NoteConfig.NOTE_TYPE_RIGHT + 1);
            noteCfg.lineIndex = noteCfg.type == NoteConfig.NOTE_TYPE_LEFT ? Random.Range(NoteConfig.LINE_INDEX_0, NoteConfig.LINE_INDEX_1 + 1) : Random.Range(NoteConfig.LINE_INDEX_2, NoteConfig.LINE_INDEX_3 + 1);

            if (_band == 0)
            {
                noteCfg.lineLayer = Random.Range(NoteConfig.LINE_LAYER_0, NoteConfig.LINE_LAYER_1 + 1);
            }
            if (_band == 1)
            {
                noteCfg.lineLayer = Random.Range(NoteConfig.LINE_LAYER_2, NoteConfig.LINE_LAYER_3 + 1);
            }
            noteCfg.cutDirection = Random.Range(NoteConfig.CUT_DIRECTION_TOP, NoteConfig.CUT_DIRECTION_LEFT + 1);
            _beatMappingContainer.noteData.Add(noteCfg);

            if (Random.Range(0, 100) > 95)
            {
                ObstacleConfig obstacleCfg = new ObstacleConfig();
                obstacleCfg.time = _currentSpectrumCfg.time;
                obstacleCfg.lineIndex = Random.Range(0, 3);
                obstacleCfg.type = Random.Range(0, 3);
                obstacleCfg.width = Random.Range(1, 3) * 0.5f;
                obstacleCfg.duration = Random.Range(1, 4);
                _beatMappingContainer.obstacleData.Add(obstacleCfg);
            }
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
        const int HALF_WINDOW_SIZE = 5;
        if (_index + HALF_WINDOW_SIZE > _maxIndex || _index - HALF_WINDOW_SIZE < _minIndex)
        {
            return false;
        }

        // Assumption: When the current pruned value is > the last && > the next, we have a peak.
        float currentPrunedFlux = _currentBeatInfo.prunedSpectralFlux;
        for (int i = _index - HALF_WINDOW_SIZE; i <= _index + HALF_WINDOW_SIZE; i++)
        {
            if (currentPrunedFlux < _spectrumConfigs[i].beatData[_band].prunedSpectralFlux)
            {
                return false;
            }
        }
        return true;
    }

}
