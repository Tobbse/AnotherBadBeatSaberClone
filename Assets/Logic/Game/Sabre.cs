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

        // TODO cut it and make it fly away somehow.
        Rigidbody rigid = otheTransform.GetComponent<Rigidbody>();
        rigid.velocity = Vector3.zero;
        rigid.useGravity = true;

        Vector3 sabreAngle = transform.position - _previousPosition;
        Vector3 blockYAxis = otheTransform.up;
        float hitAngle = Vector3.Angle(sabreAngle, blockYAxis);
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



        Vector3 pos = transform.position - otheTransform.position;
        Vector3 direction = pos.normalized;


        GameObject[] shatters = otherObject.SliceInstantiate(new EzySlice.Plane(pos, direction),
            new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f));
        Destroy(otherObject);

        foreach (GameObject shattered in shatters)
        {
            //shattered.AddComponent<MeshCollider>().convex = true;
            shattered.AddComponent<Rigidbody>();
            Vector3 velocity = transform.GetComponent<Rigidbody>().velocity;
            velocity.x = velocity.x / 5;
            velocity.y = velocity.y / 5;
            velocity.z = velocity.z / 5;
            shattered.GetComponent<Rigidbody>().velocity = velocity;
            shattered.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
        }

        /*SlicedHull hull = otherObject.Slice(pos, direction);
        Mesh lowerHull = hull.lowerHull;
        Mesh upperHull = hull.upperHull;*/
    }

    private bool isBlockLayer(int hitLayer)
    {
        return hitLayer == 8 || hitLayer == 9;
    }
}
