using UnityEngine;

public class Sabre : MonoBehaviour
{
    public int _blockHitLayer;

    private Vector3 _previousPosition;
    private Transform _hitTransform;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f))
        {
            _hitTransform = hit.transform;
            Vector3 direction = transform.position - _previousPosition;
            float angle = Vector3.Angle(transform.position - _previousPosition, _hitTransform.up);
            int hitLayer = _hitTransform.gameObject.layer;


            if (/*angle > 130 && */hitLayer == _blockHitLayer)
            {
                PScoreTracker.Instance.hit();
                Debug.Log("You hit something!!!");
                Destroy(hit.transform.gameObject);
            }
             else if (isBlockLayer(hitLayer))
            {
                PScoreTracker.Instance.hit();
                Debug.Log("You hit a wrong block!!!");
                Destroy(hit.transform.gameObject);
            }
        }
        _previousPosition = transform.position;
    }

    private bool isBlockLayer(int hitLayer)
    {
        return hitLayer == 8 || hitLayer == 9;
    }
}
