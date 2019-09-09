using UnityEngine;

/**
 * Behavior for the Note blocks, making sure that they despawn after a while or when they reach a certain position behind the player.
 **/
public class TimedBlock : MonoBehaviour
{
    private int lifetimeCycles = 400;

    void FixedUpdate()
    {
        if (GlobalSettings.OVERRIDE_BLOCK_DESPAWN) enabled = false;

        lifetimeCycles--;
        if (lifetimeCycles < 0 || gameObject.transform.position.x > 5)
        {
            MissBlock();
        }
    }

    public void MissBlock()
    {
        ScoreTracker.getInstance().miss();
        Destroy(gameObject);
    }
}
