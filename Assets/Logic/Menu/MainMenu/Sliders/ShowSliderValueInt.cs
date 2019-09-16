using UnityEngine.UI;
using UnityEngine;

namespace MenuMainMenu
{
    /// <summary>
    /// Shows int values on slider.
    /// </summary>
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

}
