using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmBehaviourScript : MonoBehaviour
{
    public Text confirmText;
    public Button confirmButton;
    public Button cancelButton;

    // Start is called before the first frame update
    void Start()
    {
        confirmText.text = ConfirmDataScript.getConfirmText();

        confirmButton.onClick.AddListener(ConfirmButtonOnClick);
        cancelButton.onClick.AddListener(CancelButtonOnClick);
    }

    private void ConfirmButtonOnClick()
    {
        SceneManager.UnloadSceneAsync("Confirm Scene");
    }

    private void CancelButtonOnClick()
    {
        SceneManager.UnloadSceneAsync("Confirm Scene");
    }
}
