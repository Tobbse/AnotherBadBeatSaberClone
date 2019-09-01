using UnityEngine;
using EzySlice;

public class Sabre : MonoBehaviour
{
    private const float MIN_ANGLE = 120f;
    private const string NO_DIRECTION = "NoDirection";

    public int blockHitLayer;

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

        for (int i = 0; i < 30; i++)
        {
            _hapticsClip.WriteSample(0x20);
        }
    }

    void Update()
    {
        // Multiple previous positions are cached because sometimes the last position is the same. Especially the last frame's position
        // is often equal to the OnCollisionEnter position, which makes sense. I still need a direction though, so I just check the
        // previous ones. Might be fixeable with FixedUpdate?
        for (int i = _prevPositions.Length -1; i > 0; i--)
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

        float angle = Vector3.Angle(direction, collision.collider.transform.up);
        bool isNoDirectionBlock = otherObject.tag != null && otherObject.tag == NO_DIRECTION;
        if (otherObject.layer == blockHitLayer && (angle >= MIN_ANGLE || isNoDirectionBlock))
        {
            _scoreTracker.hit(); 
        } else
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
                sliced.AddComponent<Rigidbody>();
                sliced.AddComponent<SlicedObject>(); // Script that will despawn object after some time.

                float addYVelocity = (i == 0) ? 2f : 0f;

                Rigidbody rigid = sliced.GetComponent<Rigidbody>();
                rigid.angularVelocity = new Vector3(
                    Random.Range(direction.x * -10, direction.x * 10),
                    Random.Range(direction.y * -10, direction.y * 10),
                    Random.Range(direction.z * -10, direction.z * 10)
                );
                rigid.velocity = new Vector3(
                    Random.Range(direction.x * 10, direction.x * 15),
                    Random.Range(direction.y * 10, direction.y * 15 + addYVelocity), // Adding some extra upwards velocity to seperate the lower from the upper hull a bit more.
                    Random.Range(direction.z * 10, direction.z * 15)
                );
            }
        }
    }

    private void _setControllerVibration()
    {
        _hapticsChannel.Mix(_hapticsClip);
    }
}
