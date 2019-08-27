using UnityEngine;
using TMPro;

public class ScoreTracker : ScriptableObject
{
    private static ScoreTracker Instance;

    private const int MAX_COMBO = 8;
    private const int POINTS_PER_HIT = 10;

    private TextMeshPro _comboText;
    private TextMeshPro _streakText;
    private AudioSource _hitSound;
    private AudioSource _missSound;
    private int _numBeats;
    private int _score = 0;
    private int _combo = 1;
    private int _hits = 0;
    private int _misses = 0;
    private int _streak = 0;
    private int _highestStreak = 0;
    private int _addedCombos = 0;

    public int NumBeats { get => _numBeats; set => _numBeats = value; }
    public int Hits{ get => _hits; set => _hits = value; }
    public int Misses{ get => _misses; set => _misses = value; }
    public int Score{ get => _score; set => _score = value; }
    public int HighestStreak { get => _highestStreak; set => _highestStreak = value; }
    public int AverageCombo { get => _addedCombos / _numBeats; set => _addedCombos = value; }

    public void resetComboCounter()
    {
        _combo = 1;
        _streak = 0;
    }

    public void hit()
    {
        if (_hitSound != null) _hitSound.PlayOneShot(_hitSound.clip);

        _addedCombos += _combo;
        _score += _combo * POINTS_PER_HIT;
        _combo = Mathf.Min(MAX_COMBO, _combo + 1);
        _hits += 1;
        _streak += 1;
        _highestStreak = Mathf.Max(_highestStreak, _streak);

        _setTexts();
    }

    public void miss()
    {
        if (_missSound != null) _missSound.PlayOneShot(_missSound.clip);

        // For Debugging
        GameObject obj = GameObject.Find("AngleText");
        if (obj != null) obj.GetComponent<TextMeshPro>().SetText("0");
        obj = GameObject.Find("HitText");
        if (obj != null)
        {
            obj.GetComponent<TextMeshPro>().SetText("MISS");
            obj.GetComponent<TextMeshPro>().color = Color.red;
        }

        resetComboCounter();
        _misses += 1;
        _streak = 0;

        _setTexts();
    }

    public void setupGameObjects()
    {
        _comboText = GameObject.Find("ComboText").GetComponent<TextMeshPro>();
        _streakText = GameObject.Find("StreakText").GetComponent<TextMeshPro>();
        _hitSound = GameObject.Find("HitSound").GetComponent<AudioSource>();
        _missSound = GameObject.Find("MissSound").GetComponent<AudioSource>();

        _hitSound.volume = 0.5f;
        _missSound.volume = 0.2f;

        if (_streakText == null || _comboText == null || _hitSound == null || _missSound == null)
        {
            Debug.LogError("Failed setting GameObjects in ScoreTracker!");
        }
    }

    private void _setTexts()
    {
        if (_comboText != null) _comboText.text = "x" + _combo.ToString();
        if (_streakText != null) _streakText.text = _streak.ToString();
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
