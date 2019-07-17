using UnityEngine;
using PSpectrumData;

public class PBeatDetector
{
    private const int SPECTRUM_SAMPLE_SIZE = PSpectrumProvider.SAMPLE_SIZE;
    private const int NUM_BINS = PSpectrumProvider.NUM_BINS;
    private const int THRESHOLD_SIZE = 30;
    private const float THRESHOLD_MULTIPLIER = 12.0f;
    private const float PEAK_DETECTION_MULTIPLIER = 6.0f;
    private const float BPM_INTERVAL = 4.0f; // 4 seconds is probably too much

    private PBeatConfig _beatConfig;
    private FastList<SpectrumInfo> _spectrumData;
    private float[] _currentSpectrum;
    private float[] _previousSpectrum;
    private float[] _bpms;
    private float _sampleRate = (float)AudioSettings.outputSampleRate;
    private float _spectrumsPerBPMInterval;
    private float _timePerSpectrum;
    private int _currentIndex;
    private int _band;
    private int _processed = 0;

    public PBeatDetector(PBeatConfig beatConfig, FastList<SpectrumInfo> spectrumData)
    {
        _beatConfig = beatConfig;
        _spectrumData = spectrumData;
        //_currentIndex = THRESHOLD_SIZE / 2;
        _currentIndex = 0;
        _band = _beatConfig.band;

        _currentSpectrum = new float[NUM_BINS];
        _previousSpectrum = new float[NUM_BINS];
        _bpms = new float[NUM_BINS];

        float timePerSample = 1f / _sampleRate;
        _timePerSpectrum = timePerSample * SPECTRUM_SAMPLE_SIZE;
        _spectrumsPerBPMInterval = BPM_INTERVAL / (timePerSample * SPECTRUM_SAMPLE_SIZE); // BPM-Interval divided by TimePerSpectrum
    }

    public void analyzeSpectrum()
    {
        Debug.Log(_currentIndex);
        //_currentIndex++;
        //return;
        _setCurrentSpectrum(_spectrumData[_currentIndex].spectrum);

        _spectrumData[_currentIndex].bandData[_band].spectralFlux = _calcSpectralFlux();

        //_bpms[_currentIndex] = _getBpm(_currentIndex); // This actually makes no sense right now. We calculate BPM for all bands and create an average every time.

        // The amount of samples we've collected so far is high enough to be able to detect a peak.
        if (_processed >= THRESHOLD_SIZE)
        {
            _spectrumData[_currentIndex].bandData[_band].threshold = _getFluxThreshold(_currentIndex);
            _spectrumData[_currentIndex].bandData[_band].prunedSpectralFlux = _getPrunedSpectralFlux(_currentIndex);

            // We do this because we need the next flux values to determin if a value is a peak.
            int indexToDetectPeak = _currentIndex - 1;
            if (_isPeak(indexToDetectPeak))
            {
                _spectrumData[indexToDetectPeak].hasPeak = true;
                _spectrumData[indexToDetectPeak].bandData[_band].isPeak = true;
                //_spectrumData[indexToDetectPeak].bandData[_band].peakBPM = _getAveragedBpm(indexToDetectPeak);
            }
        }
        _currentIndex++;
        _processed++;
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
        float sum = 0f;
        for (int i = _beatConfig.startIndex; i < _beatConfig.endIndex; i++)
        {
            sum += Mathf.Max(0f, _currentSpectrum[i] - _previousSpectrum[i]);
        }
        return sum;
    }

    private float _getFluxThreshold(int index)
    {
        int windowStartIndex = Mathf.Max(0, index - THRESHOLD_SIZE / 2); // Amount of past and future samples for the average
        int windowEndIndex = Mathf.Min(_spectrumData.Count - 1, index + THRESHOLD_SIZE / 2);

        float sum = 0.0f;
        for (int i = windowStartIndex; i < windowEndIndex; i++)
        {
            sum += _spectrumData[i].bandData[_band].spectralFlux; // Add spectral flux over the window
        }

        // Return the average multiplied by our sensitivity multiplier
        float avg = sum / (windowEndIndex - windowStartIndex);
        return avg * THRESHOLD_MULTIPLIER;
    }

    // Pruned Spectral Flux is 0 when the threshhold has not been reached.
    private float _getPrunedSpectralFlux(int index) 
    {
        return Mathf.Max(0f, _spectrumData[index].bandData[_band].spectralFlux - _spectrumData[index].bandData[_band].threshold);
    }

    // TODO this could be optimized. Does it make sense to use pruned flux? Change multiplier level?
    private bool _isPeak(int index)
    {
        float prunedFlux = _spectrumData[index].bandData[_band].prunedSpectralFlux;
        return prunedFlux > _spectrumData[index + 1].bandData[_band].prunedSpectralFlux &&
            prunedFlux > _spectrumData[index - 1].bandData[_band].prunedSpectralFlux;

        //return prunedFlux > (_spectrumData[index + 1].bandData[_band].prunedSpectralFlux * PEAK_DETECTION_MULTIPLIER) &&
        //    prunedFlux > (_spectrumData[index - 1].bandData[_band].prunedSpectralFlux * PEAK_DETECTION_MULTIPLIER);
    }

    private float _getAveragedBpm(int currIndex) {
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
    }

}
