using UnityEngine;

public class TouchableCube : MonoBehaviour
{
    public int lifetimeCycles;

    void FixedUpdate()
    {
        lifetimeCycles--;
        if (lifetimeCycles < 0 || gameObject.transform.position.x > 0)
        {
            Debug.Log("You fucked up!");
            PScoreTracker.Instance.miss();
            Destroy(gameObject);
        }
    }
}
