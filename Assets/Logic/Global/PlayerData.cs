using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerData : MonoBehaviour
{
    private static PlayerData Instance;

    private const float MAX_HEALTH = 100f;

    private float health = MAX_HEALTH;
    private TextMeshPro _hitPointsText;

    public void takeDamage(float damagePoints)
    {
        if (_hitPointsText == null) _hitPointsText = GameObject.Find("HitPointsText").GetComponent<TextMeshPro>();
        health -= damagePoints;
        ScoreTracker.getInstance().resetComboCounter();
        _hitPointsText.SetText(Mathf.CeilToInt(health).ToString() + " HP");

        if (health <= 0f)
        {
            _gameOver();
        }
    }

    void Update()
    {
        health = Mathf.Max(health + 0.1f, MAX_HEALTH);
    }

    private void _gameOver()
    {
        SceneManager.LoadScene("ScoreMenu");
    }

    public static PlayerData getInstance()
    {
        if (Instance == null)
        {
            Instance = new PlayerData();
        }
        return Instance;
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