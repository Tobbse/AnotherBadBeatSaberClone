using UnityEngine.UI;
using UnityEngine;
using Game;

namespace MenuMainMenu
{
    /**
     * Adds slider values to the difficulty slider.
     **/
    public class ShowSliderValueDifficulty : MonoBehaviour
    {
        Text _sliderText;

        void Start()
        {
            _sliderText = GetComponent<Text>();
            _sliderText.text = GameController.DIFFICULTY_EASY;
        }

        public void updateText(float value)
        {
            int intVal = Mathf.RoundToInt(value);
            string text;

            switch (intVal)
            {
                case 1:
                    text = GameController.DIFFICULTY_EASY;
                    break;

                case 2:
                    text = GameController.DIFFICULTY_NORMAL;
                    break;

                case 3:
                    text = GameController.DIFFICULTY_HARD;
                    break;

                case 4:
                    text = GameController.DIFFICULTY_EXPERT;
                    break;

                case 5:
                    text = GameController.DIFFICULTY_EXPERT_PLUS;
                    break;

                default:
                    text = GameController.DIFFICULTY_EASY;
                    break;
            }

            _sliderText.text = text;
        }
    }

}