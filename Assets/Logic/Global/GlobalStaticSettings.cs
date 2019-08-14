using UnityEngine;

public static class GlobalStaticSettings
{
    public static bool USE_CACHE = true; // Cache should probably be disabled during development so that changes actually have an effect.
    public static bool USE_OBSTACLES = false;
    public static bool OVERRIDE_BLOCK_DESPAWN = false;
    public static bool USE_SABRE_DEBUG_RAYS = false;
    public static float ONSET_SENSITIVITY_MULT = 1.0f;
}
