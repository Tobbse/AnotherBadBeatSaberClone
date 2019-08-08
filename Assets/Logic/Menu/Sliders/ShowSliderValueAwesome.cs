using UnityEngine.UI;
using UnityEngine;

public class ShowSliderValueAwesome : MonoBehaviour
{
    Text _sliderText;

    void Start()
    {
        _sliderText = GetComponent<Text>();
    }

    public void updateText(float value)
    {
        int intVal = Mathf.RoundToInt(value);
        string text;

        switch (intVal)
        {
            case 1:
                text = "AWESOME";
                break;

            case 2:
                text = "REALLY AWESOME";
                break;

            case 3:
                text = "SUPER AWESOME";
                break;

            case 4:
                text = "HYPE LEVEL";
                break;

            case 5:
                text = "ULTA SUPER HYPE LEVEL";
                break;

            case 6:
                text = "OH MY GAWD";
                break;

            case 7:
                text = "AWESOME LEVEL > 9000";
                break;

            case 8:
                text = "CHUCK NORRIS";
                break;

            default:
                text = "not so awesome.....";
                break;
        }

        _sliderText.text = text;
    }
}
