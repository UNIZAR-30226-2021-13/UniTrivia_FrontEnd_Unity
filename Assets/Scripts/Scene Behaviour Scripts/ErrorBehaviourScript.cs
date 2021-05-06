using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ErrorBehaviourScript : MonoBehaviour
{
    public Text errorText;
    public Button errorButton;

    // Start is called before the first frame update
    void Start()
    {
        errorText.text = ErrorDataScript.getErrorText();

        errorButton.onClick.AddListener(ErrorButtonOnClick);
    }

    private void ErrorButtonOnClick()
    {
        if(ErrorDataScript.getButtonMode() == 0)
        {
            Application.Quit();
        } else
        {
            SceneManager.UnloadSceneAsync("Error Scene");
        }
    }
}
