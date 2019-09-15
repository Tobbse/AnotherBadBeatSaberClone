using UnityEngine;

/**
 * Contains global settings that can be set to manipulate some game states.
 * This came in very handy during development.
 **/
public static class DevSettings
{
    public static bool USE_CACHE              = true;  // DEFAULT  TRUE
    public static bool USE_OBSTACLES          = true;  // DEFAULT  TRUE
    public static bool OVERRIDE_BLOCK_DESPAWN = false; // DEFAULT FALSE
    public static bool USE_SABRE_DEBUG_RAYS   = false; // DEFAULT FALSE
    public static bool TAKE_DAMAGE            = true;  // DEFAULT  TRUE
    public static bool USE_EFFECTS            = true;  // DEFAULT  TRUE
}
