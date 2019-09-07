using UnityEngine;
using AudioSpectrumInfo;
using System.Collections;
using System.Collections.Generic;

/**
 * A sort of plotting component for a test scene, that is not needed anymore.
 * That scene was needed to visualize the audio analysis when developing it.
 * Basically what this does is to spawn a bunch of small cubes from the generated
 * audio data and visualize the peaks and flux/threshold levels while the song is playing.
 * 
 * TODO: Check if this still works.
 **/
public class SpectrumPlotter : MonoBehaviour
{
    public const string SHOW_PRUNED = "SHOW_PRUNED";
    public const string SHOW_PEAKS = "SHOW_PEAKS";

    public GameObject pointPrefab;
    public GameObject basePoint;

    private const int DISPLAY_WINDOW_SIZE = 300;

    private List<AnalyzedSpectrumConfig> _spectrumDataList;
    private List<Transform> _plotPoints;
    private bool _isReady = false;
    private float _lastTime;
    private int _spectrumIndex = 0;
    private int _bands;
    private string _type;

    void FixedUpdate()
    {
        if (_isReady && _hasRemainingSamples())
        {
            float newTime = Time.time;
            _lastTime = newTime;

            _updatePlot();
        } else
        {
            Debug.Log("not enough samples or no samples left!");
        }
    }

    public void setDataAndStart(List<AnalyzedSpectrumConfig> spectrumDataList, string type)
    {
        _spectrumDataList = spectrumDataList;
        _type = type;
        _bands = _spectrumDataList[0].BandBeatData.Count;
        _lastTime = Time.time;
        _isReady = true;
    }

    private void _init()
    {
        _plotPoints = new List<Transform>();
        float localWidth = transform.Find("Point/BasePoint").localScale.x;

        // Instantiates cubes depending on window size.
        // The height of those cubes will later be changed on update, to visualize the spectrums.
        for (int i = 0; i < DISPLAY_WINDOW_SIZE; i++)  // -n/2...0...n/2
        {
            GameObject point = Instantiate(pointPrefab);
            Transform pointTransform = point.transform;
            Transform originalPointTransform = transform.Find("Point");
            
            foreach (Transform child in originalPointTransform)
            {
                string name = child.name;
                Renderer originalRenderer = originalPointTransform.Find(name).GetComponent<Renderer>();
                Renderer newPointRenderer = pointTransform.Find(name).GetComponent<Renderer>();
                newPointRenderer.material = originalRenderer.material;
            }

            float pointX = (DISPLAY_WINDOW_SIZE / 2) * -1 * localWidth + i * localWidth;
            pointTransform.localPosition = new Vector3(pointX, pointTransform.localPosition.y, pointTransform.localPosition.z);
            pointTransform.localPosition = new Vector3(pointX, pointTransform.localPosition.y, pointTransform.localPosition.z);

            _plotPoints.Add(pointTransform);
        }

        for (int i = 0; i < DISPLAY_WINDOW_SIZE; i++)
        {
            for (int j = 0; j < _bands; j++)
            {
                _setPointHeight(_plotPoints[i].Find("Peak" + j.ToString()), -1000);
            }
        }
    }

    private bool _hasRemainingSamples()
    {
        return _spectrumIndex < _spectrumDataList.Count;
    }

    private void _updatePlot()
    {
        if (_spectrumIndex == 0)
        {
            Transform audioAnalyzer = GameObject.Find("AudioAnalyzer").transform;
            AudioSource audio = audioAnalyzer.GetComponent<AudioSource>();
            audio.Play();
        }
        switch (_type)
        {
            case SHOW_PEAKS:
                _showPeaks();
                break;

            case SHOW_PRUNED:
                _showPruned();
                break;
        }
        _spectrumIndex++;
    }

    // Shows peaks.
    // TODO: Don't use 'Find' in update functions. This class is not used anymore, so this has low priority.
    private void _showPeaks()
    {
        for (int pointIndex = 0; pointIndex < DISPLAY_WINDOW_SIZE; pointIndex++)
        {
            AnalyzedSpectrumConfig info = _getInfo(pointIndex);

            for (int j = 0; j < _bands; j++)
            {
                BeatInfo bandData = info.BandBeatData[j];

                Transform peak = _plotPoints[pointIndex].Find("Peak" + j.ToString());

                Transform thresh = _plotPoints[pointIndex].Find("Thresh" + j.ToString());

                Color peakColor = bandData.IsPeak ? Color.red : Color.white;
                float peakHeight = bandData.IsPeak ? 1.0f : -100.0f;

                _setPointHeight(peak, peakHeight);
                peak.gameObject.SetActive(bandData.IsPeak);
                (peak.GetComponent<Renderer>() as Renderer).material.color = peakColor;

                _setPointHeight(thresh, bandData.Threshold);
            }
        }
    }

    // Shows pruned flux levels.
    // TODO: Don't use 'Find' in update functions. This class is not used anymore, so this has low priority.
    private void _showPruned()
    {
        for (int pointIndex = 0; pointIndex < DISPLAY_WINDOW_SIZE; pointIndex++)
        {
            AnalyzedSpectrumConfig info = _getInfo(pointIndex);

            for (int j = 0; j < _bands; j++)
            {
                BeatInfo bandData = info.BandBeatData[j];
                bool isZero = bandData.PrunedSpectralFlux == 0;

                Transform pruned = _plotPoints[pointIndex].Find("Pruned" + j.ToString());
                float currentHeight = (pruned.GetComponent<Renderer>() as Renderer).bounds.size.y;
                float newHeight = isZero ? 0.0005f : bandData.PrunedSpectralFlux;
                Vector3 rescale = pruned.localScale;
                rescale.y = newHeight * rescale.y / currentHeight;
                pruned.localScale = rescale;
            }
        }
    }

    private void _setPointHeight(Transform point, float height)
    {
        float displayMultiplier = 0.06f * 2;

        point.localPosition = new Vector3(point.localPosition.x, height * displayMultiplier, point.localPosition.z);
    }

    private bool _areSomePointsOOB()
    {
        return _spectrumIndex > (_spectrumDataList.Count - 1 - DISPLAY_WINDOW_SIZE);
    }

    private AnalyzedSpectrumConfig _getInfo(int pointIndex)
    {
        int pointDataIndex = _spectrumIndex + pointIndex;
        bool isOutOfBounds = pointDataIndex > _spectrumDataList.Count - 1;
        AnalyzedSpectrumConfig info = isOutOfBounds ? _getEmptySpectrumInfo() : _spectrumDataList[pointDataIndex];
        return info;
    }

    private AnalyzedSpectrumConfig _getEmptySpectrumInfo()
    {
        AnalyzedSpectrumConfig emptyInfo = new AnalyzedSpectrumConfig();
        emptyInfo.HasPeak = false;

        for (int i = 0; i < _bands; i++)
        {
            BeatInfo bandData = new BeatInfo();
            bandData.Band = i;
            bandData.IsPeak = false;
            bandData.SpectralFlux = 0.0f;
            bandData.PrunedSpectralFlux = 0.0f;
            bandData.Threshold = 0.0f;
            emptyInfo.BandBeatData.Add(bandData);
        }
        return emptyInfo;
    }

}
