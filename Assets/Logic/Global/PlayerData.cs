using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Singleton that contains player data, like player health. Regenerates a small amount of health on update.
 * Exposes API to take damage. Calls Game Over when the health reaches 0.
 **/
public class PlayerData : ScriptableObject
{
    private static PlayerData Instance;

    private const float MAX_HEALTH = 100f;
    private const float REGENERATION_RATE = 0.2f;

    private float _health;
    private bool _isGameOver;
    private RawImage _damageOverlay;

    public bool IsGameOver { get => _isGameOver; set => _isGameOver = value; }

    public PlayerData()
    {
        GameObject overlayObject = GameObject.Find("DamageOverlay");
        overlayObject.SetActive(true);
        _damageOverlay = overlayObject.GetComponent<RawImage>();
        _damageOverlay.color = _getOverlayColorWithNewAlpha(_damageOverlay.color);

        _setHealth(MAX_HEALTH);
    }

    public void reset()
    {
        _isGameOver = false;
        _health = MAX_HEALTH;
        _damageOverlay.color = _getOverlayColorWithNewAlpha(_damageOverlay.color);
    }

    public void takeDamage(float damagePoints)
    {
        if (GlobalSettings.TAKE_DAMAGE == false) return;

        _setHealth(_health - damagePoints);
        ScoreTracker.getInstance().resetComboCounter();

        if (_health <= 0f)
        {
            _isGameOver = true;
            _gameOver();
        }
    }

    private void _setHealth(float value)
    {
        _health = value;
        _damageOverlay.color = _getOverlayColorWithNewAlpha(_damageOverlay.color);
    }

    void Update()
    {
        _setHealth(Mathf.Max(_health + REGENERATION_RATE, MAX_HEALTH));
    }

    private void _gameOver()
    {
        SceneManager.LoadScene("ScoreMenu");
    }

    public static PlayerData getInstance()
    {
        if (Instance == null)
        {
            return new PlayerData();
        }
        return Instance;
    }

    private Color _getOverlayColorWithNewAlpha(Color color)
    {
        var alpha = Mathf.Max(0, (MAX_HEALTH - _health) / MAX_HEALTH - 0.2f);
        // Will basically set alpha to 0 when full HP and 1 when 0 HP, with a small offset.
        return new Color(color.r, color.g, color.b, alpha);
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