using UnityEngine;
using UnityEngine.SceneManagement;
using Audio.BeatMappingConfigs;
using System.Collections.Generic;
using GameEffects;
using GameSpawners;
using Global;
using GameSpawnedObjects;

namespace Game
{
    /**
     * Contains the main game loop.
     * Holds controllers for notes, obstacles and events/effects and passes the mapping data to them.
     * Calls those controller objects on each update to check if there is for example a new note that
     * has to be spawned or an effect that should be triggered, depending on the current time value and
     * the time values saved in the mapping configs.
     * 
     * Also handles timing depending on the bpm of the song, as distances and timings change according to the bpm.
     */
    public class GameController : MonoBehaviour
    {
        public const string DIFFICULTY_EASY = "Easy";
        public const string DIFFICULTY_NORMAL = "Normal";
        public const string DIFFICULTY_HARD = "Hard";
        public const string DIFFICULTY_EXPERT = "Expert";
        public const string DIFFICULTY_EXPERT_PLUS = "ExpertPlus";

        public GameObject leftTimedBlock;
        public GameObject rightTimedBlock;
        public GameObject leftTimedBlockNoDirection;
        public GameObject rightTimedBlockNoDirection;
        public GameObject obstacle;
        public Transform generated;
        public LaserController laserController;
        public SpinnerLightController spinnerLightController;
        public FogController fogController;

        private const float MAX_TIMEFRAME_SECONDS = 5.0f;

        private List<Rigidbody> _smallSpinnerRigids = new List<Rigidbody>();
        private List<Rigidbody> _bigSpinnerRigids = new List<Rigidbody>();
        private float _timePassed;
        private float _lastTime;
        private bool _timeframeReached;
        private AudioSource _audioSource;
        private NoteSpawner _noteSpawner;
        private ObstacleSpawner _obstacleSpawner;
        private MainEffectController _effectController;
        private float _bps;
        private float _relativeNoteTravelTime;
        private float _relativeObstacleTravelTime;
        private float _relativeTimePassed;

        // Preparing controllers and passing data to them.
        // Preparing score tracking and the audio clip.
        // Calculates the start point.
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
            if (mappingContainer.MappingInfo.Bpm == 1)
            {
                _bps = 1;
            }
            else
            {
                _bps = mappingContainer.MappingInfo.Bpm / 60;
            }

            _noteSpawner = new NoteSpawner(mappingContainer.NoteData, _bps, leftTimedBlock, rightTimedBlock,
                leftTimedBlockNoDirection, rightTimedBlockNoDirection, generated);
            _obstacleSpawner = new ObstacleSpawner(mappingContainer.ObstacleData, _bps, obstacle, generated);
            _effectController = new MainEffectController(laserController, spinnerLightController,
                fogController, mappingContainer.EventData, _bps);

            _relativeNoteTravelTime = _noteSpawner.getRelativeTravelTime();
            _relativeObstacleTravelTime = _obstacleSpawner.getRelativeTravelTime();


            ScoreTracker.getInstance().NumBeats = mappingContainer.NoteData.Count;
            ScoreTracker.getInstance().setupGameObjects();

            AudioClip audioClip = GlobalStorage.getInstance().AudioClip;
            gameObject.AddComponent<AudioSource>();
            _audioSource = gameObject.GetComponent<AudioSource>();
            _audioSource.clip = audioClip;

            // Starting point --> Start "in the past" because of block travel times. Imagine there's a note in the very beginning of the song, that wouldn't work otherwise.
            float startPoint = -1 * MAX_TIMEFRAME_SECONDS * _bps; // Not actually seconds anymore when the bps != 1.
            _timePassed = startPoint;
            _lastTime = Time.time;
            _relativeTimePassed = startPoint;

            enabled = true;
        }

        void FixedUpdate()
        {
            // Not ideal I guess, but currently contains _bigSpinnerRigids and _smallSpinnerRigids contain only one rigid, so that should be fine.
            foreach (Rigidbody smallSpinner in _smallSpinnerRigids)
            {
                smallSpinner.angularVelocity = new Vector3(0.2f, 0, 0);
            }
            foreach (Rigidbody bigSpinner in _bigSpinnerRigids)
            {
                bigSpinner.angularVelocity = new Vector3(0.1f, 0, 0);
            }

            // Calculates the relative time that has passed depending on the bpm values.
            float currentTime = Time.time;
            _timePassed += currentTime - _lastTime;
            _relativeTimePassed += (currentTime - _lastTime) * _bps;
            _lastTime = currentTime;

            // Triggers playing of audio file once, at the correct point in time.
            if (!_timeframeReached && _relativeTimePassed >= 0)
            {
                _timeframeReached = true;
                _audioSource.Play();
                _audioSource.volume = 0.25f;
            }
            else if (_timeframeReached && !_audioSource.isPlaying)
            {
                // End of loop, only called once so the 'FindObjectsOfType' call is fine. Leftover blocks have to be missed.
                TimedBlock[] timedBlocks = Object.FindObjectsOfType<TimedBlock>();
                foreach (TimedBlock block in timedBlocks)
                {
                    block.MissBlock();
                }
                SceneManager.LoadScene("ScoreMenu");
            }

            // Checks if new events/blocks/obstacles are available and should be spawned, depending on the relative time passed.
            // The travel time has to be taken into account here, because we want the blocks to spawn before they arrive at the player.
            _noteSpawner.checkBlocksSpawnable(_relativeTimePassed + NoteSpawner.BLOCK_TRAVEL_TIME * _bps);
            _obstacleSpawner.checkObstaclesSpawnable(_relativeTimePassed + ObstacleSpawner.OBSTACLE_TRAVEL_TIME * _bps);
            _effectController.checkEventsAvailable(_relativeTimePassed);
        }
    }

}
