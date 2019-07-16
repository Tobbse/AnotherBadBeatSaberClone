using UnityEngine;

namespace AudioAnalyzerPlain
{
    public class PSpectrumAnalyzer
    {
        private const int SPECTRUM_SAMPLE_SIZE = PSpectrumProvider.SPECTRUM_SAMPLE_SIZE;
        private const int THRESHOLD_SIZE = 20;
        //private const float THRESHOLD_MULTIPLIER = 1.95f;
        //private const float PEAK_DETECTION_MULTIPLIER = 1.85f;
        private const float THRESHOLD_MULTIPLIER = 1.50f;
        private const float PEAK_DETECTION_MULTIPLIER = 1.05f;
        private const int NUM_FREQUENCY_BINS = SPECTRUM_SAMPLE_SIZE / 2;

        private FastList<PSpectrumData> _spectrumDataList;
        private FastList<double[]> _spectrumsList;
        private PPostAudioAnalyzer _postAudioAnalyzer;
        private int _indexToProcess;
        private bool _isReady = false;
        private float[] _currentSpectrum;
        private float[] _previousSpectrum;
        private float[] _bpms;
        private float _sampleRate = (float)AudioSettings.outputSampleRate;
        private float _bpmInterval;
        private float _timePerSample;
        private float _timePerSpectrumData;
        private float _spectrumDataPerBPMInterval;

        public PSpectrumAnalyzer(FastList<double[]> spectrumsList)
        {
            _spectrumsList = spectrumsList;
            _spectrumDataList = new FastList<PSpectrumData>();

            _currentSpectrum = new float[spectrumsList[0].Length];
            _previousSpectrum = new float[spectrumsList[0].Length];

            _indexToProcess = THRESHOLD_SIZE / 2;

            _timePerSample = 1f / _sampleRate;
            _timePerSpectrumData = _timePerSample * SPECTRUM_SAMPLE_SIZE; // We have only 512 frequency bins but need 1024 samples to achieve that.
            _bpmInterval = 4.0f;
            _spectrumDataPerBPMInterval = _bpmInterval / _timePerSpectrumData;
        }

        public FastList<PSpectrumData> getSpectrumDataList()
        {
            return _spectrumDataList;
        }

        public void analyzeSpectrumsList()
        {
            _bpms = new float[_spectrumsList.Count];

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

            _bpms[_indexToProcess] = _getBpm(_indexToProcess);

            // The amount of samples we've collected so far is high enough to be able to detect a peak.
            if (_spectrumDataList.Count >= THRESHOLD_SIZE)
            {
                _spectrumDataList[_indexToProcess].threshold = _getFluxThreshold(_indexToProcess);
                _spectrumDataList[_indexToProcess].prunedSpectralFlux = _getPrunedSpectralFlux(_indexToProcess);

                // We do this because we need the next flux values to determin if a value is a peak.
                int indexToDetectPeak = _indexToProcess - 1;
                if (_isPeak(indexToDetectPeak))
                {
                    _spectrumDataList[indexToDetectPeak].isPeak = true;
                    _spectrumDataList[indexToDetectPeak].peakBPM = _getAveragedBpm(indexToDetectPeak);
                    Debug.Log(" bpm: " + _spectrumDataList[indexToDetectPeak].peakBPM.ToString());
                    Debug.Log("time: " + _spectrumDataList[indexToDetectPeak].time.ToString());
                    Debug.Log("__________________________________");
                }
                _indexToProcess++;
            }
        }

        private float _getAveragedBpm(int currIndex) {
            int indexDist = Mathf.CeilToInt(_spectrumDataPerBPMInterval);
            int minIndex = currIndex - indexDist;
            int maxIndex = currIndex - 1;

            if (minIndex <= 0) minIndex = 0;
            if (maxIndex <= 0) maxIndex = 0;
            if (maxIndex >= _spectrumDataList.Count) maxIndex = _spectrumDataList.Count - 1;

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
            int indexDist = Mathf.CeilToInt(_spectrumDataPerBPMInterval);
            int minIndex = beatIndex - indexDist;
            int maxIndex = beatIndex -1;
            int currentIndex = beatIndex - 1;

            if (minIndex <= 0) minIndex = 0;
            if (maxIndex <= 0) maxIndex = 0;
            if (maxIndex >= _spectrumDataList.Count) maxIndex = _spectrumDataList.Count - 1;

            int newIndexDist = maxIndex - minIndex;
            float newBpmInterval = newIndexDist * _timePerSpectrumData;

            for (int i = minIndex; i < maxIndex; i++)
            {
                if (_spectrumDataList[i].isPeak)
                {
                    beats += 1;
                }
            }
            return beats * (60 / newBpmInterval);
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
            for (int i = 0; i < NUM_FREQUENCY_BINS / 6.0f; i++)
            {
                sum += Mathf.Max(0f, _currentSpectrum[i] - _previousSpectrum[i]);
                //sum += _currentSpectrum[i];
            }
            return sum;
        }

        private float _getTimeFromIndex(int spectrumDataIndex)
        {
            return _timePerSpectrumData * spectrumDataIndex;
        }

        private float _getFluxThreshold(int index)
        {
            // How many samples in the past and future we include in our average
            int windowStartIndex = Mathf.Max(0, index - THRESHOLD_SIZE / 2);
            int windowEndIndex = Mathf.Min(_spectrumDataList.Count - 1, index + THRESHOLD_SIZE / 2);

            // Add up our spectral flux over the window
            float sum = 0.0f;
            for (int i = windowStartIndex; i < windowEndIndex; i++) {
                sum += _spectrumDataList[i].spectralFlux;
            }
            
            // Return the average multiplied by our sensitivity multiplier
            float avg = sum / (windowEndIndex - windowStartIndex);
            return avg * THRESHOLD_MULTIPLIER;
        }

        // Pruned Spectral Flux is 0 when the threshhold has not been reached.
        private float _getPrunedSpectralFlux(int index)
        {
            return Mathf.Max(0f, _spectrumDataList[index].spectralFlux - _spectrumDataList[index].threshold);
        }

        private bool _isPeak(int index)
        {
            //float flux = _spectrumDataList[index].spectralFlux;
            float prunedFlux = _spectrumDataList[index].prunedSpectralFlux;
            if (prunedFlux > (_spectrumDataList[index + 1].prunedSpectralFlux * PEAK_DETECTION_MULTIPLIER) &&
                prunedFlux > (_spectrumDataList[index - 1].prunedSpectralFlux * PEAK_DETECTION_MULTIPLIER))
            //if (flux > (_spectrumDataList[index + 1].spectralFlux * PEAK_DETECTION_MULTIPLIER) &&
            //    flux > (_spectrumDataList[index - 1].spectralFlux * PEAK_DETECTION_MULTIPLIER))
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
