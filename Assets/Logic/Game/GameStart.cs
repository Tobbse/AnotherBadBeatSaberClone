using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PSpectrumData;

public class GameStart : MonoBehaviour
{
    public GameObject touchableCube;
    public int cubesPerUpdate;
    public GameObject timedBlockPrefab;

    private FastList<PSpectrumInfo> _fullSpectrumDataList;
    private FastList<PSpectrumInfo> _beatSpectrumData = new FastList<PSpectrumInfo>();
    private PAnalyzerConfig _analyzerConfig;
    private FastList<double[]> _spectrumsList;
    private int _index = 0;
    private float _lastTime;
    private float _startTime;
    private System.Random _random = new System.Random();
    private float _timeframe = 2.0f;
    private bool _timeframeReached = false;
    private AudioSource _audioSource;
    private FastList<GameObject> _timedObjects = new FastList<GameObject>();
    private float _timedBlockDistance = 20;
    private GameObject _obj;
    private Rigidbody _rb;

    void Start()
    {
        /*for (int i = 0; i < 5000; i++)
        {
            GameObject cube = Instantiate<GameObject>(touchableCube, new Vector3(_random.Next(-50, 50), _random.Next(1, 50), _random.Next(-50, 50)), Quaternion.identity);
        }*/
        _fullSpectrumDataList = GlobalStorage.Instance.SpectrumInfo;
        _analyzerConfig = GlobalStorage.Instance.AnalyzerConfig;
        _spectrumsList = GlobalStorage.Instance.SpectrumsList;
        _filterBeats();

        AudioClip audioClip = GlobalStorage.Instance.AudioClip;
        gameObject.AddComponent<AudioSource>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioSource.clip = audioClip;

        _startTime = Time.time;
    }

    void Update()
    {
        /*for (int i = 0; i < cubesPerUpdate; i++)
        {
            GameObject cube = Instantiate<GameObject>(touchableCube, new Vector3(_random.Next(-50, 50), _random.Next(1, 50), _random.Next(-50, 50)), Quaternion.identity);
        }*/

        float timePassed = Time.time - _startTime;
        Debug.Log(timePassed);
        if (timePassed > _timeframe && !_timeframeReached)
        {
            _timeframeReached = true;
            _audioSource.Play();
        }

        PSpectrumInfo info = _beatSpectrumData[_index];
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
                _handleSpectrumInfo();
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

    private void _handleSpectrumInfo()
    {
        // TODO _timedBlockDistance could just be multiplied by the speed here, same with the speed!
        _obj = Instantiate(timedBlockPrefab, new Vector3(_timedBlockDistance * -1, 3, 0), Quaternion.identity);
        _rb = _obj.GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(_timedBlockDistance / _timeframe, 0, 0);
    }

    private void _filterBeats()
    {
        for (int i = 0; i < _fullSpectrumDataList.Count; i++)
        {
            if (_fullSpectrumDataList[i].hasPeak)
            {
                _beatSpectrumData.Add(_fullSpectrumDataList[i]);
            }
        }
    }
}
