using System.Collections.Generic;
using UnityEngine;
using BeatMappingConfigs;
using SpinnerLightEffects;

/**
 * The main effect controller handling effects like lighting.
 * It holds all the effect controllers for handling those effects and accesses their APIs when an effect should be triggered.
 * 
 * It also contains the event data from the beat mapping, checking if there is a new update available on each update.
 * 
 * The choice of effects if random at the moment, as the meaning of the event values from the bsaber-mappings is unknown
 * and seemingly not documented ANYWHERE in the internet.
 * Therefore random effeects are chosen with a certain probability for each effect, balancing the effect distribution.
 **/
public class MainEffectController
{
    private LaserController _laserController;
    private List<EventConfig> _eventData;
    private EventConfig _cfg;
    private SpinnerLightController _spinnyLightController;
    private FogController _fogController;
    private float _bps;

    public MainEffectController(LaserController laserController, SpinnerLightController spinnyLightController, FogController fogController, List<EventConfig> eventData, float bps)
    {
        _laserController = laserController;
        _spinnyLightController = spinnyLightController;
        _fogController = fogController;
        _eventData = eventData;
        _bps = bps;
    }

    public void checkEventsAvailable(float timePassed)
    {
        while (_eventData.Count > 0)
        {
            _cfg = _eventData[0];
            if (_cfg.Time <= timePassed)
            {
                _handleEvent(_cfg);
                _eventData.RemoveAt(0);
            }
            else break;
        }
    }

    // This function handles an EventConfig by completely ignoring its content and
    // creating a completely random event instead. This is done because I have no way
    // of interpreting this data as there is no information available that tells me
    // what the event data actually means.
    public void _handleEvent(EventConfig cfg)
    {
        if (DevSettings.USE_EFFECTS == false) return;

        if (DevSettings.EFFECT_SPAWN_CHANCE > 0 && Random.Range(0, 100 + 1) > DevSettings.EFFECT_SPAWN_CHANCE)
        {
            return;
        }

        //_effectController.rotateRandom(cfg.value); // This is actually crazy, don't do it.
        float rand = Random.Range(0, 100 + 1);

        // Yes, this is somewhat messy, but it works and since I have no way of interpreting the event values
        // right now, it is fine to do it like this as an intermediate solution.
        if (rand > 98f)
        {
            _spinnyLightController.frontBackTransition(FrontBackTransitionSpinnerLightEffectHandler.TRANSITION_DIRECTION_BACK_TO_FRONT, FrontBackTransitionSpinnerLightEffectHandler.TYPE_VERTICAL, SpinnerLightController.SPINNERS_TYPE_BIG, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 96f)
        {
            _spinnyLightController.frontBackTransition(FrontBackTransitionSpinnerLightEffectHandler.TRANSITION_DIRECTION_FRONT_TO_BACK, FrontBackTransitionSpinnerLightEffectHandler.TYPE_VERTICAL, SpinnerLightController.SPINNERS_TYPE_BIG, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 94f)
        {
            _spinnyLightController.frontBackTransition(FrontBackTransitionSpinnerLightEffectHandler.TRANSITION_DIRECTION_BACK_TO_FRONT, FrontBackTransitionSpinnerLightEffectHandler.TYPE_VERTICAL, SpinnerLightController.SPINNERS_TYPE_SMALL, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 92f)
        {
            _spinnyLightController.frontBackTransition(FrontBackTransitionSpinnerLightEffectHandler.TRANSITION_DIRECTION_FRONT_TO_BACK, FrontBackTransitionSpinnerLightEffectHandler.TYPE_VERTICAL, SpinnerLightController.SPINNERS_TYPE_SMALL, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 90f)
        {
            _spinnyLightController.frontBackTransition(FrontBackTransitionSpinnerLightEffectHandler.TRANSITION_DIRECTION_BACK_TO_FRONT, FrontBackTransitionSpinnerLightEffectHandler.TYPE_HORIZONTAL, SpinnerLightController.SPINNERS_TYPE_BIG, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 88f)
        {
            _spinnyLightController.frontBackTransition(FrontBackTransitionSpinnerLightEffectHandler.TRANSITION_DIRECTION_FRONT_TO_BACK, FrontBackTransitionSpinnerLightEffectHandler.TYPE_HORIZONTAL, SpinnerLightController.SPINNERS_TYPE_BIG, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 86f)
        {
            _spinnyLightController.frontBackTransition(FrontBackTransitionSpinnerLightEffectHandler.TRANSITION_DIRECTION_BACK_TO_FRONT, FrontBackTransitionSpinnerLightEffectHandler.TYPE_HORIZONTAL, SpinnerLightController.SPINNERS_TYPE_SMALL, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 84f)
        {
            _spinnyLightController.frontBackTransition(FrontBackTransitionSpinnerLightEffectHandler.TRANSITION_DIRECTION_FRONT_TO_BACK, FrontBackTransitionSpinnerLightEffectHandler.TYPE_HORIZONTAL, SpinnerLightController.SPINNERS_TYPE_SMALL, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 82f)
        {
            _spinnyLightController.blinkAll(SpinnerLightController.SPINNERS_TYPE_BIG, BlinkAllSpinnerLightEffectHandler.TYPE_HORIZONTAL, Random.Range(30, 40));
        } else if (rand > 74f)
        {
            _spinnyLightController.blinkAll(SpinnerLightController.SPINNERS_TYPE_SMALL, BlinkAllSpinnerLightEffectHandler.TYPE_VERTICAL, Random.Range(30, 40));
        } else if (rand > 56f)
        {
            _spinnyLightController.blinkAll(SpinnerLightController.SPINNERS_TYPE_BIG, BlinkAllSpinnerLightEffectHandler.TYPE_VERTICAL, Random.Range(30, 40));
        } else if (rand > 48f)
        {
            _fogController.redBlink(20);
        } else if (rand > 46f)
        {
            _fogController.yellowBlink(20);
        } else if (rand > 44f)
        {
            _fogController.greenBlink(20);
        } else if (rand > 42f)
        {
            _fogController.blackBlink(20);
        } else if (rand > 40f)
        {
            _fogController.blueBlink(20);
        } else if (rand > 10) {
            _laserController.setRandomRotation(cfg.Value);
        } else
        {
            _laserController.setRandomAngularSpeed(0.1f);
        }
    }
}
