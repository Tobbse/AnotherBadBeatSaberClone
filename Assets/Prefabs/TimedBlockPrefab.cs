using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBlockPrefab : MonoBehaviour
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
