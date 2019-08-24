using UnityEngine;
using AudioSpectrumInfo;
using System.Collections.Generic;

public class PPostAudioAnalyzer
{
    private List<AnalyzedSpectrumConfig>  _spectrumDataList;

    public PPostAudioAnalyzer(List<AnalyzedSpectrumConfig> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;
    }

    /*public List<SpectrumInfo> findExtraBeats()
    {
        float totalAverageFlux = _getAverageFluxForInterval(0, _spectrumDataList.Count - 1);

        for (int i = 0; i < _spectrumDataList.Count; i++)
        {
            SpectrumInfo spectrumData = _spectrumDataList[i];

            if (spectrumData.isPeak)
            {
                float beforeAverageFlux = _getAverageFluxForInterval(i - 10, i);
                float afterAverageFlux = _getAverageFluxForInterval(i, i + 10);
                //Debug.Log(beforeAverageFlux);
                //Debug.Log(afterAverageFlux);

                if (PSpectrumAnalysisUtils.shouldBeExtraPeak(totalAverageFlux, beforeAverageFlux))
                {
                    int previousBeatIndex = _getNextBeatIndex(i, false);
                    Debug.Log(previousBeatIndex);
                    if (previousBeatIndex != -1)
                    {
                        SpectrumInfo previousBeat = _spectrumDataList[previousBeatIndex];
                        float averageBpm = (previousBeat.peakBPM + spectrumData.peakBPM) / 2;
                        int newIndex = (i - previousBeatIndex) / 2;
                        _spectrumDataList[newIndex].isPeak = true;
                        _spectrumDataList[newIndex].isExtraPeak = true;
                        _spectrumDataList[newIndex].peakBPM = averageBpm;

                        //float newTime = spectrumData.time - (previousBeat.time / 2);
                        //int closestPreviousIndex = _getNearestSpectrumDataIndex(newTime, i);
                        //if (closestPreviousIndex != 0)
                        //{
                        //    _spectrumDataList[closestPreviousIndex].isPeak = true;
                        //    _spectrumDataList[closestPreviousIndex].peakBPM = averageBpm;
                        //    Debug.Log("Adding an additional previous beat!");
                        //}
}
                }
                if (PSpectrumAnalysisUtils.shouldBeExtraPeak(totalAverageFlux, afterAverageFlux))
                {
                    int nextBeatIndex = _getNextBeatIndex(i, true);
                    Debug.Log(nextBeatIndex);
                    if (nextBeatIndex != -1)
                    {
                        PSpectrumData nextBeat = _spectrumDataList[nextBeatIndex];
                        float averageBpm = (nextBeat.peakBPM + spectrumData.peakBPM) / 2;
                        int newIndex = (nextBeatIndex - i) / 2;
                        _spectrumDataList[newIndex].isPeak = true;
                        _spectrumDataList[newIndex].isExtraPeak = true;
                        _spectrumDataList[newIndex].peakBPM = averageBpm;

                        //float newTime = spectrumData.time + (nextBeat.time / 2);
                        //int closestNextIndex = _getNearestSpectrumDataIndex(newTime, i);
                        //if (closestNextIndex != 0)
                        //{
                        //    _spectrumDataList[closestNextIndex].isPeak = true;
                        //    _spectrumDataList[closestNextIndex].peakBPM = averageBpm;
                        //    Debug.Log("Adding an additional next beat!");
                        //}
}
                }
            }
        }
        return _spectrumDataList;
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
                if (PSpectrumAnalysisUtils.sampleIsCloser(nextTime, descTime, time)) descIndex += 1;
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
                if (PSpectrumAnalysisUtils.sampleIsCloser(ascTime, prevTime, time)) ascIndex -= 1;
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

        if (PSpectrumAnalysisUtils.sampleIsCloser(ascTime, descTime, time)) return ascIndex;
        else return descIndex;
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
    }*/

}
