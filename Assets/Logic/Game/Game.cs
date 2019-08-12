using UnityEngine;
using UnityEngine.SceneManagement;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;

public class Game : MonoBehaviour

{
    public static string DIFFICULTY_EASY = "Easy";
    public static string DIFFICULTY_NORMAL = "Normal";
    public static string DIFFICULTY_HARD = "Hard";
    public static string DIFFICULTY_EXPERT = "Expert";
    public static string DIFFICULTY_EXPERT_PLUS = "ExpertPlus";

    public int cubesPerUpdate;
    public GameObject leftHandTimedBlockPrefab;
    public GameObject rightHandTimedBlockPrefab;
    public GameObject obstacle;

    private TrackConfig _analyzerConfig;
    private int _noteIndex = 0;
    private int _obstacleIndex = 0;
    private float _lastTime;
    private float _startTime;
    private System.Random _random = new System.Random();
    private float _timeframe = 2.2f;
    private bool _timeframeReached = false;
    private AudioSource _audioSource;
    private FastList<GameObject> _timedObjects = new FastList<GameObject>();
    private float _timedBlockDistance = 15;
    private GameObject _obj;
    private ScoreTracker _scoreTracker;
    private FastList<EventConfig> _eventData;
    private FastList<NoteConfig> _noteData;
    private FastList<ObstacleConfig> _obstacleData;
    private FastList<BookmarkConfig> _bookmarkData;
    private Dictionary<int, float> _verticalMapping = new Dictionary<int, float>();
    private Dictionary<int, float> _horizontalMapping = new Dictionary<int, float>();
    private Dictionary<int, int> _cutDirectionMapping = new Dictionary<int, int>();
    private Dictionary<int, GameObject> _blockTypeMapping = new Dictionary<int, GameObject>();

    void Start()
    {
        enabled = false;
        _setupMappings();

        MappingContainer mappingContainer = GlobalStorage.Instance.MappingContainer;
        _eventData = mappingContainer.eventData;
        _noteData = mappingContainer.noteData;
        _obstacleData = mappingContainer.obstacleData;
        _bookmarkData = mappingContainer.bookmarkData;

        PlayerData.Instance = new PlayerData();
        _scoreTracker = new ScoreTracker(mappingContainer.noteData.Count);
        _scoreTracker.setGameObjects();

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
        _verticalMapping[NoteConfig.LINE_LAYER_0] = 0.0f;
        _verticalMapping[NoteConfig.LINE_LAYER_1] = 0.4f;
        _verticalMapping[NoteConfig.LINE_LAYER_2] = 0.8f;
        _verticalMapping[NoteConfig.LINE_LAYER_3] = 1.2f;

        _horizontalMapping[NoteConfig.LINE_INDEX_0] = -0.6f;
        _horizontalMapping[NoteConfig.LINE_INDEX_1] = -0.2f;
        _horizontalMapping[NoteConfig.LINE_INDEX_2] = 0.2f;
        _horizontalMapping[NoteConfig.LINE_INDEX_3] = 0.6f;

        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_TOP] = 0;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_RIGHT] = 90;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_BOTTOM] = 180;
        _cutDirectionMapping[NoteConfig.CUT_DIRECTION_LEFT] = 270;

        _blockTypeMapping[NoteConfig.NOTE_TYPE_LEFT] = leftHandTimedBlockPrefab;
        _blockTypeMapping[NoteConfig.NOTE_TYPE_RIGHT] = rightHandTimedBlockPrefab;
    }

    void Update()
    {
        float timePassed = Time.time - _startTime;

        if (_timeframeReached && !_audioSource.isPlaying)
        {
            TimedBlock[] timedBlocks = Object.FindObjectsOfType<TimedBlock>();
            foreach (TimedBlock block in timedBlocks)
            {
                block.missBlock();
            }
            SceneManager.LoadScene("ScoreMenu");
        }

        if (timePassed > _timeframe && !_timeframeReached)
        {
            _timeframeReached = true;
            _audioSource.Play();
            _audioSource.volume = 0.25f;
        }

        for (int i = _noteIndex; i < _noteData.Count; i++)
        {
            if (_noteData[i].time <= timePassed)
            {
                _handleNote(_noteData[i]);
                _noteIndex++;
            }
            else break;
        }

        for (int i = _obstacleIndex; i < _obstacleData.Count; i++)
        {
            if (_obstacleData[i].time <= timePassed)
            {
                _handleObstacle(_obstacleData[i]);
                _obstacleIndex++;
            }
            else break;
        }
    }

    // TODO _timedBlockDistance could just be multiplied by the speed here, same with the speed!
    private void _handleNote(NoteConfig noteConfig)
    {
        Debug.Log("Note time: " + noteConfig.time.ToString());

        float xPos = _timedBlockDistance * -1;
        float yPos = 2.0f + _verticalMapping[noteConfig.lineLayer];
        float zPos = _horizontalMapping[noteConfig.lineIndex];

        GameObject prefab = _blockTypeMapping[noteConfig.type];
        _obj = Instantiate(prefab, new Vector3(xPos, yPos, zPos), Quaternion.identity);

        int angle = _cutDirectionMapping[noteConfig.cutDirection];
        _obj.transform.Rotate(new Vector3(angle, 0, 0));

        _obj.GetComponent<Rigidbody>().velocity = new Vector3(_timedBlockDistance / _timeframe, 0, 0);
    }

    private void _handleObstacle(ObstacleConfig obstacleConfig)
    {
        // TODO implement properly before using this. Currently the obstacles get destroyed when colliding with the player.
        if (!GlobalStaticSettings.USE_OBSTACLES) return;

        float length = _durationToWidth(obstacleConfig.duration);
        float xPos = (_timedBlockDistance * -1) - (length / 2);
        float yPos = 0;
        float zPos = _horizontalMapping[obstacleConfig.lineIndex];

        _obj = Instantiate(obstacle, new Vector3(xPos, yPos, zPos), Quaternion.identity);
        _obj.layer = 11;
        _obj.transform.localScale = new Vector3(length, 7.0f, obstacleConfig.width);

        _obj.GetComponent<Rigidbody>().velocity = new Vector3(_timedBlockDistance / _timeframe, 0, 0);
        int i = 0;
    }

    private float _durationToWidth(float duration)
    {
        return _timedBlockDistance / _timeframe * duration;
    }
}
