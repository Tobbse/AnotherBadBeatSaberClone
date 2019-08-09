using UnityEngine;

public class TimedBlock : MonoBehaviour
{
    public int lifetimeCycles;

    void FixedUpdate()
    {
        lifetimeCycles--;
        if (lifetimeCycles < 0 || gameObject.transform.position.x > 5)
        {
            PScoreTracker.Instance.miss();
            Destroy(gameObject);
        }
    }
}
