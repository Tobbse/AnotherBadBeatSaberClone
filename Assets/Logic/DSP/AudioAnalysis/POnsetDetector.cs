using UnityEngine;
using PSpectrumInfo;
using PAnalyzerConfigs;
using PMappingConfigs;

public class POnsetDetector
{
    private const float DETECTION_MULT_BEFORE = 1.5f;
    private const float DETECTION_MULT_AFTER = 0.5f;
    
    private PAnalyzerBandConfig _analyzerBandConfig;
    private FastList<PAnalyzedSpectrumData> _analyzedSpectrumData;
    private PMappingContainer _beatMappingContainer;
    private float[] _currentSpectrum;
    private float[] _previousSpectrum;
    private float _timePerSpectrum;
    private int _thresholdSize;
    private int _band;
    private int _processed;
    private int _clipSampleRate;
    private int _beatBlockCounter;
    private int _index;
    private int _beatCounter;
    private int _maxIndex;
    private int _minIndex;

    public POnsetDetector(PAnalyzerBandConfig beatConfig, FastList<PAnalyzedSpectrumData> spectrumData, TrackConfig config, PMappingContainer beatMappingContainer)
    {
        _analyzerBandConfig = beatConfig;
        _analyzedSpectrumData = spectrumData;
        _band = _analyzerBandConfig.band;
        _clipSampleRate = config.ClipSampleRate;
        _thresholdSize = beatConfig.thresholdSize;

        _currentSpectrum = new float[PSpectrumProvider.NUM_BINS];
        _previousSpectrum = new float[PSpectrumProvider.NUM_BINS];
        _beatCounter = 0;

        _beatMappingContainer = beatMappingContainer;

        float timePerSample = 1f / _clipSampleRate;
        _timePerSpectrum = timePerSample * PSpectrumProvider.SAMPLE_SIZE;

        _minIndex = _thresholdSize / 2;
        _maxIndex = _analyzedSpectrumData.Count - 1 - (_thresholdSize / 2);
    }

    public void resetIndex()
    {
        _index = 0;
    }

    public void getNextFluxValue()
    {
        _setCurrentSpectrum(_analyzedSpectrumData[_index].spectrum);
        _analyzedSpectrumData[_index].beatData[_band].spectralFlux = _calcSpectralFlux();
        _index++;
    }

    public void analyzeNextSpectrum()
    {
        _setCurrentSpectrum(_analyzedSpectrumData[_index].spectrum);

        _analyzedSpectrumData[_index].beatData[_band].threshold = _getFluxThreshold();
        _analyzedSpectrumData[_index].beatData[_band].prunedSpectralFlux = _getPrunedSpectralFlux();

        if (_beatBlockCounter > 0)
        {
            _beatBlockCounter--;
        }
        else if (_isPeak())
        {
            _beatBlockCounter = _analyzerBandConfig.beatBlockCounter;
            _analyzedSpectrumData[_index].hasPeak = true;
            _analyzedSpectrumData[_index].beatData[_band].isPeak = true;
            _analyzedSpectrumData[_index].peakBands.Add(_band);

            float time = _analyzedSpectrumData[_index].time;
            if (Random.Range(0, 100) > 50) {
                PEventConfig eventCfg = new PEventConfig();
                eventCfg.time = time;
                eventCfg.type = Random.Range(0, 3);
                eventCfg.value = Random.Range(0, 3);
                _beatMappingContainer.eventData.Add(eventCfg);
            }

            PNoteConfig noteCfg = new PNoteConfig();
            noteCfg.time = time;
            noteCfg.type = Random.Range(PNoteConfig.NOTE_TYPE_LEFT, PNoteConfig.NOTE_TYPE_RIGHT + 1);
            noteCfg.lineIndex = noteCfg.type == PNoteConfig.NOTE_TYPE_LEFT ? Random.Range(PNoteConfig.LINE_INDEX_0, PNoteConfig.LINE_INDEX_1 + 1) : Random.Range(PNoteConfig.LINE_INDEX_2, PNoteConfig.LINE_INDEX_3 + 1);
            noteCfg.lineLayer = Random.Range(PNoteConfig.LINE_LAYER_0, PNoteConfig.LINE_LAYER_3 + 1);
            noteCfg.cutDirection = Random.Range(PNoteConfig.CUT_DIRECTION_TOP, PNoteConfig.CUT_DIRECTION_LEFT + 1);
            _beatMappingContainer.noteData.Add(noteCfg);

            if (Random.Range(0, 100) > 80)
            {
                PObstacleConfig obstacleCfg = new PObstacleConfig();
                obstacleCfg.time = time;
                obstacleCfg.lineIndex = Random.Range(0, 3);
                obstacleCfg.type = Random.Range(0, 3);
                obstacleCfg.width = Random.Range(1, 4);
                obstacleCfg.duration = Random.Range(1, 4);
                _beatMappingContainer.obstacleData.Add(obstacleCfg);
            }
        }
        _index++;
    }

    public FastList<PAnalyzedSpectrumData> getSpectrumDataList()
    {
        return _analyzedSpectrumData;
    }

    public PMappingContainer getBeatMappingContainer()
    {
        return _beatMappingContainer;
    }

    private void _setCurrentSpectrum(float[] spectrum)
    {
        _currentSpectrum.CopyTo(_previousSpectrum, 0);
        spectrum.CopyTo(_currentSpectrum, 0);
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
        int start = Mathf.Max(0, _index - _analyzerBandConfig.thresholdSize / 2); // Amount of past and future samples for the average
        int end = Mathf.Min(_analyzedSpectrumData.Count - 1, _index + _analyzerBandConfig.thresholdSize / 2);

        float threshold = 0.0f;
        for (int i = start; i <= end; i++)
        {
            threshold += _analyzedSpectrumData[i].beatData[_band].spectralFlux; // Add spectral flux over the window
        }

        // Threshold is average flux multiplied by sensitivity constant.
        threshold /= (float)(end - start);
        return threshold * _analyzerBandConfig.tresholdMult;
    }

    // Pruned Spectral Flux is 0 when the threshhold has not been reached.
    private float _getPrunedSpectralFlux() 
    {
        return Mathf.Max(0f, _analyzedSpectrumData[_index].beatData[_band].spectralFlux - _analyzedSpectrumData[_index].beatData[_band].threshold);
    }

    // TODO this could be optimized. Does it make sense to use pruned flux? Change multiplier level?
    private bool _isPeak()
    {
        if (_index < 1 || _index >= _analyzedSpectrumData.Count - 1)
        {
            return false;
        }

        float previousPruned = _analyzedSpectrumData[_index - 1].beatData[_band].prunedSpectralFlux;
        float currentPruned = _analyzedSpectrumData[_index].beatData[_band].prunedSpectralFlux;
        float nextPruned = _analyzedSpectrumData[_index + 1].beatData[_band].prunedSpectralFlux;

        // TÒDO figure out what is best here.
        //return currentPruned > previousPruned;
        return currentPruned > nextPruned;
    }
}
