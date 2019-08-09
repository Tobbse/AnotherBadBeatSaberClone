using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    private const int MAX_HEALTH = 100;

    private int health = MAX_HEALTH;

    public void takeDamage(int damagePoints)
    {
        health -= damagePoints;
        PScoreTracker.Instance.resetComboCounter();
        if (health <= 0)
        {
            _gameOver();
        }
    }

    void Update()
    {
        health = Mathf.Max(health + 1, MAX_HEALTH);
    }

    private void _gameOver()
    {
        SceneManager.LoadScene("Score");
    }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }
}