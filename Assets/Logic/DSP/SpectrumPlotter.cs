using UnityEngine;
using AudioAnalyzerPlain;

public class SpectrumPlotter : MonoBehaviour
{
    public static int DISPLAY_WINDOW_SIZE = 300;

    private int _currentPlotIndex = 0;
    private FastList<SpectrumData> _spectrumDataList;
    private FastList<Transform> _plotPoints;
    private bool _isReady = false;

    void Start()
    {
        _plotPoints = new FastList<Transform>();
        float localWidth = transform.Find("Point/BasePoint").localScale.x;

        // -n/2...0...n/2
        for (int i = 0; i < DISPLAY_WINDOW_SIZE; i++)
        {
            //Instantiate points
            GameObject point = Instantiate(Resources.Load("Point"), transform) as GameObject;
            Transform pointTransform = point.transform;
            Transform originalPointTransform = transform.Find("Point");
            // Applying original materials to all new sub points.
            foreach (Transform child in originalPointTransform)
            {
                string name = child.name;
                Renderer originalRenderer = originalPointTransform.Find(name).GetComponent<Renderer>();
                Renderer newPointRenderer = pointTransform.Find(name).GetComponent<Renderer>();
                newPointRenderer.material = originalRenderer.material;
            }

            // Set position
            float pointX = (DISPLAY_WINDOW_SIZE / 2) * -1 * localWidth + i * localWidth;
            pointTransform.localPosition = new Vector3(pointX, pointTransform.localPosition.y, pointTransform.localPosition.z);
            pointTransform.localPosition = new Vector3(pointX, pointTransform.localPosition.y, pointTransform.localPosition.z);

            _plotPoints.Add(pointTransform);
        }
    }

    void FixedUpdate()
    {
        if (_isReady && _hasRemainingSamples())
        {
            _updatePlot();
        }
    }

    public void setSpectrumData(FastList<SpectrumData> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;
        _isReady = true;
    }

    private bool _hasRemainingSamples()
    {
        return _currentPlotIndex <= _spectrumDataList.Count;
    }

    private void _updatePlot()
    {
        if (_plotPoints.Count < DISPLAY_WINDOW_SIZE - 1)
            return;

        int numPlotted = 0;
        int windowStart = 0;
        int windowEnd = 0;

        if (_currentPlotIndex > 0)
        {
            windowStart = Mathf.Max(0, _currentPlotIndex - DISPLAY_WINDOW_SIZE / 2);
            windowEnd = Mathf.Min(_currentPlotIndex + DISPLAY_WINDOW_SIZE / 2, _spectrumDataList.Count - 1);
        }
        else
        {
            windowStart = Mathf.Max(0, _spectrumDataList.Count - DISPLAY_WINDOW_SIZE - 1);
            windowEnd = Mathf.Min(windowStart + DISPLAY_WINDOW_SIZE, _spectrumDataList.Count);
        }

        for (int i = windowStart; i < windowEnd; i++)
        {
            int plotIndex = numPlotted;
            numPlotted++;

            Transform fluxPoint = _plotPoints[plotIndex].Find("FluxPoint");
            Transform threshPoint = _plotPoints[plotIndex].Find("ThreshPoint");
            Transform peakPoint = _plotPoints[plotIndex].Find("PeakPoint");
            Transform extraPeakPoint = _plotPoints[plotIndex].Find("ExtraPeakPoint");


            if (_spectrumDataList[i].isExtraPeak)
            {
                Debug.Log("");
                _setPointHeight(extraPeakPoint, _spectrumDataList[i].spectralFlux);
                _setPointHeight(peakPoint, 0f);
                _setPointHeight(fluxPoint, 0f);
            }
            else if (_spectrumDataList[i].isPeak)
            {
                _setPointHeight(peakPoint, _spectrumDataList[i].spectralFlux);
                _setPointHeight(extraPeakPoint, -1000f);
                _setPointHeight(fluxPoint, 0f);
            }
            else
            {
                _setPointHeight(fluxPoint, _spectrumDataList[i].spectralFlux);
                _setPointHeight(extraPeakPoint, 0f);
                _setPointHeight(peakPoint, -1000f);
            }
            _setPointHeight(threshPoint, _spectrumDataList[i].threshold);
        }

        _currentPlotIndex += 1;

    }

    private void _setPointHeight(Transform point, float height)
    {
        float displayMultiplier = 0.06f;

        point.localPosition = new Vector3(point.localPosition.x, height * displayMultiplier, point.localPosition.z);
    }

}
