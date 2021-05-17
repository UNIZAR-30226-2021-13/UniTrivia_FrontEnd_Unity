using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public static class SoundManager
{
    private static AudioSource musicAS;
    private static AudioSource buttonAS;
    private static AudioSource chatAS;
    private static AudioSource diceAS;

    public static void Init()
    {
        GameObject soundManagerGO = new GameObject("SoundManager");

        //Music
        musicAS = soundManagerGO.AddComponent<AudioSource>();
        musicAS.clip = Resources.Load<AudioClip>("Sounds/musicadefondo");

        //Button
        buttonAS = soundManagerGO.AddComponent<AudioSource>();
        buttonAS.clip = Resources.Load<AudioClip>("Sounds/button");

        //NewChat
        chatAS = soundManagerGO.AddComponent<AudioSource>();
        chatAS.clip = Resources.Load<AudioClip>("Sounds/chat");

        //Dice
        diceAS = soundManagerGO.AddComponent<AudioSource>();
        diceAS.clip = Resources.Load<AudioClip>("Sounds/tiradadado");

        UnityEngine.Object.DontDestroyOnLoad(soundManagerGO);
    }

    public static void PlayMusic()
    {
        if (PlayerPrefs.GetInt("musicActive") == 1)
        {
            musicAS.volume = 0.01f;
            musicAS.loop = true;
            musicAS.Play();
        }
    }

    public static void PlayButtonSound()
    {
        if (PlayerPrefs.GetInt("soundActive") == 1)
        {
            buttonAS.Play();
        }
    }

    public static void PlayChatSound()
    {
        if (PlayerPrefs.GetInt("soundActive") == 1)
        {
            chatAS.Play();
        }
    }

    public static void PlayDiceSound()
    {
        if (PlayerPrefs.GetInt("soundActive") == 1)
        {
            diceAS.Play();
        }
    }
}
