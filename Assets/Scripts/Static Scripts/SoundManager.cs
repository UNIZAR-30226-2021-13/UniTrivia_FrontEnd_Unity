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
    private static AudioSource optionAS;
    private static AudioSource correctAnswerAS;
    private static AudioSource incorrectAnswerAS;

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

        //Option
        optionAS = soundManagerGO.AddComponent<AudioSource>();
        optionAS.clip = Resources.Load<AudioClip>("Sounds/option");

        //Answers
        correctAnswerAS = soundManagerGO.AddComponent<AudioSource>();
        correctAnswerAS.clip = Resources.Load<AudioClip>("Sounds/acierto");
        incorrectAnswerAS = soundManagerGO.AddComponent<AudioSource>();
        incorrectAnswerAS.clip = Resources.Load<AudioClip>("Sounds/fallo");

        UnityEngine.Object.DontDestroyOnLoad(soundManagerGO);
    }

    public static void PlayMusic()
    {
        if (PlayerPrefs.GetInt("musicActive") == 1)
        {
            musicAS.volume = 0.03f;
            musicAS.loop = true;
            musicAS.Play();
        }
    }

    public static void StopMusic()
    {
        musicAS.Pause();
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

    public static void PlayOptionSound()
    {
        if (PlayerPrefs.GetInt("soundActive") == 1)
        {
            optionAS.Play();
        }
    }

    public static void PlayAnswerSound(bool correctAnswer)
    {
        if(PlayerPrefs.GetInt("soundActive") == 1)
        {
            if (correctAnswer)
            {
                correctAnswerAS.Play();
            }
            else
            {
                incorrectAnswerAS.Play();
            }
        }
        
    }
}
