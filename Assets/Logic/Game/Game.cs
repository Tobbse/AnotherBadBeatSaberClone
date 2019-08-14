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
    public GameObject obstacle;

    private float _timePassed = 0;
    private bool _timeframeReached = false;
    private AudioSource _audioSource;
    private NoteSpawner _noteSpawner;
    private ObstacleSpawner _obstacleSpawner;
    
    void Start()
    {
        enabled = false;

        MappingContainer mappingContainer = GlobalStorage.Instance.MappingContainer;
        _noteSpawner = new NoteSpawner(mappingContainer.noteData, leftTimedBlock, rightTimedBlock);
        _obstacleSpawner = new ObstacleSpawner(mappingContainer.obstacleData, obstacle);

        PlayerData.Instance = new PlayerData();
        ScoreTracker scoreTracker = new ScoreTracker(mappingContainer.noteData.Count);
        scoreTracker.setGameObjects();
        ScoreTracker.Instance = scoreTracker;

        AudioClip audioClip = GlobalStorage.Instance.AudioClip;
        gameObject.AddComponent<AudioSource>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioSource.clip = audioClip;

        enabled = true;
    }

    void Update()
    {
        _timePassed += Time.deltaTime;

        if (_timeframeReached && !_audioSource.isPlaying)
        {
            TimedBlock[] timedBlocks = Object.FindObjectsOfType<TimedBlock>();
            foreach (TimedBlock block in timedBlocks)
            {
                block.missBlock();
            }
            SceneManager.LoadScene("ScoreMenu");
        }

        if (_timePassed > MAX_TIMEFRAME && !_timeframeReached)
        {
            _timeframeReached = true;
            _audioSource.Play();
            _audioSource.volume = 0.25f;
        }

        _noteSpawner.checkBlocksSpawnable(_timePassed);
        _obstacleSpawner.checkBlocksSpawnable(_timePassed);
    }
}
