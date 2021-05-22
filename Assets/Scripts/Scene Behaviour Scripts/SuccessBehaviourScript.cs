using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SuccessBehaviourScript : MonoBehaviour
{
    public Text successText;

    // Start is called before the first frame update
    void Start()
    {
        successText.text = SuccessDataScript.getSuccessText();

        StartCoroutine(WaitAndExit(2));
    }

    IEnumerator WaitAndExit(int time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        switch (SuccessDataScript.getReturnScene())
        {
            case "Login Scene":
                SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
                break;
            default:
                SceneManager.UnloadSceneAsync("Success Scene");
                break;
        }
    }
}
