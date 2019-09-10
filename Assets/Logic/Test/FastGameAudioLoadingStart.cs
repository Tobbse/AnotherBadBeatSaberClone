using UnityEngine;

/**
 * Helper script used in the 'FastGame' scene. Used for development purposes only.
 * Some settings are set to make the developer's life easier and the audio loading is
 * triggered immediately, without going to the menu screen.
 **/
public class FastGameAudioLoadingStart : MonoBehaviour
{
    public GameObject loader;

    public void Start()
    {
        string path = "Assets/Resources/Audio/beats.mp3";
        //string path = "Assets/Resources/Audio/trancesystem.mp3";
        //string path = "Assets/Resources/Audio/02 - A Subtle Dagger.mp3";

        GlobalStorage.getInstance().Difficulty = Game.DIFFICULTY_EASY;
        GlobalStorage.getInstance().AudioPath = path;

        //GlobalSettings.TAKE_DAMAGE = false;
        DevSettings.USE_CACHE = true;
        DevSettings.TAKE_DAMAGE = false;
        //GlobalSettings.USE_EFFECTS = false;

        Instantiate(loader);
    }
}
