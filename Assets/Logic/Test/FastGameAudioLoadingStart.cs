using UnityEngine;
using Global;
using Game;

namespace Test
{
    /// <summary>
    /// Helper script used in the 'FastGame' scene.Used for development purposes only.
    /// Some settings are set to make the developer's life easier and the audio loading is
    /// triggered immediately, without going to the menu screen.
    /// </summary>
    public class FastGameAudioLoadingStart : MonoBehaviour
    {
        public GameObject loader;

        public void Start()
        {
            string path = "Assets/Resources/Audio/beats.mp3";
            //string path = "Assets/Resources/Audio/trancesystem.mp3";
            //string path = "Assets/Resources/Audio/02 - A Subtle Dagger.mp3";

            GlobalStorage.getInstance().Difficulty = GameController.DIFFICULTY_EXPERT_PLUS;
            GlobalStorage.getInstance().AudioPath = path;

            DevSettings.TAKE_DAMAGE = false;
            DevSettings.USE_CACHE = true;
            DevSettings.EFFECT_SPAWN_CHANCE = 10f; // Only 10 % spawn chance for effects. This is necessary for some songs, because they are simply too crazy otherwise.

            Instantiate(loader);
        }
    }

}
