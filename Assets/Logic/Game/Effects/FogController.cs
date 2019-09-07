using UnityEngine;

/**
 * This class exposes an API to the MainEffectController to control fog lighting effects.
 **/
public class FogController : MonoBehaviour
{
    public MeshRenderer baseFog;
    public VolumetricFogEffect redFog;
    public VolumetricFogEffect yellowFog;
    public VolumetricFogEffect greenFog;
    public VolumetricFogEffect blackFog;
    public VolumetricFogEffect blueFog;

    private int _disableBaseFogForFrames;

    public void redBlink(int frames)
    {
        redFog.ActiveFrames = frames;
        _disableBaseFogForFrames = frames;
        baseFog.enabled = false;
    }

    public void yellowBlink(int frames)
    {
        yellowFog.ActiveFrames = frames;
        _disableBaseFogForFrames = frames;
        baseFog.enabled = false;
    }

    public void greenBlink(int frames)
    {
        greenFog.ActiveFrames = frames;
        _disableBaseFogForFrames = frames;
        baseFog.enabled = false;
    }

    public void blackBlink(int frames)
    {
        blackFog.ActiveFrames = frames;
        _disableBaseFogForFrames = frames;
        baseFog.enabled = false;
    }

    public void blueBlink(int frames)
    {
        blueFog.ActiveFrames = frames;
        _disableBaseFogForFrames = frames;
        baseFog.enabled = false;
    }

    void Update()
    {
        if (_disableBaseFogForFrames > 0)
        {
            _disableBaseFogForFrames--;
        } else
        {
            baseFog.enabled = true;
        }
    }
}
