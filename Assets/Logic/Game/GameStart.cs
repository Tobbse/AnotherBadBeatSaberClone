using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStart : MonoBehaviour
{
    void Start()
    {
        System.Random random = new System.Random();

        /*for (int i = 0; i < 100; i++)
        {
            GameObject cube = Instantiate(Resources.Load("Touchable Cube"), transform) as GameObject;
            cube.transform.position = new Vector3(random.Next(1, 10), random.Next(1, 10), random.Next(1, 10));
        }*/

        AudioClip audioClip = GlobalStorage.Instance.AudioClip;
        AudioSource audioSource = new AudioSource();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void Update()
    {
        
    }
}
