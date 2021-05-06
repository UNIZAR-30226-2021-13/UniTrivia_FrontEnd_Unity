using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsBehaviourScript : MonoBehaviour
{
    public Button ReturnButton;

    // Start is called before the first frame update
    void Start()
    {
        ReturnButton.onClick.AddListener(ReturnButtonOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReturnButtonOnClick()
    {
        //SceneManager.UnloadSceneAsync("Options Scene");
        SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
    }
}
