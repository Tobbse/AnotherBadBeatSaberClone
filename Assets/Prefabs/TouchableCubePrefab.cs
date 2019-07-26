using UnityEngine;

public class TouchableCube : MonoBehaviour
{
    public int lifetimeCycles;

    void FixedUpdate()
    {
        lifetimeCycles--;
        if (lifetimeCycles < 0)
        {
            Destroy(gameObject);
        }
    }
}
