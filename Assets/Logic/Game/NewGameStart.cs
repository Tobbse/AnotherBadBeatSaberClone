using UnityEngine;
using UnityEngine.SceneManagement;
using PAnalyzerConfigs;
using PMappingConfigs;
using System.Collections.Generic;

public class NewGameStart : MonoBehaviour
{
    public GameObject touchableCube;
    public int cubesPerUpdate;
    public GameObject leftHandTimedBlockPrefab;
    public GameObject rightHandTimedBlockPrefab;

    private TrackConfig _analyzerConfig;
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
    private PScoreTracker _scoreTracker;
    private FastList<PEventConfig> _eventData;
    private FastList<PNoteConfig> _noteData;
    private FastList<PObstacleConfig> _obstacleData;
    private FastList<PBookmarkConfig> _bookmarkData;
    private Dictionary<int, float> _verticalMapping = new Dictionary<int, float>();
    private Dictionary<int, float> _horizontalMapping = new Dictionary<int, float>();
    private Dictionary<int, int> _cutDirectionMapping = new Dictionary<int, int>();
    private Dictionary<int, int> _layerMapping = new Dictionary<int, int>();
    private Dictionary<int, GameObject> _blockTypeMapping = new Dictionary<int, GameObject>();

    void Start()
    {
        enabled = false;
        _setupMappings();

        PMappingContainer mappingContainer = GlobalStorage.Instance.MappingContainer;
        _eventData = mappingContainer.eventData;
        _noteData = mappingContainer.noteData;
        _obstacleData = mappingContainer.obstacleData;
        _bookmarkData = mappingContainer.bookmarkData;
        _scoreTracker = new PScoreTracker(mappingContainer.noteData.Count);

        _analyzerConfig = GlobalStorage.Instance.TrackConfig;

        AudioClip audioClip = GlobalStorage.Instance.AudioClip;
        gameObject.AddComponent<AudioSource>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioSource.clip = audioClip;

        enabled = true;
        _startTime = Time.time;
    }

    private void _setupMappings()
    {
        _verticalMapping[PNoteConfig.LINE_LAYER_0] = 0.0f;
        _verticalMapping[PNoteConfig.LINE_LAYER_1] = 0.4f;
        _verticalMapping[PNoteConfig.LINE_LAYER_2] = 0.8f;
        _verticalMapping[PNoteConfig.LINE_LAYER_3] = 1.2f;

        _horizontalMapping[PNoteConfig.LINE_INDEX_0] = -0.6f;
        _horizontalMapping[PNoteConfig.LINE_INDEX_1] = -0.2f;
        _horizontalMapping[PNoteConfig.LINE_INDEX_2] = 0.2f;
        _horizontalMapping[PNoteConfig.LINE_INDEX_3] = 0.6f;

        _cutDirectionMapping[PNoteConfig.CUT_DIRECTION_TOP] = 0;
        _cutDirectionMapping[PNoteConfig.CUT_DIRECTION_RIGHT] = 90;
        _cutDirectionMapping[PNoteConfig.CUT_DIRECTION_BOTTOM] = 180;
        _cutDirectionMapping[PNoteConfig.CUT_DIRECTION_LEFT] = 270;

        _layerMapping[PNoteConfig.NOTE_TYPE_LEFT] = 8;
        _layerMapping[PNoteConfig.NOTE_TYPE_RIGHT] = 9;

        _blockTypeMapping[PNoteConfig.NOTE_TYPE_LEFT] = leftHandTimedBlockPrefab;
        _blockTypeMapping[PNoteConfig.NOTE_TYPE_RIGHT] = rightHandTimedBlockPrefab;
    }

    void Update()
    {
        float timePassed = Time.time - _startTime;

        if (_timeframeReached && !_audioSource.isPlaying)
        {
            TimedBlock[] timedBlocks = Object.FindObjectsOfType<TimedBlock>();
            foreach (TimedBlock block in timedBlocks)
            {
                GameObject.Destroy(block);
            }
            SceneManager.LoadScene("Score");
        }

        if (timePassed > _timeframe && !_timeframeReached)
        {
            _timeframeReached = true;
            _audioSource.Play();
            _audioSource.volume = 0.25f;
        }

        for (int i = _index; i < _noteData.Count; i++)
        {
            if (_noteData[i].time <= timePassed)
            {
                _handleNote(_noteData[i]);
                _index++;
            } else
            {
                break;
            }
        }
    }

    // TODO _timedBlockDistance could just be multiplied by the speed here, same with the speed!
    private void _handleNote(PNoteConfig noteConfig)
    {
        float xPos = _timedBlockDistance * -1;
        float yPos = 1.5f + _verticalMapping[noteConfig.lineLayer];
        float zPos = _horizontalMapping[noteConfig.lineIndex];

        GameObject prefab = _blockTypeMapping[noteConfig.type];
        _obj = Instantiate(prefab, new Vector3(xPos, yPos, zPos), Quaternion.identity);
        //_obj.layer = _layerMapping[noteConfig.type];

        int angle = _cutDirectionMapping[noteConfig.cutDirection];
        _obj.transform.Rotate(new Vector3(angle, 0, 0));

        _rb = _obj.GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(_timedBlockDistance / _timeframe, 0, 0);
    }
}
