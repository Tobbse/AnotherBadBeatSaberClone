using UnityEngine.UI;
using UnityEngine;

/**
 * Adds slider values to the difficulty slider.
 **/
public class ShowSliderValueDifficulty : MonoBehaviour
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
                text = Game.DIFFICULTY_EASY;
                break;

            case 2:
                text = Game.DIFFICULTY_NORMAL;
                break;

            case 3:
                text = Game.DIFFICULTY_HARD;
                break;

            case 4:
                text = Game.DIFFICULTY_EXPERT;
                break;

            case 5:
                text = Game.DIFFICULTY_EXPERT_PLUS;
                break;

            default:
                text = Game.DIFFICULTY_EASY;
                break;
        }

        _sliderText.text = text;
    }
}