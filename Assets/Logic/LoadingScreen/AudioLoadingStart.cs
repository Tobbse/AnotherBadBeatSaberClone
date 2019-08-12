using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioLoadingStart : MonoBehaviour
{
    public GameObject loader;

    void Start()
    {
        if (SceneManager.GetActiveScene().isLoaded == false)
        {
            SceneManager.sceneLoaded += _onSceneLoaded;
        }
        else
        {
            _init();
        }
    }

    private void _onSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        _init();
    }

    private void _init()
    {
        StartCoroutine(waitForSecondsThenLoad(0.3f));
    }

    private IEnumerator waitForSecondsThenLoad(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Instantiate(loader);
    }
}
