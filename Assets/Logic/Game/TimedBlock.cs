using UnityEngine;

public class TimedBlock : MonoBehaviour
{
    public int lifetimeCycles;

    void FixedUpdate()
    {
        lifetimeCycles--;
        if (lifetimeCycles < 0 || gameObject.transform.position.x > 3) // TODO 3 is kind of arbitrary, check this again.
        {
            missBlock();
        }
    }

    public void missBlock()
    {
        PScoreTracker.Instance.miss();
        Destroy(gameObject);
    }
}
