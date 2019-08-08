using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using PSpectrumInfo;
using PAnalyzerConfigs;

public class GameStart : MonoBehaviour
{
    public GameObject touchableCube;
    public int cubesPerUpdate;
    public GameObject timedBlockPrefab;

    private FastList<PAnalyzedSpectrumData> _fullSpectrumDataList;
    private FastList<PAnalyzedSpectrumData> _beatSpectrumData = new FastList<PAnalyzedSpectrumData>();
    private TrackConfig _analyzerConfig;
    private FastList<double[]> _spectrumsList;
    private int _index = 0;
    private float _lastTime;
    private float _startTime;
    private System.Random _random = new System.Random();
    private float _timeframe = 2.0f;
    private bool _timeframeReached = false;
    private AudioSource _audioSource;
    private FastList<GameObject> _timedObjects = new FastList<GameObject>();
    private float _timedBlockDistance = 35;
    private GameObject _obj;
    private Rigidbody _rb;
    private PScoreTracker _scoreTracker;

    void Start()
    {
        /*for (int i = 0; i < 5000; i++)
        {
            GameObject cube = Instantiate<GameObject>(touchableCube, new Vector3(_random.Next(-50, 50), _random.Next(1, 50), _random.Next(-50, 50)), Quaternion.identity);
        }*/
        enabled = false;
        _fullSpectrumDataList = GlobalStorage.Instance.SpectrumInfo;
        _analyzerConfig = GlobalStorage.Instance.AnalyzerConfig;
        _spectrumsList = GlobalStorage.Instance.SpectrumsList;
        _filterBeats();

        AudioClip audioClip = GlobalStorage.Instance.AudioClip;
        gameObject.AddComponent<AudioSource>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioSource.clip = audioClip;

        enabled = true;
        _startTime = Time.time;
    }

    void Update()
    {
        /*for (int i = 0; i < cubesPerUpdate; i++)
        {
            GameObject cube = Instantiate<GameObject>(touchableCube, new Vector3(_random.Next(-50, 50), _random.Next(1, 50), _random.Next(-50, 50)), Quaternion.identity);
        }*/

        float timePassed = Time.time - _startTime;
        //Debug.Log(timePassed);

        if (_timeframeReached && !_audioSource.isPlaying)
        {
            SceneManager.LoadScene("Score");
        }

        if (timePassed > _timeframe && !_timeframeReached)
        {
            _timeframeReached = true;
            _audioSource.Play();
        }

        PAnalyzedSpectrumData info = _beatSpectrumData[_index];
        /*'while (timePassed >= info.time)
        {
            _index++;
            if (_index >= _beatSpectrumData.Count)
            {
                enabled = false;
                return;
            }

            info = _beatSpectrumData[_index];
            _handleSpectrumInfo();
        }*/
        for (int i = _index; i < _beatSpectrumData.Count; i++)
        {
            if (_beatSpectrumData[i].time <= timePassed)
            {
                _handleSpectrumInfo(_beatSpectrumData[i]);
                _index++;
            } else
            {
                break;
            }
        }
        if (_index >= _beatSpectrumData.Count)
        {
            //enabled = false;
            return;
        }
    }

    // TODO _timedBlockDistance could just be multiplied by the speed here, same with the speed!
    private void _handleSpectrumInfo(PAnalyzedSpectrumData info)
    {
        foreach (int peak in info.peakBands)
        {
            float spectralFlux = info.beatData[peak].spectralFlux;
            _obj = Instantiate(timedBlockPrefab, new Vector3(_timedBlockDistance * -1, 1.5f + spectralFlux, -0.5f + peak), Quaternion.identity);
            if (peak == 0)
            {
                _obj.layer = 8;
            }
            if (peak == 1)
            {
                _obj.layer = 9; 
            }
            _rb = _obj.GetComponent<Rigidbody>();
            _rb.velocity = new Vector3(_timedBlockDistance / _timeframe, 0, 0);
        }
    }

    private void _filterBeats()
    {
        int beats = 0;
        for (int i = 0; i < _fullSpectrumDataList.Count; i++)
        {
            PAnalyzedSpectrumData info = _fullSpectrumDataList[i];
            if (info.hasPeak)
            {
                _beatSpectrumData.Add(info);
                beats += info.peakBands.Count;
            }
        }
        if (beats == 0)
        {
            Debug.LogException(new Exception("No beat was found in the file!"));
            enabled = false;
        }
        _scoreTracker = new PScoreTracker(beats);
    }
}
