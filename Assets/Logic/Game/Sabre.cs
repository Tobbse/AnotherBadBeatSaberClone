using UnityEngine;
using TMPro;

public class Sabre : MonoBehaviour
{
    public int blockHitLayer;

    private Vector3 _previousPosition;
    private Transform _hitTransform;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f))
        {
            _hitTransform = hit.transform;
            int hitLayer = _hitTransform.gameObject.layer;
            if (!isBlockLayer(hitLayer))
            {
                Debug.Log("Hit object from wrong layer: " + _hitTransform.name);
                return;
            }

            Rigidbody rigid = _hitTransform.GetComponent<Rigidbody>();
            rigid.velocity = Vector3.zero;
            rigid.useGravity = true;
            _hitTransform.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

            Vector3 sabreAngle = transform.position - _previousPosition;
            Vector3 blockYAxis = hit.transform.up;
            float hitAngle = Vector3.Angle(sabreAngle, blockYAxis);
            bool correctHit = false;

            //Debug.Log("Sabre Angle:   " + sabreAngle.ToString());
            //Debug.Log("Block Y Axis:  " + blockYAxis.ToString());
            //Debug.Log("Hit Angle:     " + hitAngle.ToString());

            if (hitLayer == blockHitLayer && hitAngle > 120)
            {
                PScoreTracker.Instance.hit();
                Debug.Log("Correct Hit!");
                Destroy(hit.transform.gameObject);
                correctHit = true;
            } else
            {
                PScoreTracker.Instance.miss();
                Debug.Log("Incorrect Hit!");
                Destroy(hit.transform.gameObject);
            }
            
            GameObject.Find("AngleText").GetComponent<TextMeshPro>().SetText(hitAngle.ToString());
            GameObject.Find("HitText").GetComponent<TextMeshPro>().SetText(correctHit ? "CORRECT HIT": "wrong hit");
            GameObject.Find("HitText").GetComponent<TextMeshPro>().color = correctHit ? Color.green : Color.red;
        }
        _previousPosition = transform.position;
    }

    private bool isBlockLayer(int hitLayer)
    {
        return hitLayer == 8 || hitLayer == 9;
    }
}
