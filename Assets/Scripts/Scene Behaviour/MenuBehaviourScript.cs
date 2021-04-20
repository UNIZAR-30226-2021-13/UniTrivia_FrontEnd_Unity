using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviourScript : MonoBehaviour
{
    public Button RandomGameButton;
    public Button JoinGameButton;
    public Button CreateGameButton;
    public Button ProfileButton;
    public Button LoginButton;
    public Button OptionsButton;

    // Start is called before the first frame update
    void Start()
    {
        RandomGameButton.interactable = false; //RandomGameButton.onClick.AddListener(LoginButtonOnClick);
        JoinGameButton.interactable = false; //JoinGameButton.onClick.AddListener(LoginButtonOnClick);
        CreateGameButton.interactable = false; //CreateGameButton.onClick.AddListener(LoginButtonOnClick);
        ProfileButton.onClick.AddListener(ProfileButtonOnClick);
        LoginButton.onClick.AddListener(LoginButtonOnClick);
        OptionsButton.onClick.AddListener(OptionsButtonOnClick);

        if (PlayerPrefs.GetString("Username").Equals("Invitado"))
        {
            ProfileButton.gameObject.SetActive(false);
        } else
        {
            LoginButton.gameObject.SetActive(false);
        }
    }

    void ProfileButtonOnClick()
    {
        SceneManager.LoadScene("Profile Scene", LoadSceneMode.Single);
    }

    void LoginButtonOnClick()
    {
        SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
    }

    void OptionsButtonOnClick()
    {
        SceneManager.LoadScene("Options Scene", LoadSceneMode.Single);
    }
}
