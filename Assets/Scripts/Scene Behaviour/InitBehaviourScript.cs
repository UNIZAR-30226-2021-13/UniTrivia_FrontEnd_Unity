using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitBehaviourScript : MonoBehaviour
{
    private IEnumerator sleepy;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
    }
}
