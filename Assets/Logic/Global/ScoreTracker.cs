﻿using UnityEngine;
using TMPro;

/**
 * Singleton that keeps track of the player's score as well as multiplier and combo values.
 * Handles hits and misses.
 **/
public class ScoreTracker : ScriptableObject
{
    private static ScoreTracker Instance;

    private const int MAX_COMBO = 8;
    private const int POINTS_PER_HIT = 10;
    private const float DAMAGE_PER_MISS = 3f;

    private TextMeshPro _multText;
    private TextMeshPro _comboText;
    private AudioSource _hitSound;
    private AudioSource _missSound;
    private PlayerData _playerData;
    private int _numBeats;
    private int _score = 0;
    private int _multiplier = 1;
    private int _hits = 0;
    private int _misses = 0;
    private int _combo = 0;
    private int _highestStreak = 0;
    private int _averageCombo = 0;

    public int NumBeats { get => _numBeats; set => _numBeats = value; }
    public int Hits{ get => _hits; set => _hits = value; }
    public int Misses{ get => _misses; set => _misses = value; }
    public int Score{ get => _score; set => _score = value; }
    public int HighestStreak { get => _highestStreak; set => _highestStreak = value; }
    public int AverageCombo { get => _averageCombo / _numBeats; set => _averageCombo = value; }

    public void reset()
    {
        resetComboCounter();
        _hits = 0;
        _misses = 0;
        _score = 0;
        _highestStreak = 0;
        _averageCombo = 0;
    }

    public void resetComboCounter()
    {
        _multiplier = 1;
        _combo = 0;
    }

    public void hit()
    {
        if (_hitSound != null) _hitSound.PlayOneShot(_hitSound.clip);

        _averageCombo += _multiplier;
        _score += _multiplier * POINTS_PER_HIT;
        _multiplier = Mathf.Min(MAX_COMBO, _multiplier + 1);
        _hits += 1;
        _combo += 1;
        _highestStreak = Mathf.Max(_highestStreak, _combo);

        _setTexts();
    }

    public void miss()
    {
        if (_missSound != null) _missSound.PlayOneShot(_missSound.clip);

        // For Debugging
        /*GameObject obj = GameObject.Find("AngleText");
        if (obj != null) obj.GetComponent<TextMeshPro>().SetText("0");
        obj = GameObject.Find("HitText");
        if (obj != null)
        {
            obj.GetComponent<TextMeshPro>().SetText("MISS");
            obj.GetComponent<TextMeshPro>().color = Color.red;
        }*/

        resetComboCounter();
        _playerData.takeDamage(DAMAGE_PER_MISS);
        _misses += 1;
        _combo = 0;

        _setTexts();
    }

    public void setupGameObjects()
    {
        _multText = GameObject.Find("MultiplierText").GetComponent<TextMeshPro>();
        _comboText = GameObject.Find("ComboText").GetComponent<TextMeshPro>();
        _hitSound = GameObject.Find("HitSound").GetComponent<AudioSource>();
        _missSound = GameObject.Find("MissSound").GetComponent<AudioSource>();
        _playerData = PlayerData.getInstance();

        _hitSound.volume = 0.4f;
        _missSound.volume = 0.4f;

        if (_comboText == null || _multText == null || _hitSound == null || _missSound == null)
        {
            Debug.LogError("Failed setting GameObjects in ScoreTracker!");
        }
    }

    private void _setTexts()
    {
        if (_multText != null) _multText.text = "x" + _multiplier.ToString();
        if (_comboText != null) _comboText.text = _combo.ToString();
    }

    public static ScoreTracker getInstance()
    {
        if (Instance == null)
        {
            return new ScoreTracker();
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
