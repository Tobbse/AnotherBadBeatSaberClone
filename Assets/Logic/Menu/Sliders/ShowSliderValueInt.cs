using UnityEngine.UI;
using UnityEngine;

/**
 * Shows int values on slider.
 **/
public class ShowSliderValueInt : MonoBehaviour
{
    Text _sliderText;

    void Start()
    {
        _sliderText = GetComponent<Text>();
    }

    public void updateText(float value)
    {
        _sliderText.text = Mathf.RoundToInt(value).ToString();
    }
}
