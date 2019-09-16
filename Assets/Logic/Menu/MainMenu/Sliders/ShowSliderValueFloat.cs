using UnityEngine.UI;
using UnityEngine;

namespace MenuMainMenu
{
    /// <summary>
    /// Shows float values on slider.
    /// </summary>
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

}
