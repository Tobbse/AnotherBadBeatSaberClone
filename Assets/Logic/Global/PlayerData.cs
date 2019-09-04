using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerData : ScriptableObject
{
    private static PlayerData Instance;

    private const float MAX_HEALTH = 100f;

    private float _health;
    private TextMeshPro _hitPointsText;
    private RawImage _damageOverlay;

    public void takeDamage(float damagePoints)
    {
        if (GlobalStaticSettings.TAKE_DAMAGE == false) return;

        _setHealth(_health - damagePoints);
        ScoreTracker.getInstance().resetComboCounter();

        // Todo fix this
        // GAME OVER SCREEN? GAME OVER SOUND?
        // MAYBE JUST CHANGE THE HEADLINE OF THE SCORE SCREEN OR STH.
        if (_health <= 0f)
        {
            _gameOver();
        } else if (_hitPointsText == null)
        {
            _hitPointsText = GameObject.Find("HitPointsText").GetComponent<TextMeshPro>();
        } else if (_hitPointsText != null)
        {
            _hitPointsText.SetText(Mathf.CeilToInt(_health).ToString() + " HP");
        }
    }

    private void _setHealth(float value)
    {
        _health = value;
        Color color = _damageOverlay.color;
        _damageOverlay.color = new Color(color.r, color.g, color.b, (MAX_HEALTH - _health) / MAX_HEALTH);  // Will set alpha to 0 when full HP and 1 when 0 HP.
    }

    void Update()
    {
        //_setHealth(Mathf.Max(_health + 0.1f, MAX_HEALTH));
        _setHealth(50);

        // TODO fix this too
        if (_hitPointsText == null)
        {
            _hitPointsText = GameObject.Find("HitPointsText").GetComponent<TextMeshPro>();
        }
        else if (_hitPointsText != null)
        {
            _hitPointsText.SetText(Mathf.CeilToInt(_health).ToString() + " HP");
        }
    }

    void Start()
    {
        GameObject overlayObject = GameObject.Find("DamageOverlay");
        overlayObject.SetActive(true);
        _damageOverlay = overlayObject.GetComponent<RawImage>();

        _setHealth(MAX_HEALTH);

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