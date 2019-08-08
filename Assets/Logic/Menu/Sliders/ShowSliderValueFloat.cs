using UnityEngine.UI;
using UnityEngine;

public class ShowSliderValueFloat : MonoBehaviour
{
    Text _sliderText;

    void Start()
    {
        _sliderText = GetComponent<Text>();
    }

    public void updateText(float value)
    {
        _sliderText.text = value.ToString();
    }
}
