using UnityEngine;
using PSpectrumData;

public class POnsetDetector
{
    private const float DETECTION_MULT_BEFORE = 1.5f;
    private const float DETECTION_MULT_AFTER = 0.5f;
    //private const float BPM_INTERVAL = 4.0f; // 4 seconds is probably too much
    
    private PBeatConfig _beatConfig;
    private FastList<SpectrumInfo> _spectrumData;
    
    private float[] _currentSpectrum;
    private float[] _previousSpectrum;
    private float[] _bpms;
    private float _spectrumsPerBPMInterval;
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

    public POnsetDetector(PBeatConfig beatConfig, FastList<SpectrumInfo> spectrumData, PAnalyzerConfig config)
    {
        _beatConfig = beatConfig;
        _spectrumData = spectrumData;
        _band = _beatConfig.band;
        _clipSampleRate = config.ClipSampleRate;
        _thresholdSize = beatConfig.thresholdSize;

        _currentSpectrum = new float[PSpectrumProvider.NUM_BINS];
        _previousSpectrum = new float[PSpectrumProvider.NUM_BINS];
        _beatCounter = 0;

        float timePerSample = 1f / _clipSampleRate;
        _timePerSpectrum = timePerSample * PSpectrumProvider.SAMPLE_SIZE;

        _minIndex = _thresholdSize / 2;
        _maxIndex = _spectrumData.Count - 1 - (_thresholdSize / 2);
    }

    public void resetIndex()
    {
        _index = 0;
    }

    public void getNextFluxValue()
    {
        _setCurrentSpectrum(_spectrumData[_index].spectrum);
        _spectrumData[_index].bandData[_band].spectralFlux = _calcSpectralFlux();
        _index++;
    }

    public void analyzeNextSpectrum()
    {
        _setCurrentSpectrum(_spectrumData[_index].spectrum);

        _spectrumData[_index].bandData[_band].threshold = _getFluxThreshold();
        _spectrumData[_index].bandData[_band].prunedSpectralFlux = _getPrunedSpectralFlux();

        if (_beatBlockCounter > 0)
        {
            _beatBlockCounter--;
        }
        else if (_isPeak())
        {
            _beatBlockCounter = _beatConfig.beatBlockCounter;
            _spectrumData[_index].hasPeak = true;
            _spectrumData[_index].bandData[_band].isPeak = true;
        }
    _index++;
    }

    public FastList<SpectrumInfo> getSpectrumDataList()
    {
        return _spectrumData;
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
        int firstBin = _beatConfig.startIndex;
        int secondBin = _beatConfig.endIndex;

        for (int i = firstBin; i <= secondBin; i++)
        {
            flux += Mathf.Max(0f, _currentSpectrum[i] - _previousSpectrum[i]);
        }
        return flux;
    }

    private float _getFluxThreshold()
    {
        int start = Mathf.Max(0, _index - _beatConfig.thresholdSize / 2); // Amount of past and future samples for the average
        int end = Mathf.Min(_spectrumData.Count - 1, _index + _beatConfig.thresholdSize / 2);

        float threshold = 0.0f;
        for (int i = start; i <= end; i++)
        {
            threshold += _spectrumData[i].bandData[_band].spectralFlux; // Add spectral flux over the window
        }

        // Threshold is average flux multiplied by sensitivity constant.
        threshold /= (float)(end - start);
        return threshold * _beatConfig.tresholdMult;
    }

    // Pruned Spectral Flux is 0 when the threshhold has not been reached.
    private float _getPrunedSpectralFlux() 
    {
        return Mathf.Max(0f, _spectrumData[_index].bandData[_band].spectralFlux - _spectrumData[_index].bandData[_band].threshold);
    }

    // TODO this could be optimized. Does it make sense to use pruned flux? Change multiplier level?
    private bool _isPeak()
    {
        if (_index < 1 || _index >= _spectrumData.Count - 1)
        {
            return false;
        }

        float previousPruned = _spectrumData[_index - 1].bandData[_band].prunedSpectralFlux;
        float currentPruned = _spectrumData[_index].bandData[_band].prunedSpectralFlux;
        float nextPruned = _spectrumData[_index + 1].bandData[_band].prunedSpectralFlux;

        //return currentPruned > previousPruned;
        return currentPruned > nextPruned;
    }

    /*private float _getAveragedBpm(int currIndex) {
        int indexDist = Mathf.CeilToInt(_spectrumsPerBPMInterval);
        int minIndex = currIndex - indexDist;
        int maxIndex = currIndex - 1;

        if (minIndex <= 0) minIndex = 0;
        if (maxIndex <= 0) maxIndex = 0;
        if (maxIndex >= _spectrumData.Count) maxIndex = _spectrumData.Count - 1;

        int newIndexDist = maxIndex - minIndex;

        float sum = 0.0f;
        for (int i = minIndex; i < maxIndex; i++)
        {
            sum += _bpms[i];
        }
        return sum / newIndexDist;
    }

    private float _getBpm(int beatIndex)
    {
        int beats = 0;
        int indexDist = Mathf.CeilToInt(_spectrumsPerBPMInterval);
        int minIndex = beatIndex - indexDist;
        int maxIndex = beatIndex -1;
        int currentIndex = beatIndex - 1;

        if (minIndex <= 0) minIndex = 0;
        if (maxIndex <= 0) maxIndex = 0;
        if (maxIndex >= _spectrumData.Count) maxIndex = _spectrumData.Count - 1;

        int newIndexDist = maxIndex - minIndex;
        float newBpmInterval = newIndexDist * _timePerSpectrum;

        for (int i = minIndex; i < maxIndex; i++)
        {
            if (_spectrumData[i].hasPeak)
            {
                beats += 1;
            }
        }
        return beats * (60 / newBpmInterval);
    }*/

}
