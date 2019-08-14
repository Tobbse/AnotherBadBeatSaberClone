using UnityEngine;

public class Obstacle : MonoBehaviour
{
    GameObject _player;

    private void Start()
    {
        // TODO don't use find
        _player = GameObject.Find("Player");
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.name == "Player" || collision.transform.IsChildOf(_player.transform))
        {
            PlayerData.Instance.takeDamage(5);
            GameObject.Destroy(gameObject);
        }
    }
}
