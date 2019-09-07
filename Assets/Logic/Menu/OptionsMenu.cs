using UnityEngine;
using System;

/**
 * Options Menu UI script to handle button clicks.
 **/
public class OptionsMenu : MonoBehaviour
{
    private Action _backButtonCallback;

    public void setBackCallback(Action backButtonCallback)
    {
        _backButtonCallback = backButtonCallback;
    }

    public void clickBack()
    {
        _backButtonCallback();
        gameObject.SetActive(false);
    }
}
