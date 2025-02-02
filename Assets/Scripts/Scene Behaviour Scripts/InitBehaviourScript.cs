using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitBehaviourScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ErrorDataScript.setErrorText("No hay conexi�n a Internet");
            ErrorDataScript.setButtonMode(0);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        } else
        {
            if(!PlayerPrefs.HasKey("soundActive"))
            {
                PlayerPrefs.SetInt("soundActive", 1);
            }

            if (!PlayerPrefs.HasKey("musicActive"))
            {
                PlayerPrefs.SetInt("musicActive", 1);
            }

            SoundManager.Init();
            SoundManager.PlayMusic();

            SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
        }
    }
}
