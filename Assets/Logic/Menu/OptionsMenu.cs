using UnityEngine;
using System;

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
