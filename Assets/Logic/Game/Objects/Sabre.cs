using UnityEngine;
using EzySlice;
using Global;

namespace GameSpawnedObjects
{
    /// <summary>
    /// The Sabre behavior.This includes hitting, missing as well as slicing them
    /// and creating sliced objects from sliced notes (using an external library) and vibrations.
    /// </summary>
    public class Sabre : MonoBehaviour
    {
        private const float MIN_ANGLE = 120f;
        private const string NO_DIRECTION = "NoDirection";
        private const byte VIBRATION_STRENGTH = 0x50;

        public int blockHitLayer;
        public Transform generated;

        private ScoreTracker _scoreTracker;
        private Vector3[] _prevPositions;
        private OVRHapticsClip _hapticsClip;
        private OVRHaptics.OVRHapticsChannel _hapticsChannel;

        private void Start()
        {
            _scoreTracker = ScoreTracker.getInstance();
            _prevPositions = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
            _hapticsClip = new OVRHapticsClip();
            _hapticsChannel = (blockHitLayer == 8) ? OVRHaptics.LeftChannel : OVRHaptics.RightChannel;

            // The OVRHapticsClip is used to play vibrations on the controllers.
            // The amount of samples written (or the length of loop) determines the length of the vibration,
            // the written values define the strength of the vibration.
            for (int i = 0; i < 30; i++)
            {
                _hapticsClip.WriteSample(VIBRATION_STRENGTH);
            }
        }

        void Update()
        {
            // Multiple previous positions are cached because sometimes the last position is the same as the current one.
            // Especially the last frame's position is often equal to the OnCollisionEnter position, which makes sense, but
            // I still need a direction though, so I just check the previous ones.
            for (int i = _prevPositions.Length - 1; i > 0; i--)
            {
                _prevPositions[i] = _prevPositions[i - 1];
            }
            _prevPositions[0] = transform.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            GameObject otherObject = collision.gameObject;
            Vector3 direction = Vector3.zero;
            Vector3 currentPos = transform.position;

            foreach (Vector3 prevPos in _prevPositions)
            {
                if (prevPos != currentPos)
                {
                    direction = currentPos - prevPos;
                    break;
                }
            }

            // NO_DIRECTION blocks do not depend on the hit angle.
            // Otherwise the hit angle depends if the hit was correct (score) or incorrect (miss).
            float angle = Vector3.Angle(direction, collision.collider.transform.up);
            bool isNoDirectionBlock = otherObject.tag != null && otherObject.tag == NO_DIRECTION;
            if (otherObject.layer == blockHitLayer && (angle >= MIN_ANGLE || isNoDirectionBlock))
            {
                _scoreTracker.hit();
            }
            else
            {
                _scoreTracker.miss();
            }

            _sliceCube(otherObject, direction);
            _setControllerVibration();
        }

        private void _handleMiss()
        {
            ScoreTracker.getInstance().miss();
        }

        private void _handleHit()
        {
            ScoreTracker.getInstance().hit();
        }

        // The library EzySlice is used to cut the cubes in half and create two new objects from the block.
        // Some speed and rotation values are applied to these new objects, to make them fly away.
        private void _sliceCube(GameObject cube, Vector3 direction)
        {
            GameObject[] slicedObjects = cube.SliceInstantiate(transform.position, direction);
            Destroy(cube);

            if (slicedObjects != null && slicedObjects.Length > 0)
            {
                GameObject sliced;
                for (int i = 0; i < slicedObjects.Length; i++)
                {
                    sliced = slicedObjects[i];
                    sliced.transform.parent = generated;
                    sliced.AddComponent<Rigidbody>();
                    sliced.AddComponent<SlicedObject>(); // Script that will despawn object after some time.

                    float addedYVelocity = (i == 0) ? 2f : 0f;

                    Rigidbody rigid = sliced.GetComponent<Rigidbody>();
                    rigid.angularVelocity = new Vector3(
                        Random.Range(direction.x * -5, direction.x * 5),
                        Random.Range(direction.y * -5, direction.y * 5),
                        Random.Range(direction.z * -5, direction.z * 5)
                    );
                    rigid.velocity = new Vector3(
                        Random.Range(direction.x * 10, direction.x * 15),
                        Random.Range(direction.y * 1, direction.y * 2 + addedYVelocity), // Adding some extra upwards velocity to seperate the lower from the upper hull a bit more. Looks nicer.
                        Random.Range(direction.z * 5, direction.z * 10)
                    );
                }
            }
        }

        // Adds vibration to the left or right controller.
        private void _setControllerVibration()
        {
            _hapticsChannel.Mix(_hapticsClip);
        }
    }

}
