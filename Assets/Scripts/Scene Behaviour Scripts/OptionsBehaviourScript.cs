using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsBehaviourScript : MonoBehaviour
{
    public Toggle soundOption;
    public Toggle musicOption;
    public Button ReturnButton;

    // Start is called before the first frame update
    void Start()
    {
        ReturnButton.onClick.AddListener(ReturnButtonOnClick);

        soundOption.isOn = PlayerPrefs.GetInt("soundActive") == 1;
        musicOption.isOn = PlayerPrefs.GetInt("musicActive") == 1;
        soundOption.onValueChanged.AddListener(soundOptionChanged);
        musicOption.onValueChanged.AddListener(musicOptionChanged);
    }

    void soundOptionChanged(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("soundActive", 1);
            SoundManager.PlayOptionSound();
        } else
        {
            PlayerPrefs.SetInt("soundActive", 0);
        }
    }

    void musicOptionChanged(bool isOn)
    {
        SoundManager.PlayOptionSound();
        if (isOn)
        {
            PlayerPrefs.SetInt("musicActive", 1);
            SoundManager.PlayMusic();
        }
        else
        {
            PlayerPrefs.SetInt("musicActive", 0);
            SoundManager.StopMusic();
        }
    }

    void ReturnButtonOnClick()
    {
        SoundManager.PlayButtonSound();
        //SceneManager.UnloadSceneAsync("Options Scene");
        SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
    }
}
