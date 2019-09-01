using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeatMappingConfigs;
using SpinnyLight;

public class MainLightController
{
    private LaserController _laserController;
    private List<EventConfig> _eventData;
    private EventConfig _cfg;
    private SpinnyLightController _spinnyLightController;
    private FogController _fogController;
    private float _bps;

    public MainLightController(LaserController laserController, SpinnyLightController spinnyLightController, FogController fogController, List<EventConfig> eventData, float bps)
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
            if (_cfg.time <= timePassed)
            {
                _handleEvent(_cfg);
                _eventData.RemoveAt(0);
            }
            else break;
        }
    }

    public void _handleEvent(EventConfig cfg)
    {
        //_effectController.rotateRandom(cfg.value); // This is actually crazy.
        float rand = Random.Range(0, 100 + 1);

        if (rand > 98f)
        {
            _spinnyLightController.frontBackTransition(SpinnyThingFrontToBackHandler.TRANSITION_DIRECTION_BACK_TO_FRONT, SpinnyThingFrontToBackHandler.TYPE_VERTICAL, SpinnyLightController.SPINNERS_TYPE_BIG, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 96f)
        {
            _spinnyLightController.frontBackTransition(SpinnyThingFrontToBackHandler.TRANSITION_DIRECTION_FRONT_TO_BACK, SpinnyThingFrontToBackHandler.TYPE_VERTICAL, SpinnyLightController.SPINNERS_TYPE_BIG, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 94f)
        {
            _spinnyLightController.frontBackTransition(SpinnyThingFrontToBackHandler.TRANSITION_DIRECTION_BACK_TO_FRONT, SpinnyThingFrontToBackHandler.TYPE_VERTICAL, SpinnyLightController.SPINNERS_TYPE_SMALL, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 92f)
        {
            _spinnyLightController.frontBackTransition(SpinnyThingFrontToBackHandler.TRANSITION_DIRECTION_FRONT_TO_BACK, SpinnyThingFrontToBackHandler.TYPE_VERTICAL, SpinnyLightController.SPINNERS_TYPE_SMALL, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 90f)
        {
            _spinnyLightController.frontBackTransition(SpinnyThingFrontToBackHandler.TRANSITION_DIRECTION_BACK_TO_FRONT, SpinnyThingFrontToBackHandler.TYPE_HORIZONTAL, SpinnyLightController.SPINNERS_TYPE_BIG, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 88f)
        {
            _spinnyLightController.frontBackTransition(SpinnyThingFrontToBackHandler.TRANSITION_DIRECTION_FRONT_TO_BACK, SpinnyThingFrontToBackHandler.TYPE_HORIZONTAL, SpinnyLightController.SPINNERS_TYPE_BIG, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 86f)
        {
            _spinnyLightController.frontBackTransition(SpinnyThingFrontToBackHandler.TRANSITION_DIRECTION_BACK_TO_FRONT, SpinnyThingFrontToBackHandler.TYPE_HORIZONTAL, SpinnyLightController.SPINNERS_TYPE_SMALL, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 84f)
        {
            _spinnyLightController.frontBackTransition(SpinnyThingFrontToBackHandler.TRANSITION_DIRECTION_FRONT_TO_BACK, SpinnyThingFrontToBackHandler.TYPE_HORIZONTAL, SpinnyLightController.SPINNERS_TYPE_SMALL, Random.Range(5, 20), 0, Random.Range(3, 8));
        } else if (rand > 82f)
        {
            _spinnyLightController.blinkAll(SpinnyLightController.SPINNERS_TYPE_BIG, SpinnyThingBlinkAllHandler.TYPE_HORIZONTAL, Random.Range(30, 40));
        } else if (rand > 74f)
        {
            _spinnyLightController.blinkAll(SpinnyLightController.SPINNERS_TYPE_SMALL, SpinnyThingBlinkAllHandler.TYPE_VERTICAL, Random.Range(30, 40));
        } else if (rand > 56f)
        {
            _spinnyLightController.blinkAll(SpinnyLightController.SPINNERS_TYPE_BIG, SpinnyThingBlinkAllHandler.TYPE_VERTICAL, Random.Range(30, 40));
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
            _laserController.setRandomRotation(cfg.value);
        } else
        {
            _laserController.setRandomAngularSpeed(0.1f);
        }
    }
}
