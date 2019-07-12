using UnityEngine;

namespace AudioAnalyzerPlain
{
    public class SpectrumAnalyzer
    {
        public const int SPECTRUM_SAMPLE_SIZE = 1024;
        public const int BPM_INTERVAL = 2000;  // ms

        private const int THRESHOLD_SIZE = 50;
        private const float THRESHOLD_MULTIPLIER = 1.5f;

        private FastList<SpectrumData> _spectrumDataList;
        private FastList<double[]> _spectrumsList;
        private float[] _currentSpectrum;
        private float[] _previousSpectrum;
        private float _sampleRate = (float)AudioSettings.outputSampleRate;
        private int _indexToProcess;
        private bool _isReady = false;

        public SpectrumAnalyzer(FastList<double[]> spectrumsList)
        {
            _spectrumsList = spectrumsList;
            _spectrumDataList = new FastList<SpectrumData>();

            _currentSpectrum = new float[SPECTRUM_SAMPLE_SIZE];
            _previousSpectrum = new float[SPECTRUM_SAMPLE_SIZE];

            _indexToProcess = THRESHOLD_SIZE / 2;
        }

        public FastList<SpectrumData> getSpectrumDataList()
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
            _findExtraBeats();

            _isReady = true;
        }

        public bool isReady()
        {
            return _isReady;
        }

        private void _analyzeSpectrum(float[] spectrum, float time)
        {
            _setCurrentSpectrum(spectrum);

            SpectrumData spectrumData = new SpectrumData();
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
                SpectrumData currentSearch = _spectrumDataList[searchIndex];
                if (currentSearch.time < minTime) break;
                if (currentSearch.isPeak) beatCounter += 1; 
                searchIndex--;
            }

