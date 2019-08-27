using UnityEngine;
using TMPro;
using EzySlice;

public class Sabre : MonoBehaviour
{
    public int blockHitLayer;

    // TODO check if those angles make sense.
    private const float MIN_ANGLE = 120f;

    private Vector3 _prevPos;
    private Vector3 _prevPrevPos;
    private ScoreTracker _scoreTracker;

    private void Start()
    {
        _scoreTracker = ScoreTracker.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        float length = 1.7f; // Makes it a little bit longer than the 1.1 unit long saber.
        Vector3 newPos = transform.position + (transform.forward * -0.65f); // Moving start of Ray back on the z axis of the object, so that we don't start from the middle. We want to use the whole saber.

        /*RaycastHit hit;
        if (GlobalStaticSettings.USE_SABRE_DEBUG_RAYS)
        {
            Debug.DrawRay(newPos, transform.forward * length, Color.green);
        }
        if (Physics.Raycast(newPos, transform.forward, out hit, length))
        {
            _handleHit(hit.transform);

        }*/
        _prevPrevPos = _prevPos;
        _prevPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherObject = collision.gameObject;

        Vector3 direction;
        Vector3 currentPos = transform.position;
        if (_prevPos != currentPos)
        {
            direction = currentPos - _prevPos;
        } else
        {
            direction = currentPos - _prevPrevPos;
        }
        float angle = Vector3.Angle(direction, collision.collider.transform.up);
        ContactPoint con = collision.GetContact(0);
        Debug.Log("-----------------------");
        Debug.Log(con.point.x);
        Debug.Log(con.point.y);
        Debug.Log(con.point.z);

        //bool isUpperHit = Vector3.Angle(con.normal, collision.collider.transform.up) < 20;


        if (otherObject.layer != blockHitLayer || angle < MIN_ANGLE)
        {
            _scoreTracker.miss();
        } else if (true) // When angle is correct
        {
            _scoreTracker.hit();
        }

        _sliceCube(otherObject, direction);
        //_sliceCube(otherObject, collision.relativeVelocity); // Check if this works fine.
    }

    private void _handleMiss()
    {
        ScoreTracker.getInstance().miss();
    }

    private void _handleHit()
    {
        ScoreTracker.getInstance().hit();
    }

    /*private void _handleHit(Transform otheTransform)
    {
        int hitLayer = otheTransform.gameObject.layer;
        GameObject otherObject = otheTransform.gameObject;
        if (!isBlockLayer(hitLayer))
        {
            // Debug.Log("Hit object from wrong layer: " + otheTransform.name);
            return;
        }

        Vector3 direction = transform.position - _previousPosition;
        Vector3 blockYAxis = otheTransform.up;
        float hitAngle = Vector3.Angle(direction, blockYAxis);
        bool correctHit = false;

        // TODO check if this works with the no direction blocks
        if (hitLayer == blockHitLayer && (hitAngle > 120 || otherObject.name.Contains("NoDirection")))
        {
            ScoreTracker.getInstance().hit();
            correctHit = true;
        }
        else
        {
            ScoreTracker.getInstance().miss();
        }

        // TODO this is only for debugging, delete this later or do it properly.
        GameObject obj = GameObject.Find("AngleText");
        if (obj != null) obj.GetComponent<TextMeshPro>().SetText(hitAngle.ToString());
        obj = GameObject.Find("HitText");
        if (obj != null)
        {
            obj.GetComponent<TextMeshPro>().SetText(correctHit ? "CORRECT HIT" : "wrong hit");
            obj.GetComponent<TextMeshPro>().color = correctHit ? Color.green : Color.red;
        }

        sliceCube(otherObject, direction);
    }*/

    private void _sliceCube(GameObject cube, Vector3 direction)
    {
        GameObject[] slicedObjects = cube.SliceInstantiate(transform.position, direction);
        Destroy(cube);

        if (slicedObjects != null && slicedObjects.Length > 0)
        {
            foreach (GameObject sliced in slicedObjects)
            {
                sliced.AddComponent<Rigidbody>();
                sliced.AddComponent<SlicedObject>(); // Script that will despawn object after some time.

                Rigidbody rigid = sliced.GetComponent<Rigidbody>();
                rigid.angularVelocity = new Vector3(
                    Random.Range(direction.x * 5, direction.x * 15),
                    Random.Range(direction.y * 5, direction.y * 15),
                    Random.Range(direction.z * 5, direction.z * 15)
                );
                rigid.velocity = getRandomVelocityFromDirection(direction);
                rigid.AddForce(Physics.gravity * rigid.mass);
            }
        }
    }

    private Vector3 getRandomVelocityFromDirection(Vector3 direction)
    {
        return new Vector3(Random.Range(direction.x * 25, direction.x * 50),
            Random.Range(direction.y * 25, direction.y * 50),
            Random.Range(direction.z * 25, direction.z * 50)
        );
    }

    /*private bool isBlockLayer(int hitLayer)
    {
        return hitLayer == 8 || hitLayer == 9;
    }*/
}
