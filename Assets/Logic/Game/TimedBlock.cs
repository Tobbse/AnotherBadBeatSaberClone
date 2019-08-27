using UnityEngine;

public class TimedBlock : MonoBehaviour
{
    public int lifetimeCycles;

    void FixedUpdate()
    {
        if (GlobalStaticSettings.OVERRIDE_BLOCK_DESPAWN) enabled = false;

        lifetimeCycles--;
        if (lifetimeCycles < 0 || gameObject.transform.position.x > 6) // TODO 6 is super arbitrary, check this again.
        {
            missBlock();
        }
    }

    public void missBlock()
    {
        ScoreTracker.getInstance().miss();
        Destroy(gameObject);
    }
}