            searchIndex = beatIndex + 1;
            while (searchIndex < _spectrumDataList.Count)
            {
                SpectrumData currentSearch = _spectrumDataList[searchIndex];
                if (currentSearch.time > maxTime) break;
                if (currentSearch.isPeak) beatCounter += 1;
                searchIndex++;
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

        private void _findExtraBeats()
        {
            float totalAverageFlux = _getAverageFluxForInterval(0, _spectrumDataList.Count - 1);

            for (int i = 0; i < _spectrumDataList.Count; i++)
            {
                SpectrumData spectrumData = _spectrumDataList[i];

                if (spectrumData.isPeak)
                {
                    float beforeAverageFlux = _getAverageFluxForInterval(i - 10, i);
                    float afterAverageFlux = _getAverageFluxForInterval(i, i + 10);
                    //Debug.Log(beforeAverageFlux);
                    //Debug.Log(afterAverageFlux);

                    if (shouldBePeak(totalAverageFlux, beforeAverageFlux))
                    {
                        int previousBeatIndex = _getNextBeatIndex(i, false);
                        Debug.Log(previousBeatIndex);
                        if (previousBeatIndex != -1)
                        {
                            SpectrumData previousBeat = _spectrumDataList[previousBeatIndex];
                            float averageBpm = (previousBeat.peakBPM + spectrumData.peakBPM) / 2;
                            int newIndex = (i - previousBeatIndex) / 2;
                            _spectrumDataList[newIndex].isPeak = true;
                            _spectrumDataList[newIndex].isExtraPeak = true;
                            _spectrumDataList[newIndex].peakBPM = averageBpm;

                            /*float newTime = spectrumData.time - (previousBeat.time / 2);
                            int closestPreviousIndex = _getNearestSpectrumDataIndex(newTime, i);
                            if (closestPreviousIndex != 0)
                            {
                                _spectrumDataList[closestPreviousIndex].isPeak = true;
                                _spectrumDataList[closestPreviousIndex].peakBPM = averageBpm;
                                Debug.Log("Adding an additional previous beat!");
                            }*/
                        }
                    }
                    if (shouldBePeak(totalAverageFlux, afterAverageFlux))
                    {
                        int nextBeatIndex = _getNextBeatIndex(i, true);
                        Debug.Log(nextBeatIndex);
                        if (nextBeatIndex != -1)
                        {
                            SpectrumData nextBeat = _spectrumDataList[nextBeatIndex];
                            float averageBpm = (nextBeat.peakBPM + spectrumData.peakBPM) / 2;
                            int newIndex = (nextBeatIndex - i) / 2;
                            _spectrumDataList[newIndex].isPeak = true;
                            _spectrumDataList[newIndex].isExtraPeak = true;
                            _spectrumDataList[newIndex].peakBPM = averageBpm;

                            /*float newTime = spectrumData.time + (nextBeat.time / 2);
                            int closestNextIndex = _getNearestSpectrumDataIndex(newTime, i);
                            if (closestNextIndex != 0)
                            {
                                _spectrumDataList[closestNextIndex].isPeak = true;
                                _spectrumDataList[closestNextIndex].peakBPM = averageBpm;
                                Debug.Log("Adding an additional next beat!");
                            }*/
                        }
                    }
                }
            }
        }

        private int _getNearestSpectrumDataIndex(float time, int startIndex)
        {
            float ascTime = -1.0f;
            float descTime = 1.0f;
            int ascIndex = startIndex += 1;
            int descIndex = startIndex -= 1;

            while (descIndex >= 0)
            {
                descTime = _spectrumDataList[descIndex].time;
                if (descTime < time)
                {
                    float nextTime = _spectrumDataList[descIndex + 1].time;
                    descTime = _spectrumDataList[descIndex].time;
                    if (_sampleIsCloser(nextTime, descTime, time)) descIndex += 1;
                    descTime = _spectrumDataList[descIndex].time;
                    break;
                }
            }

            while (ascIndex < _spectrumDataList.Count)
            {
                ascTime = _spectrumDataList[ascIndex].time;
                if (ascTime > time)
                {
                    float prevTime = _spectrumDataList[ascIndex - 1].time;
                    ascTime = _spectrumDataList[ascIndex].time;
                    if (_sampleIsCloser(ascTime, prevTime, time)) ascIndex -= 1;
                    ascTime = _spectrumDataList[ascIndex].time;
                    break;
                }
            }
            if (ascTime == -1.0f || descTime == -1.0f)
            {
                if (ascTime == -1.0f && descTime == -1.0f) return -1;
                if (ascTime == -1.0f) return descIndex;
                if (descTime == -1.0f) return ascIndex;
            }

            if (_sampleIsCloser(ascTime, descTime, time)) return ascIndex;
            else return descIndex;
        }

        private bool _sampleIsCloser(float newVal, float oldVal, float trueVal)
        {
            return Mathf.Abs(trueVal - newVal) < Mathf.Abs(trueVal - oldVal);
        }

        private int _getNextBeatIndex(int index, bool ascending)
        {
            if (ascending)
            {
                int searchIndex = index + 1;
                while (searchIndex < _spectrumDataList.Count)
                {
                    if (_spectrumDataList[searchIndex].isPeak)
                    {
                        return searchIndex;
                    }
                    searchIndex++;
                    if (searchIndex - index > 100) return -1;
                }
            }
            else
            {
                int searchIndex = index - 1;
                while (searchIndex >= 0)
                {
                    if (_spectrumDataList[searchIndex].isPeak)
                    {
                        return searchIndex;
                    }
                    searchIndex--;
                    if (index - searchIndex > 100) return -1;
                }
            }
            return -1;
        }

        private float _getAverageFluxForInterval(int minIndex, int maxIndex)
        {
            if (minIndex < 0) minIndex = 0;
            if (maxIndex >= _spectrumDataList.Count) maxIndex = _spectrumDataList.Count - 1;

            float fluxSum = 0;
            for (int i = minIndex; i <= maxIndex; i++)
            {
                fluxSum += _spectrumDataList[i].spectralFlux;
            }
            fluxSum /= maxIndex - minIndex;

            return fluxSum;
        }

        private bool shouldBePeak(float totalAverageFlux, float currentAverageFlux)
        {
            //Debug.Log("Found another peak!");
            return currentAverageFlux > totalAverageFlux * 1.5;
        }
    }


    public class SpectrumData
    {
        public float time;
        public float spectralFlux;
        public float threshold;
        public float prunedSpectralFlux;
        public bool isPeak;
        public bool isExtraPeak;
        public float peakBPM = 0;
        public float[] spectrum;
    }

}
