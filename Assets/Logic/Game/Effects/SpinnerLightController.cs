using UnityEngine;
using System.Collections.Generic;
using GameEffects.SpinnerLightEffects;

namespace GameEffects
{
    /**
    * Effect controller for the Spinner Lights exposing an API to the MainEffectController to control spinning light effects.
    * 
    * This controller is more complicated then the other effect controllers. It contains the big and small spinner game objects
    * and adds various blinking and transition effects to them.
    * 
    * It holds all running effects and disposes them when they have been executed.
    **/
    public class SpinnerLightController : MonoBehaviour
    {
        public const string SPINNERS_TYPE_BIG = "SPINNERS_TYPE_BIG";
        public const string SPINNERS_TYPE_SMALL = "SPINNERS_TYPE_SMALL";

        private List<SpinnerLight> _smallSpinnyThings = new List<SpinnerLight>();
        private List<SpinnerLight> _bigSpinnyThings = new List<SpinnerLight>();
        private List<BaseSpinnerLightEffectHandler> _activeLightHandlers = new List<BaseSpinnerLightEffectHandler>();

        public void Start()
        {
            List<SpinnerLight> spinners = new List<SpinnerLight>(gameObject.GetComponentsInChildren<SpinnerLight>());

            foreach (SpinnerLight spinner in spinners)
            {
                if (spinner.transform.parent.name.Contains("Big"))
                {
                    _bigSpinnyThings.Add(spinner);
                }
                else if (spinner.transform.parent.name.Contains("Small"))
                {
                    _smallSpinnyThings.Add(spinner);
                }
            }
        }

        public void frontBackTransition(string transitionDirection, string horizontalVertical, string spinnersSize, int flashDuration = 10, int startDelay = 0, int transitionTime = FrontBackTransitionSpinnerLightEffectHandler.LIGHT_TRANSITION_TIME)
        {
            List<SpinnerLight> spinners = spinnersSize == SPINNERS_TYPE_BIG ? _bigSpinnyThings : _smallSpinnyThings;
            _activeLightHandlers.Add(new FrontBackTransitionSpinnerLightEffectHandler(spinners, horizontalVertical, transitionDirection, flashDuration, startDelay, transitionTime));
        }

        public void blinkAll(string spinnerSize, string horizontalVertical, int duration)
        {
            List<SpinnerLight> spinners = spinnerSize == SPINNERS_TYPE_BIG ? _bigSpinnyThings : _smallSpinnyThings;
            _activeLightHandlers.Add(new BlinkAllSpinnerLightEffectHandler(spinners, horizontalVertical, duration));
        }

        public void Update()
        {
            BaseSpinnerLightEffectHandler lightHandler;
            for (int i = _activeLightHandlers.Count - 1; i >= 0; i--)
            {
                lightHandler = _activeLightHandlers[i];
                if (lightHandler.isDone())
                {
                    lightHandler = null;
                    _activeLightHandlers.RemoveAt(i);
                }
                else lightHandler.update();
            }
        }
    }

}
