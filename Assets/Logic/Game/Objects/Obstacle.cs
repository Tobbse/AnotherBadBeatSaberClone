using UnityEngine;

/**
 * Defines the behavior of obstacles. Checks if the obstacle collider intersects with the player collider bounds
 * on every update. Has to be done like this, because we don't want actual collision here, we just want to
 * know if there is an intersection to apply damage.
 **/
public class Obstacle : MonoBehaviour
{
    private const float DAMAGE_POINTS = 0.3f;

    public Collider obstacleCollider;

    private Collider _playerCollider;

    private void Start()
    {
        // Has to be done using 'Find' AFAIK because this is a prefab.
        _playerCollider = GameObject.Find("PlayerCollider").GetComponent<Collider>();
    }

    private void Update()
    {
        if (_playerCollider.bounds.Intersects(obstacleCollider.bounds)) {
            PlayerData.getInstance().takeDamage(DAMAGE_POINTS);
        }
    }
}
