namespace Global
{
    /**
     * Contains global settings that can be set to manipulate some game states.
     * This came in very handy during development.
     **/
    public static class DevSettings
    {
        public static bool USE_CACHE = true;  // DEFAULT  TRUE    Toggles cache usage
        public static bool USE_OBSTACLES = true;  // DEFAULT  TRUE    Toggles obstacles
        public static bool OVERRIDE_BLOCK_DESPAWN = false; // DEFAULT FALSE    Prevents blocks from despawning
        public static bool USE_SABRE_DEBUG_RAYS = false; // DEFAULT FALSE    Toggle debug rays on sabres
        public static bool TAKE_DAMAGE = true;  // DEFAULT  TRUE    Toggle damage
        public static bool USE_EFFECTS = true;  // DEFAULT  TRUE    Toggle effects
        public static float EFFECT_SPAWN_CHANCE = -1;    // DEFAULT    -1    Spawn Chance for effects in %
    }

}
