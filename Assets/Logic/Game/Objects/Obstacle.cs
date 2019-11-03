using UnityEngine;
using Global;

namespace GameSpawnedObjects
{
    /// <summary>
    /// Defines the behavior of obstacles.Checks if the obstacle collider intersects with the player collider bounds
    /// on every update. Has to be done like this, because we don't want actual collision here, we just want to
    /// know if there is an intersection to apply damage.
    /// Also makes sure that the obstacle despawns after a while.
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        private const float OBSTACLE_DAMAGE = 0.4f;  // Applied for every collision update.

        public Collider obstacleCollider;

        private Collider _playerCollider;
        private int lifetimeCycles = 650;

        private void Start()
        {
            _playerCollider = GameObject.Find("PlayerCollider").GetComponent<Collider>();
        }

        private void FixedUpdate()
        {
            if (_playerCollider.bounds.Intersects(obstacleCollider.bounds))
            {
                PlayerData.getInstance().takeDamage(OBSTACLE_DAMAGE);
            }
            lifetimeCycles--;
            if (lifetimeCycles < 0 || gameObject.transform.position.x > 100)
            {
                Destroy(gameObject);
            }
        }
    }

}
