using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BgmManager : MonoBehaviour
{

    public AudioClip bgm1;
    public AudioClip bgm2;
    public AudioSource audioSource;
    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Map")
        {
            audioSource.clip = bgm1;
            audioSource.Play();

        }
        else if (SceneManager.GetActiveScene().name == "Game")
        {
            audioSource.clip = bgm2;
            audioSource.Play();

        }
        else
        {
            audioSource.clip = (Random.value > 0.5f) ? bgm1 : bgm2;
            audioSource.Play();
        }
        audioSource.volume = PlayerPrefs.GetInt("Music");
    }
}
