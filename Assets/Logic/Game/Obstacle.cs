using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private const float DAMAGE_POINTS = 0.3f;

    private Collider _collider;
    private Collider playerCollider;

    private void Start()
    {
        // TODO don't use find
        _collider = gameObject.GetComponent<Collider>();
        playerCollider = GameObject.Find("PlayerCollider").GetComponent<Collider>();
    }

    private void Update()
    {
        if (playerCollider.bounds.Intersects(_collider.bounds)) {
            PlayerData.getInstance().takeDamage(DAMAGE_POINTS);
        }
    }
}
