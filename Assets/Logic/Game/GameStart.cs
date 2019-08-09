using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using PSpectrumInfo;
using PAnalyzerConfigs;
using PMappingConfigs;

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
    private FastList<PEventConfig> _eventData;
    private FastList<PNoteConfig> _noteData;
    private FastList<PObstacleConfig> _obstacleData;
    private FastList<PBookmarkConfig> _bookmarkData;

    void Start()
    {
        enabled = false;

        PMappingContainer mappingContainer = GlobalStorage.Instance.MappingContainer;
        _eventData = mappingContainer.eventData;
        _noteData = mappingContainer.noteData;
        _obstacleData = mappingContainer.obstacleData;
        _bookmarkData = mappingContainer.bookmarkData;

        _fullSpectrumDataList = GlobalStorage.Instance.SpectrumInfo;
        _analyzerConfig = GlobalStorage.Instance.TrackConfig;
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

    private void _spawnRandomStuff()
    {
        for (int i = 0; i < cubesPerUpdate; i++)
        {
            GameObject cube = Instantiate<GameObject>(touchableCube, new Vector3(_random.Next(-50, 50), _random.Next(1, 50), _random.Next(-50, 50)), Quaternion.identity);
        }
    }
}
