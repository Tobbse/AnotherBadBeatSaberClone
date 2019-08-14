using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using AudioSpectrumInfo;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;

public class OldGame : MonoBehaviour
{
    public GameObject touchableCube;
    public int cubesPerUpdate;
    public GameObject timedBlockPrefab;

    private List<AnalyzedSpectrumData> _fullSpectrumDataList;
    private List<AnalyzedSpectrumData> _beatSpectrumData = new List<AnalyzedSpectrumData>();
    private TrackConfig _analyzerConfig;
    private List<double[]> _spectrumsList;
    private int _index = 0;
    private float _lastTime;
    private float _startTime;
    private System.Random _random = new System.Random();
    private float _timeframe = 2.0f;
    private bool _timeframeReached = false;
    private AudioSource _audioSource;
    private List<GameObject> _timedObjects = new List<GameObject>();
    private float _timedBlockDistance = 35;
    private GameObject _obj;
    private Rigidbody _rb;
    private ScoreTracker _scoreTracker;
    private List<EventConfig> _eventData;
    private List<NoteConfig> _noteData;
    private List<ObstacleConfig> _obstacleData;
    private List<BookmarkConfig> _bookmarkData;

    void Start()
    {
        enabled = false;

        MappingContainer mappingContainer = GlobalStorage.getInstance().MappingContainer;
        _eventData = mappingContainer.eventData;
        _noteData = mappingContainer.noteData;
        _obstacleData = mappingContainer.obstacleData;
        _bookmarkData = mappingContainer.bookmarkData;

        _fullSpectrumDataList = GlobalStorage.getInstance().SpectrumInfo;
        _analyzerConfig = GlobalStorage.getInstance().TrackConfig;
        _spectrumsList = GlobalStorage.getInstance().SpectrumsList;
        _filterBeats();

        AudioClip audioClip = GlobalStorage.getInstance().AudioClip;
        gameObject.AddComponent<AudioSource>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioSource.clip = audioClip;

        enabled = true;
        _startTime = Time.time;
    }

    void Update()
    {
        //_spawnRandomStuff();

        float timePassed = Time.time - _startTime;

        if (_timeframeReached && !_audioSource.isPlaying)
        {
            SceneManager.LoadScene("Score");
        }

        if (!_timeframeReached && timePassed > _timeframe)
        {
            _timeframeReached = true;
            _audioSource.Play();
        }

        AnalyzedSpectrumData info = _beatSpectrumData[_index];
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
    private void _handleSpectrumInfo(AnalyzedSpectrumData info)
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
            AnalyzedSpectrumData info = _fullSpectrumDataList[i];
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
        _scoreTracker = new ScoreTracker(beats);
    }

    private void _spawnRandomStuff()
    {
        for (int i = 0; i < cubesPerUpdate; i++)
        {
            GameObject cube = Instantiate<GameObject>(touchableCube, new Vector3(_random.Next(-50, 50), _random.Next(1, 50), _random.Next(-50, 50)), Quaternion.identity);
        }
    }
}
