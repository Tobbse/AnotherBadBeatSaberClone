using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SpinnyLight
{
    public class SpinnyLightController : MonoBehaviour
    {
        public const string SPINNERS_TYPE_BIG = "SPINNERS_TYPE_BIG";
        public const string SPINNERS_TYPE_SMALL = "SPINNERS_TYPE_SMALL";

        private List<SpinnyThing> _smallSpinnyThings = new List<SpinnyThing>();
        private List<SpinnyThing> _bigSpinnyThings = new List<SpinnyThing>();
        private List<BaseSpinnyThingLightHandler> _activeLightHandlers = new List<BaseSpinnyThingLightHandler>();

        public void Start()
        {
            List<SpinnyThing> spinners = new List<SpinnyThing>(gameObject.GetComponentsInChildren<SpinnyThing>());

            foreach (SpinnyThing spinner in spinners)
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

        // TODO add speed here?
        // TODO don't pass stupid params
        public void frontBackTransition(string transitionDirection, string horizontalVertical, string spinnersSize, int flashDuration = 10, int startDelay = 0, int transitionTime = SpinnyThingFrontToBackHandler.LIGHT_TRANSITION_TIME)
        {
            List<SpinnyThing> spinners = spinnersSize == SPINNERS_TYPE_BIG ? _bigSpinnyThings : _smallSpinnyThings;
            _activeLightHandlers.Add(new SpinnyThingFrontToBackHandler(spinners, horizontalVertical, transitionDirection, flashDuration, startDelay, transitionTime));
        }

        public void blinkAll(string spinnerSize, string horizontalVertical, int duration)
        {
            List<SpinnyThing> spinners = spinnerSize == SPINNERS_TYPE_BIG ? _bigSpinnyThings : _smallSpinnyThings;
            _activeLightHandlers.Add(new SpinnyThingBlinkAllHandler(spinners, horizontalVertical, duration));
        }

        public void Update()
        {
            BaseSpinnyThingLightHandler lightHandler;
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


    public class BaseSpinnyThingLightHandler
    {
        protected bool _isDone;

        public bool isDone()
        {
            return _isDone;
        }

        public virtual void update()
        {
        }
    }


    public class SpinnyThingFrontToBackHandler : BaseSpinnyThingLightHandler
    {
        public const string TRANSITION_DIRECTION_FRONT_TO_BACK = "TRANSITION_DIRECTION_FRONT_TO_BACK";
        public const string TRANSITION_DIRECTION_BACK_TO_FRONT = "TRANSITION_DIRECTION_BACK_TO_FRONT";
        public const string TYPE_VERTICAL = "TYPE_VERTICAL";
        public const string TYPE_HORIZONTAL = "TYPE_HORIZONTAL";

        public const int LIGHT_TRANSITION_TIME = 5;

        private List<SpinnyThing> _spinners;
        private string _type;
        private int _startTimer;
        private int _flashDuration;
        private int _speed = LIGHT_TRANSITION_TIME;
        private int _currentTransitionCounter;
        private int _spinnerIndex;
        private string _direction;

        public SpinnyThingFrontToBackHandler(List<SpinnyThing> spinners, string horizontalVertical, string direction, int flashDuration, int startDelay, int currentTransitionCounter)
        {
            _spinners = spinners;
            _type = horizontalVertical;
            _startTimer = startDelay;
            _currentTransitionCounter = currentTransitionCounter;
            _speed = _currentTransitionCounter;
            _direction = direction;

            if (direction == TRANSITION_DIRECTION_BACK_TO_FRONT) _spinnerIndex = _spinners.Count - 1;

            // TODO maybe add more light types
            switch (_type)
            {
                case TYPE_VERTICAL:
                case TYPE_HORIZONTAL:
                    _flashDuration = flashDuration; // Light up for x frames each time.
                    _currentTransitionCounter = LIGHT_TRANSITION_TIME;
                    break;
            }
        }

        public override void update()
        {
            if (_spinnerIndex >= _spinners.Count || _spinnerIndex < 0) return;
            if (_startTimer > 0)
            {
                _startTimer--;
                return;
            }

            if (_currentTransitionCounter == _speed)
            {
                _currentTransitionCounter = 0;
                switch (_type)
                {
                    case TYPE_VERTICAL:
                        _spinners[_spinnerIndex].VerticalLightFrames = _flashDuration;
                        break;

                    case TYPE_HORIZONTAL:
                        _spinners[_spinnerIndex].HorizontalLightFrames = _flashDuration;
                        break;
                }
                _spinnerIndex = (_direction == TRANSITION_DIRECTION_BACK_TO_FRONT) ? _spinnerIndex - 1 : _spinnerIndex + 1;
                if (_spinnerIndex == _spinners.Count || _spinnerIndex < 0) _isDone = true;
            }
            _currentTransitionCounter++;
        }       
    }


    public class SpinnyThingBlinkAllHandler : BaseSpinnyThingLightHandler
    {
        public const string TYPE_VERTICAL = "TYPE_VERTICAL";
        public const string TYPE_HORIZONTAL = "TYPE_HORIZONTAL";

        private List<SpinnyThing> _spinners;
        private string _type;
        private int _startValue;
        private int _spinnerIndex;
        private string type;

        public SpinnyThingBlinkAllHandler(List<SpinnyThing> spinners, string type, int duration)
        {
            _type = type;
            _spinners = spinners;
            _startValue = duration;

            _showLights();
        }

        public override void update()
        {
            // Doesn't need to update anything.
        }

        private void _showLights()
        {
            foreach (SpinnyThing spinner in _spinners)
            {
                switch (_type)
                {
                    case TYPE_VERTICAL:
                        spinner.VerticalLightFrames = _startValue;
                        break;

                    case TYPE_HORIZONTAL:
                        spinner.HorizontalLightFrames = _startValue;
                        break;
                }
            }
            _isDone = true;
        }
    }

}