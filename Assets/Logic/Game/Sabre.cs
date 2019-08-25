using UnityEngine;
using TMPro;
using EzySlice;

public class Sabre : MonoBehaviour
{
    public int blockHitLayer;

    private Vector3 _previousPosition;
    private bool _useDebugRays;

    private void Start()
    {
        _useDebugRays = GlobalStaticSettings.USE_SABRE_DEBUG_RAYS;
    }

    // Update is called once per frame
    void Update()
    {
        float length = 1.7f; // Makes it a little bit longer than the 1.1 unit long saber.
        Vector3 newPos = transform.position + (transform.forward * -0.65f); // Moving start of Ray back on the z axis of the object, so that we don't start from the middle. We want to use the whole saber.

        RaycastHit hit;
        if (_useDebugRays)
        {
            Debug.DrawRay(newPos, transform.forward * length, Color.green);
        }
        if (Physics.Raycast(newPos, transform.forward, out hit, length))
        {
            _handleHit(hit.transform);

        }
        _previousPosition = transform.position;
    }

    private void _handleHit(Transform otheTransform)
    {
        int hitLayer = otheTransform.gameObject.layer;
        GameObject otherObject = otheTransform.gameObject;
        if (!isBlockLayer(hitLayer))
        {
            // Debug.Log("Hit object from wrong layer: " + otheTransform.name);
            return;
        }

        /*Rigidbody rigid = otheTransform.GetComponent<Rigidbody>();
        rigid.velocity = Vector3.zero;
        rigid.useGravity = true;*/

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
    }

    private void sliceCube(GameObject cube, Vector3 direction)
    {
        GameObject[] slicedObjects = cube.SliceInstantiate(transform.position, direction);
        Destroy(cube);

        if (slicedObjects != null && slicedObjects.Length > 0)
        {
            foreach (GameObject sliced in slicedObjects)
            {
                sliced.AddComponent<Rigidbody>();
                sliced.AddComponent<SlicedObject>();
                Rigidbody rigid = sliced.GetComponent<Rigidbody>();
                rigid.angularVelocity = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
                rigid.velocity = getRandomVelocityFromDirection(direction);
                rigid.AddForce(Physics.gravity * rigid.mass);
            }
        }
    }

    private Vector3 getRandomVelocityFromDirection(Vector3 direction)
    {
        return new Vector3(Random.Range(direction.x * 15, direction.x * 25),
            Random.Range(direction.y * 15, direction.y * 25),
            Random.Range(direction.z * 15, direction.z * 25)
        );
    }

    private bool isBlockLayer(int hitLayer)
    {
        return hitLayer == 8 || hitLayer == 9;
    }
}
