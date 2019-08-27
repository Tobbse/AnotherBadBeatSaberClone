using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerData : ScriptableObject
{
    private static PlayerData Instance;

    private const float MAX_HEALTH = 100f;

    private float health = MAX_HEALTH;
    private TextMeshPro _hitPointsText;

    public void takeDamage(float damagePoints)
    {
        if (GlobalStaticSettings.TAKE_DAMAGE == false) return;

        health -= damagePoints;
        ScoreTracker.getInstance().resetComboCounter();

        // Todo fix this
        if (health <= 0f)
        {
            _gameOver();
        } else if (_hitPointsText == null)
        {
            _hitPointsText = GameObject.Find("HitPointsText").GetComponent<TextMeshPro>();
        } else if (_hitPointsText != null)
        {
            _hitPointsText.SetText(Mathf.CeilToInt(health).ToString() + " HP");
        }
    }

    void Update()
    {
        health = Mathf.Max(health + 0.1f, MAX_HEALTH);

        // TODO fix this too
        if (_hitPointsText == null)
        {
            _hitPointsText = GameObject.Find("HitPointsText").GetComponent<TextMeshPro>();
        }
        else if (_hitPointsText != null)
        {
            _hitPointsText.SetText(Mathf.CeilToInt(health).ToString() + " HP");
        }
    }

    void Start()
    {
        if (GlobalStaticSettings.TAKE_DAMAGE == false)
        {
            _hitPointsText = GameObject.Find("HitPointsText").GetComponent<TextMeshPro>();
            if (_hitPointsText != null)
            {
                _hitPointsText.text = "INVINCIBLE!!!";
            }
        }
    }

    private void _gameOver()
    {
        SceneManager.LoadScene("ScoreMenu"); // TODO there should be some kind of game over screen.
    }

    public static PlayerData getInstance()
    {
        if (Instance == null)
        {
            return new PlayerData();
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