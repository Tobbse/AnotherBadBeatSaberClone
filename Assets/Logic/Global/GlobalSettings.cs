using UnityEngine;

/**
 * Contains global settings that can be set to manipulate some game states.
 * This came in very handy during development.
 **/
public static class GlobalSettings
{
    public static bool USE_CACHE = true; // Cache should probably be disabled during development so that changes actually have an effect.
    public static bool USE_OBSTACLES = true;
    public static bool OVERRIDE_BLOCK_DESPAWN = false;
    public static bool USE_SABRE_DEBUG_RAYS = false; // Enable only for debugging purposes.
    public static bool TAKE_DAMAGE = true;
    public static bool USE_EFFECTS = true;
    public static float ONSET_SENSITIVITY_MULT = 1.3f;
}
