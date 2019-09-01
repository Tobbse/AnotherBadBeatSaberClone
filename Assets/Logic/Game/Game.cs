using UnityEngine;
using UnityEngine.SceneManagement;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Collections.Generic;
using SpinnyLight;

public class Game : MonoBehaviour
{
    public static string DIFFICULTY_EASY = "Easy";
    public static string DIFFICULTY_NORMAL = "Normal";
    public static string DIFFICULTY_HARD = "Hard";
    public static string DIFFICULTY_EXPERT = "Expert";
    public static string DIFFICULTY_EXPERT_PLUS = "ExpertPlus";

    private const float MAX_TIMEFRAME_SECONDS = 5.0f;

    public GameObject leftTimedBlock;
    public GameObject rightTimedBlock;
    public GameObject leftTimedBlockNoDirection;
    public GameObject rightTimedBlockNoDirection;
    public GameObject obstacle;
    public LaserController laserController;
    public SpinnyLightController spinnyLightController;

    private List<Rigidbody> _smallSpinnerRigids = new List<Rigidbody>();
    private List<Rigidbody> _bigSpinnerRigids = new List<Rigidbody>();
    private float _timePassed;
    private float _lastTime;
    private bool _timeframeReached;
    private AudioSource _audioSource;
    private NoteSpawner _noteSpawner;
    private ObstacleSpawner _obstacleSpawner;
    private MainLightController _lightHandler;
    private float _bps;
    private float _relativeNoteTravelTime;
    private float _relativeObstacleTravelTime;
    private float _relativeTimePassed;
    
    void Start()
    {
        enabled = false;

        GameObject[] spinners = GameObject.FindGameObjectsWithTag("Spinner");
        foreach (GameObject spinner in spinners)
        {
            if (spinner.name.Contains("Small")) _smallSpinnerRigids.Add(spinner.GetComponent<Rigidbody>());
            else if (spinner.name.Contains("Big")) _bigSpinnerRigids.Add(spinner.GetComponent<Rigidbody>());
        }

        MappingContainer mappingContainer = GlobalStorage.getInstance().MappingContainer;
        if (mappingContainer.mappingInfo.bpm == 1)
        {
            _bps = 1;
        } else
        {
            _bps = mappingContainer.mappingInfo.bpm / 60;
        }
        
        _noteSpawner = new NoteSpawner(mappingContainer.noteData, _bps, leftTimedBlock, rightTimedBlock, leftTimedBlockNoDirection, rightTimedBlockNoDirection);
        _obstacleSpawner = new ObstacleSpawner(mappingContainer.obstacleData, _bps, obstacle);
        _lightHandler = new MainLightController(laserController, spinnyLightController, mappingContainer.eventData, _bps);

        _relativeNoteTravelTime = _noteSpawner.getRelativeTravelTime();
        _relativeObstacleTravelTime = _obstacleSpawner.getRelativeTravelTime();


        ScoreTracker.getInstance().NumBeats = mappingContainer.noteData.Count;
        ScoreTracker.getInstance().setupGameObjects();

        AudioClip audioClip = GlobalStorage.getInstance().AudioClip;
        gameObject.AddComponent<AudioSource>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioSource.clip = audioClip;

        // Starting point --> Start "in the past" because of block travel times. Imagine there's a note in the very beginning of the song.
        float startPoint = -1 * MAX_TIMEFRAME_SECONDS * _bps; // Not actually seconds anymore when the bps != 1.
        _timePassed = startPoint;
        _lastTime = Time.time;
        _relativeTimePassed = startPoint;

        enabled = true;
    }

    void Update()
    {
        foreach (Rigidbody smallSpinner in _smallSpinnerRigids) // Currently contains only one object, should be fine.
        {
            smallSpinner.angularVelocity = new Vector3(0.2f, 0, 0);
        }
        foreach (Rigidbody bigSpinner in _bigSpinnerRigids) // Currently contains only one object, should be fine.
        {
            bigSpinner.angularVelocity = new Vector3(0.1f, 0, 0);
        }

        float currentTime = Time.time;
        _timePassed += currentTime - _lastTime;
        _relativeTimePassed += (currentTime - _lastTime) * _bps;
        _lastTime = currentTime;

        if (!_timeframeReached && _relativeTimePassed >= 0)
        {
            _timeframeReached = true;
            _audioSource.Play();
            _audioSource.volume = 0.25f;
        } else if (_timeframeReached && !_audioSource.isPlaying)
        {
            TimedBlock[] timedBlocks = Object.FindObjectsOfType<TimedBlock>(); // Only called once.
            foreach (TimedBlock block in timedBlocks)
            {
                block.missBlock();
            }
            SceneManager.LoadScene("ScoreMenu");
        }

        _noteSpawner.checkBlocksSpawnable(_relativeTimePassed + NoteSpawner.BLOCK_TRAVEL_TIME * _bps);
        _obstacleSpawner.checkObstaclesSpawnable(_relativeTimePassed + ObstacleSpawner.OBSTACLE_TRAVEL_TIME * _bps);
        _lightHandler.checkEventsAvailable(_relativeTimePassed);
    }
}
