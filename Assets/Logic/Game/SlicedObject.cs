using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicedObject : MonoBehaviour
{
    private const int MAX_LIFECYCLES = 350;

    private int _lifecyles;

    // Start is called before the first frame update
    void Start()
    {
        _lifecyles = MAX_LIFECYCLES;
    }

    // Update is called once per frame
    void Update()
    {
        _lifecyles--;

        if (_lifecyles <= 0)
        {
            Destroy(gameObject);
        }
    }
}
