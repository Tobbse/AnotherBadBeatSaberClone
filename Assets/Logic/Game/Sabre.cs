using UnityEngine;
using TMPro;

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
        if (!isBlockLayer(hitLayer))
        {
            Debug.Log("Hit object from wrong layer: " + otheTransform.name);
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

        if (hitLayer == blockHitLayer && hitAngle > 120)
        {
            ScoreTracker.getInstance().hit();
            Debug.Log("Correct Hit!");
            Destroy(otheTransform.gameObject);
            correctHit = true;
        }
        else
        {
            ScoreTracker.getInstance().miss();
            Debug.Log("Incorrect Hit!");
            Destroy(otheTransform.gameObject);
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
    }

    private bool isBlockLayer(int hitLayer)
    {
        return hitLayer == 8 || hitLayer == 9;
    }
}
