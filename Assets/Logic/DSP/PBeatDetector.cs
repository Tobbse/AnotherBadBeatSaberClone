using UnityEngine;
using PSpectrumData;

public class PBeatDetector
{
    //public static int THRESHOLD_SIZE = 100;
    //public static int BEAT_BLOCK_TIME = 50;

    private const int SPECTRUM_SAMPLE_SIZE = PSpectrumProvider.SAMPLE_SIZE;
    private const int NUM_BINS = PSpectrumProvider.NUM_BINS;
    private const float THRESHOLD_MULTIPLIER = 5;
    private const float DETECTION_MULT_BEFORE = 1.5f;
    private const float DETECTION_MULT_AFTER = 0.5f;
    //private const float BPM_INTERVAL = 4.0f; // 4 seconds is probably too much

    
    private PBeatConfig _beatConfig;
    private FastList<SpectrumInfo> _spectrumData;
    private int _currentIndex;
    private float[] _currentSpectrum;
    private float[] _previousSpectrum;
    private float[] _bpms;
    private float _spectrumsPerBPMInterval;
    private float _timePerSpectrum;
    private int _band;
    private int _processed = 0;
    private int _clipSampleRate;
    private int _thresholdActivationCounter = 0;

    public PBeatDetector(PBeatConfig beatConfig, FastList<SpectrumInfo> spectrumData, PAnalyzerConfig config)
    {
        _beatConfig = beatConfig;
        _spectrumData = spectrumData;
        //_currentIndex = THRESHOLD_SIZE / 2;
        _currentIndex = 0;
        _band = _beatConfig.band;
        _clipSampleRate = config.ClipSampleRate;

        _currentSpectrum = new float[NUM_BINS];
        _previousSpectrum = new float[NUM_BINS];
        //_bpms = new float[NUM_BINS];

        float timePerSample = 1f / _clipSampleRate;
        _timePerSpectrum = timePerSample * SPECTRUM_SAMPLE_SIZE;
        //_spectrumsPerBPMInterval = BPM_INTERVAL / (timePerSample * SPECTRUM_SAMPLE_SIZE); // BPM-Interval divided by TimePerSpectrum
    }

    public void resetIndex()
    {
        _currentIndex = 0;
    }

    /*public void analyzeSpectrum()
    {
        _setCurrentSpectrum(_spectrumData[_currentIndex].spectrum);
        _spectrumData[_currentIndex].bandData[_band].spectralFlux = _calcSpectralFlux();

        if (_processed >= _beatConfig.thresholdSize)
        {
            int indexToDetectPeak = _currentIndex - 1;
            _spectrumData[_currentIndex].bandData[_band].threshold = _getFluxThreshold(_currentIndex);
            _spectrumData[_currentIndex].bandData[_band].prunedSpectralFlux = _getPrunedSpectralFlux(_currentIndex);

            if (_thresholdActivationCounter > 0)
            {
                _thresholdActivationCounter--;
            }
            else if (_isPeak(indexToDetectPeak))
            { // We do this because we need the next flux values to determin if a value is a peak.
                _thresholdActivationCounter = _beatConfig.beatTime;
                _spectrumData[indexToDetectPeak].hasPeak = true;
                _spectrumData[indexToDetectPeak].bandData[_band].isPeak = true;
                //_spectrumData[indexToDetectPeak].bandData[_band].peakBPM = _getAveragedBpm(indexToDetectPeak);
            }
        }
        _currentIndex++;
        _processed++;
    }*/

    public void getFluxValues()
    {
        _setCurrentSpectrum(_spectrumData[_currentIndex].spectrum);
        _spectrumData[_currentIndex].bandData[_band].spectralFlux = _calcSpectralFlux();
        _currentIndex++;
    }

    public void analyzeSpectrum()
    {
        _setCurrentSpectrum(_spectrumData[_currentIndex].spectrum);

        if (_currentIndex >= _beatConfig.thresholdSize / 2)
        {
            _spectrumData[_currentIndex].bandData[_band].threshold = _getFluxThreshold(_currentIndex);
            _spectrumData[_currentIndex].bandData[_band].prunedSpectralFlux = _getPrunedSpectralFlux(_currentIndex);

            if (_thresholdActivationCounter > 0)
            {
                _thresholdActivationCounter--;
            }
            else if (_isPeak(_currentIndex))
            { // We do this because we need the next flux values to determin if a value is a peak.
                _thresholdActivationCounter = _beatConfig.beatTime;
                _spectrumData[_currentIndex].hasPeak = true;
                _spectrumData[_currentIndex].bandData[_band].isPeak = true;
                //_spectrumData[indexToDetectPeak].bandData[_band].peakBPM = _getAveragedBpm(indexToDetectPeak);
            }
        }
        _currentIndex++;
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
        int windowStartIndex = Mathf.Max(0, index - _beatConfig.thresholdSize / 2); // Amount of past and future samples for the average
        int windowEndIndex = Mathf.Min(_spectrumData.Count - 1, index + _beatConfig.thresholdSize / 2);
        //int windowStartIndex = Mathf.Max(0, index - _beatConfig.thresholdSize); // Amount of past samples for the average
        //int windowEndIndex = Mathf.Min(_spectrumData.Count - 1, index - 1);

        float sum = 0.0f;
        for (int i = windowStartIndex; i < windowEndIndex; i++)
        {
            sum += _spectrumData[i].bandData[_band].spectralFlux; // Add spectral flux over the window
        }

        // Return the average multiplied by our sensitivity multiplier
        float avg = sum / (windowEndIndex - windowStartIndex);
        return avg * _beatConfig.tresholdMult;
    }

    // Pruned Spectral Flux is 0 when the threshhold has not been reached.
    private float _getPrunedSpectralFlux(int index) 
    {
        return Mathf.Max(0f, _spectrumData[index].bandData[_band].spectralFlux - _spectrumData[index].bandData[_band].threshold);
    }

    // TODO this could be optimized. Does it make sense to use pruned flux? Change multiplier level?
    private bool _isPeak(int index)
    {
        if (index >= _spectrumData.Count - 1 || index - 2 < 0) return false;

        Debug.Log("INDEXINDEXINDEXINDEXINDEX: " + index.ToString());
        float prunedFlux = _spectrumData[index].bandData[_band].prunedSpectralFlux;
        //return prunedFlux > _spectrumData[index + 1].bandData[_band].prunedSpectralFlux &&
        //    prunedFlux > _spectrumData[index - 1].bandData[_band].prunedSpectralFlux;
        return prunedFlux > (_spectrumData[index + 1].bandData[_band].prunedSpectralFlux * DETECTION_MULT_AFTER) &&
            prunedFlux > (_spectrumData[index - 1].bandData[_band].prunedSpectralFlux * DETECTION_MULT_BEFORE * 2) &&
            prunedFlux > (_spectrumData[index - 2].bandData[_band].prunedSpectralFlux * DETECTION_MULT_BEFORE);
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
