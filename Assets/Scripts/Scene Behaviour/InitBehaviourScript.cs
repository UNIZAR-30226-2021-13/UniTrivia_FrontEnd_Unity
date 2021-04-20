using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitBehaviourScript : MonoBehaviour
{
    public Canvas ErrorCanvas;
    public Text ErrorMessage;
    public Button ErrorButton;

    // Start is called before the first frame update
    void Start()
    {
        ErrorButton.onClick.AddListener(ErrorButtonOnClick);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ErrorMessage.GetComponent<Text>().text = "No existe conexión a Internet";
            ErrorCanvas.enabled = true;
        } else
        {
            ErrorCanvas.enabled = false;

            SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
        }
    }

    void ErrorButtonOnClick()
    {
        Application.Quit();
    }
}
