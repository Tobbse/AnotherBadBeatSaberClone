using System.Collections.Generic;

namespace GameEffects
{
    /**
     * This namescace exposes an API to the MainEffectController to control fog lighting effects.
     * It is more complex then the other effect controllers.
     **/
    namespace SpinnerLightEffects
    {
        /**
         * Base class for spinner light effects.
         **/
        public class BaseSpinnerLightEffectHandler
        {
            protected bool _isDone;

            public bool isDone()
            {
                return _isDone;
            }

            public virtual void update()
            {
                // Override in subclasses.
            }
        }


        /**
         * This effect will light up all spinners of a certain type consecutively with a delay, so that a
         * transition from the front to the back (or the other direction) is created.
        **/
        public class FrontBackTransitionSpinnerLightEffectHandler : BaseSpinnerLightEffectHandler
        {
            public const string TRANSITION_DIRECTION_FRONT_TO_BACK = "TRANSITION_DIRECTION_FRONT_TO_BACK";
            public const string TRANSITION_DIRECTION_BACK_TO_FRONT = "TRANSITION_DIRECTION_BACK_TO_FRONT";
            public const string TYPE_VERTICAL = "TYPE_VERTICAL";
            public const string TYPE_HORIZONTAL = "TYPE_HORIZONTAL";

            public const int LIGHT_TRANSITION_TIME = 5;

            private List<SpinnerLight> _spinners;
            private string _type;
            private int _startTimer;
            private int _flashDuration;
            private int _speed = LIGHT_TRANSITION_TIME;
            private int _currentTransitionCounter;
            private int _spinnerIndex;
            private string _direction;

            public FrontBackTransitionSpinnerLightEffectHandler(List<SpinnerLight> spinners, string horizontalVertical, string direction,
                int flashDuration, int startDelay, int currentTransitionCounter)
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


        /**
         * This effect will make all spinners of a certain type (veritcal, horizontal...)
         * light up for a certain amount of frames.
         **/
        public class BlinkAllSpinnerLightEffectHandler : BaseSpinnerLightEffectHandler
        {
            public const string TYPE_VERTICAL = "TYPE_VERTICAL";
            public const string TYPE_HORIZONTAL = "TYPE_HORIZONTAL";

            private List<SpinnerLight> _spinners;
            private string _type;
            private int _startValue;
            private int _spinnerIndex;
            private string type;

            public BlinkAllSpinnerLightEffectHandler(List<SpinnerLight> spinners, string type, int duration)
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
                foreach (SpinnerLight spinner in _spinners)
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

}
