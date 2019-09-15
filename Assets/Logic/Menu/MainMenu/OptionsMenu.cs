using UnityEngine;
using System;

namespace MenuMainMenu
{
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

}
