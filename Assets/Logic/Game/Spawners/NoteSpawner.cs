using UnityEngine;
using System.Collections.Generic;
using Audio.BeatMappingConfigs;

namespace GameSpawners
{
    /**
     * Contains note mapping data and spawns notes at the correct time. The updating is triggered from the 'Game' object,
     * which contains the main loop updating the spawner objects.
     **/
    public class NoteSpawner : ScriptableObject
    {
        public const float BLOCK_DISTANCE = 20;
        public const float BLOCK_TRAVEL_TIME = 1.5f;

        private GameObject _leftTimedBlock;
        private GameObject _rightTimedBlock;
        private GameObject _leftTimedBlockNoDirection;
        private GameObject _rightTimedBlockNoDirection;
        private Transform _generated;
        private GameObject _obj;
        private List<NoteConfig> _noteData;
        private List<GameObject> _blocks = new List<GameObject>();
        private NoteConfig _cfg;
        private float _bps;
        private float _relativeTravelTime;
        private float _speed;

        public NoteSpawner(List<NoteConfig> noteData, float bps, GameObject leftTimedBlock, GameObject rightTimedBlock,
            GameObject leftTimedBlockNoDirection, GameObject rightTimedBlockNoDirection, Transform generated)
        {
            _noteData = noteData;
            _bps = bps;
            _leftTimedBlock = leftTimedBlock;
            _rightTimedBlock = rightTimedBlock;
            _leftTimedBlockNoDirection = leftTimedBlockNoDirection;
            _rightTimedBlockNoDirection = rightTimedBlockNoDirection;
            _generated = generated;

            _relativeTravelTime = BLOCK_TRAVEL_TIME /*/ _bps*/;
            _speed = BLOCK_DISTANCE / _relativeTravelTime;
        }

        public void checkBlocksSpawnable(float timePassed)
        {
            while (_noteData.Count > 0)
            {
                _cfg = _noteData[0];
                if (_cfg.Time <= timePassed)
                {
                    _spawnNote(_cfg);
                    _noteData.RemoveAt(0);
                }
                else break;
            }
        }

        public List<GameObject> getBlocks()
        {
            return _blocks;
        }

        public float getRelativeTravelTime()
        {
            return _relativeTravelTime;
        }

        // Spawns a note object and sets the values of that object, according to the note config.
        private void _spawnNote(NoteConfig noteConfig)
        {
            GameObject prefab = noteConfig.Type == NoteConfig.NOTE_TYPE_LEFT ? _leftTimedBlock : _rightTimedBlock;
            Vector3 position = new Vector3(
                BLOCK_DISTANCE * -1,
                2.0f + ObjectSpawnPositionProvider.getVerticalPosition(noteConfig.LineLayer),
                ObjectSpawnPositionProvider.getHorizontalPosition(noteConfig.LineIndex)
            );

            int cutDirection = ObjectSpawnPositionProvider.getCutDirection(noteConfig.CutDirection);
            if (cutDirection == ObjectSpawnPositionProvider.getCutDirection(NoteConfig.CUT_DIRECTION_NONE))
            {
                prefab = prefab == _leftTimedBlock ? _leftTimedBlockNoDirection : _rightTimedBlockNoDirection;
                cutDirection = 0;
            }

            _obj = Instantiate(prefab, position, Quaternion.identity);
            _obj.transform.parent = _generated;
            _obj.transform.Rotate(new Vector3(cutDirection, 0, 0));
            _obj.GetComponent<Rigidbody>().velocity = new Vector3(_speed, 0, 0);
            _blocks.Add(_obj);
        }
    }

}