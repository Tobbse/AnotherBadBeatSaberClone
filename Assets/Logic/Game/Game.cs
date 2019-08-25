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

    private const float MAX_TIMEFRAME = 5.0f;

    public GameObject leftTimedBlock;
    public GameObject rightTimedBlock;
    public GameObject leftTimedBlockNoDirection;
    public GameObject rightTimedBlockNoDirection;
    public GameObject obstacle;

    private List<Rigidbody> _spinnerRigids;
    private float _timePassed;
    private float _lastTime;
    private bool _timeframeReached;
    private AudioSource _audioSource;
    private NoteSpawner _noteSpawner;
    private ObstacleSpawner _obstacleSpawner;
    
    void Start()
    {
        enabled = false;

        GameObject[] spinners = GameObject.FindGameObjectsWithTag("Spinner");
        _spinnerRigids = new List<Rigidbody>();
        foreach (GameObject spinner in spinners)
        {
            _spinnerRigids.Add(spinner.GetComponent<Rigidbody>());
        }

        MappingContainer mappingContainer = GlobalStorage.getInstance().MappingContainer;
        _noteSpawner = new NoteSpawner(mappingContainer.noteData, leftTimedBlock, rightTimedBlock, leftTimedBlockNoDirection, rightTimedBlockNoDirection);
        _obstacleSpawner = new ObstacleSpawner(mappingContainer.obstacleData, obstacle);

        ScoreTracker.getInstance().NumBeats = mappingContainer.noteData.Count;
        ScoreTracker.getInstance().setupGameObjects();

        AudioClip audioClip = GlobalStorage.getInstance().AudioClip;
        gameObject.AddComponent<AudioSource>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioSource.clip = audioClip;

        _timePassed = -1 * MAX_TIMEFRAME;
        _lastTime = Time.time;

        enabled = true;
    }

    void Update()
    {
        foreach (Rigidbody spinner in _spinnerRigids) // Currently contains only one object, should be fine.
        {
            spinner.angularVelocity = new Vector3(0.15f, 0, 0);
        }

        float currentTime = Time.time;
        _timePassed += currentTime - _lastTime;
        _lastTime = currentTime;

        if (!_timeframeReached && _timePassed >= 0)
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

        _noteSpawner.checkBlocksSpawnable(_timePassed + NoteSpawner.BLOCK_TRAVEL_TIME);
        _obstacleSpawner.checkBlocksSpawnable(_timePassed + ObstacleSpawner.OBSTACLE_TRAVEL_TIME);
    }
}
