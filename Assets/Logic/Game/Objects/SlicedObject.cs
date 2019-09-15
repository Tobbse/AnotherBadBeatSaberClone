using UnityEngine;

namespace GameSpawnedObjects
{
    /**
     * An object that was cut and has to despawn after some time. After cutting a block, 2 of these objects will be created.
     **/
    public class SlicedObject : MonoBehaviour
    {
        private const int MAX_LIFECYCLES = 150;

        private int _lifecyles;

        void Start()
        {
            _lifecyles = MAX_LIFECYCLES;
        }

        void Update()
        {
            _lifecyles--;

            if (_lifecyles <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
