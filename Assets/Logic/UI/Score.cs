using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("HITS").GetComponent<Text>().text = "HITS: " + PScoreTracker.Instance.Hits.ToString();
        GameObject.Find("SCORE").GetComponent<Text>().text = "SCORE: " + PScoreTracker.Instance.Score.ToString();
        GameObject.Find("MISSES").GetComponent<Text>().text = "MISSES: " + PScoreTracker.Instance.Misses.ToString();
        GameObject.Find("TOTAL").GetComponent<Text>().text = "TOTAL: " + PScoreTracker.Instance.TotalBeats.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
