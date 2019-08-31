using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FastAudioLoadingStart : MonoBehaviour
{
    public GameObject loader;

    void Start()
    {
        //string path = "Assets/Resources/Audio/beats.mp3";
        //string path = "Assets/Resources/Audio/trancesystem.mp3";
        string path = "Assets/Resources/Audio/02 - A Subtle Dagger.mp3";

        GlobalStorage.getInstance().Difficulty = Game.DIFFICULTY_EASY;
        GlobalStorage.getInstance().AudioPath = path;

        GlobalStaticSettings.TAKE_DAMAGE = false;
        GlobalStaticSettings.USE_CACHE = false;

        Instantiate(loader);
    }
}
