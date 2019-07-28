using UnityEngine;

public class TimedBlock : MonoBehaviour
{
    public int lifetimeCycles;

    void FixedUpdate()
    {
        lifetimeCycles--;
        if (lifetimeCycles < 0)
        {
            PScoreTracker.Instance.miss();
            Destroy(gameObject);
        }
    }
}
