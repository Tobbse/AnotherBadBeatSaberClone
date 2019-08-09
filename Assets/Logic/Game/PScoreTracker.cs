using UnityEngine;
using TMPro;

public class PScoreTracker : ScriptableObject
{
    public static PScoreTracker Instance;

    private const int MAX_COMBO = 8;
    private const int POINTS_PER_HIT = 10;

    private int _totalBeats;
    private int _score;
    private int _combo = 1;
    private int _hits;
    private int _misses;

    public int TotalBeats { get => _totalBeats; set => _totalBeats = value; }
    public int Hits{ get => _hits; set => _hits = value; }
    public int Misses{ get => _misses; set => _misses = value; }
    public int Score{ get => _score; set => _score = value; }

    public PScoreTracker(int totalBeats)
    {
        _totalBeats = totalBeats;
    }

    public void resetComboCounter()
    {
        _combo = 1;
    }

    public void hit()
    {
        GameObject.Find("HitSound").GetComponent<AudioSource>().Play();
        _score += _combo * POINTS_PER_HIT;
        _combo = Mathf.Min(MAX_COMBO, _combo);
        _hits += 1;
    }

    public void miss()
    {
        GameObject.Find("MissSound").GetComponent<AudioSource>().Play();

        GameObject.Find("AngleText").GetComponent<TextMeshPro>().SetText("0");
        GameObject.Find("HitText").GetComponent<TextMeshPro>().SetText("MISS");
        GameObject.Find("HitText").GetComponent<TextMeshPro>().color = Color.red;

        //audioSource.Play(); // TODO better use PlayOneShot here?!?
        //Debug.Log("Missed block!");
        resetComboCounter();
        _misses += 1;
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
