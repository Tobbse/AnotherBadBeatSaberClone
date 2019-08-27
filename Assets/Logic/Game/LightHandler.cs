using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeatMappingConfigs;

public class LightHandler
{
    private EffectController _effectController;
    private List<EventConfig> _eventData;
    private EventConfig _cfg;
    private float _bps;

    public LightHandler(EffectController effectController, List<EventConfig> eventData, float bps)
    {
        _effectController = effectController;
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
        _effectController.setRandomRotation(cfg.value);
    }
}
