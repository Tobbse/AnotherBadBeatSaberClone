using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FastAudioLoadingStart : MonoBehaviour
{
    public GameObject loader;

    void Start()
    {
        string path = "E:/Daten/coding_projects/Unity Projects/VR_Project/Assets/Resources/Audio\\trancesystem.mp3";

        GlobalStorage.getInstance().Difficulty = Game.DIFFICULTY_EASY;
        GlobalStorage.getInstance().AudioPath = path;

        GlobalStaticSettings.USE_CACHE = false;

        Instantiate(loader);
    }
}
