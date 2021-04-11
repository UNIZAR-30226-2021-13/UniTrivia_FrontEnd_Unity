using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RecoverBehaviourScript : MonoBehaviour
{
    public Button EmailButton;
    public Button VerifyButton;
    public Button PasswordButton;

    Button emailButton;
    Button verifyButton;
    Button passwordButton;

    // Start is called before the first frame update
    void Start()
    {
        emailButton = EmailButton.GetComponent<Button>();
        verifyButton = VerifyButton.GetComponent<Button>();
        passwordButton = PasswordButton.GetComponent<Button>();

        emailButton.onClick.AddListener(EmailButtonOnClick);
        verifyButton.onClick.AddListener(VerifyButtonOnClick);
        passwordButton.onClick.AddListener(PasswordButtonOnClick);

        verifyButton.interactable = false;
        passwordButton.interactable = false;
    }

    void EmailButtonOnClick()
    {
        verifyButton.interactable = true;
    }

    void VerifyButtonOnClick()
    {
        passwordButton.interactable = true;
    }

    void PasswordButtonOnClick()
    {
        SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
    }
}
