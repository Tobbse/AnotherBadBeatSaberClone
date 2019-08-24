using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private const float DAMAGE_POINTS = 0.3f;

    private Collider _collider;
    private Collider _playerTrackingSpace;

    private void Start()
    {
        // TODO don't use find
        _collider = gameObject.GetComponent<Collider>();
        _playerTrackingSpace = GameObject.Find("TrackingSpace").GetComponent<Collider>();
    }

    private void Update()
    {
        if (_playerTrackingSpace.bounds.Intersects(_collider.bounds)) {
            PlayerData.getInstance().takeDamage(DAMAGE_POINTS);
        }
    }
}
