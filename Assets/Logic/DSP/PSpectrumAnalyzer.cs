using UnityEngine;

namespace AudioAnalyzerPlain
{
    public class PSpectrumAnalyzer
    {
        public const int SPECTRUM_SAMPLE_SIZE = 1024;
        public const int BPM_INTERVAL = 2000;  // ms

        private const int THRESHOLD_SIZE = 50;
        private const float THRESHOLD_MULTIPLIER = 1.5f;

        private FastList<PSpectrumData> _spectrumDataList;
        private FastList<double[]> _spectrumsList;
        private PPostAudioAnalyzer _postAudioAnalyzer;
        private float[] _currentSpectrum;
        private float[] _previousSpectrum;
        private float _sampleRate = (float)AudioSettings.outputSampleRate;
        private int _indexToProcess;
        private bool _isReady = false;

        public PSpectrumAnalyzer(FastList<double[]> spectrumsList)
        {
            _spectrumsList = spectrumsList;
            _spectrumDataList = new FastList<PSpectrumData>();

            _currentSpectrum = new float[SPECTRUM_SAMPLE_SIZE];
            _previousSpectrum = new float[SPECTRUM_SAMPLE_SIZE];

            _indexToProcess = THRESHOLD_SIZE / 2;
        }

        public FastList<PSpectrumData> getSpectrumDataList()
        {
            return _spectrumDataList;
        }

        public void analyzeSpectrumsList()
        {
            for (int i = 0; i < _spectrumsList.Count; i++)
            {
                double[] doubleSpectrum = _spectrumsList[i];
                float[] floatSpectrum = System.Array.ConvertAll(doubleSpectrum, doubleVal => (float)doubleVal);
                _analyzeSpectrum(floatSpectrum, _getTimeFromIndex(i));
            }

            //_postAudioAnalyzer = new PPostAudioAnalyzer(_spectrumDataList);
            //_spectrumDataList = _postAudioAnalyzer.findExtraBeats();

            _isReady = true;
        }

        public bool isReady()
        {
            return _isReady;
        }

        private void _analyzeSpectrum(float[] spectrum, float time)
        {
            _setCurrentSpectrum(spectrum);

            PSpectrumData spectrumData = new PSpectrumData();
            spectrumData.spectrum = spectrum;
            spectrumData.time = time;
            spectrumData.spectralFlux = calculateSpectralFlux();
            _spectrumDataList.Add(spectrumData);

            // The amount of samples we've collected so far is high enough to be able to detect a peak.
            if (_spectrumDataList.Count >= THRESHOLD_SIZE)
            {
                _spectrumDataList[_indexToProcess].threshold = _getFluxThreshold(_indexToProcess);
                _spectrumDataList[_indexToProcess].prunedSpectralFlux = _getPrunedSpectralFlux(_indexToProcess);

                int indexToDetectPeak = _indexToProcess - 1;
                if (_isPeak(indexToDetectPeak))
                {
                    _spectrumDataList[indexToDetectPeak].isPeak = true;
                    _spectrumDataList[indexToDetectPeak].peakBPM = _getPeakBpm(indexToDetectPeak, _spectrumDataList[indexToDetectPeak].time);
                    Debug.Log(" bpm: " + _spectrumDataList[indexToDetectPeak].peakBPM.ToString());
                    Debug.Log("time: " + _spectrumDataList[indexToDetectPeak].time.ToString());
                    Debug.Log("__________________________________");
                }
                _indexToProcess++;
            }
        }

        private float _getPeakBpm(int beatIndex, float beatTime)
        {
            int beatCounter = 0;
            float minTime = beatTime - (BPM_INTERVAL / 2);
            float maxTime = beatTime + (BPM_INTERVAL / 2);
            int searchIndex = beatIndex -1;

            while (searchIndex >= 0)
            {
                PSpectrumData currentSearch = _spectrumDataList[searchIndex];
                if (currentSearch.time < minTime) break;
                if (currentSearch.isPeak) beatCounter += 1; 
                searchIndex--;
                //if (beatIndex - searchIndex > 100) break;
            }

            searchIndex = beatIndex + 1;
            while (searchIndex < _spectrumDataList.Count)
            {
                PSpectrumData currentSearch = _spectrumDataList[searchIndex];
                if (currentSearch.time > maxTime) break;
                if (currentSearch.isPeak) beatCounter += 1;
                searchIndex++;
                //if (searchIndex - beatIndex > 100) break;
            }

            float multiplier = 60000.0f / (float)BPM_INTERVAL;
            float bpm = beatCounter * multiplier;

            return bpm;
        }

        private void _setCurrentSpectrum(float[] spectrum)
        {
            _currentSpectrum.CopyTo(_previousSpectrum, 0);
            spectrum.CopyTo(_currentSpectrum, 0);
        }

        // Calculates the rectified spectral flux.
        private float calculateSpectralFlux()
        {
            float sum = 0f;

            // Aggregate positive changes in spectrum data
            for (int i = 0; i < SPECTRUM_SAMPLE_SIZE; i++)
            {
                sum += Mathf.Max(0f, _currentSpectrum[i] - _previousSpectrum[i]);
            }
            return sum;
        }

        private float _getTimeFromIndex(int index)
        {
            return ((1f / _sampleRate) * index);
        }

        private float _getFluxThreshold(int index)
        {
            // How many samples in the past and future we include in our average
            int windowStartIndex = Mathf.Max(0, index - THRESHOLD_SIZE / 2);
            int windowEndIndex = Mathf.Min(_spectrumDataList.Count - 1, index + THRESHOLD_SIZE / 2);

            // Add up our spectral flux over the window
            float sum = 0f;
            for (int i = windowStartIndex; i < windowEndIndex; i++)
            {
                sum += _spectrumDataList[i].spectralFlux;
            }

            // Return the average multiplied by our sensitivity multiplier
            float avg = sum / (windowEndIndex - windowStartIndex);
            return avg * THRESHOLD_MULTIPLIER;
        }

        private float _getPrunedSpectralFlux(int index)
        {
            return Mathf.Max(0f, _spectrumDataList[index].spectralFlux - _spectrumDataList[index].threshold);
        }

        private bool _isPeak(int index)
        {
            if (_spectrumDataList[index].prunedSpectralFlux > _spectrumDataList[index + 1].prunedSpectralFlux &&
                _spectrumDataList[index].prunedSpectralFlux > _spectrumDataList[index - 1].prunedSpectralFlux)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
