using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RequestServer : MonoBehaviour
{
    private const string URL = "https://unitrivia.com/api";
    private const string HOST = "unitrivia.com";
    //private const string API_KEY = "ENTER_YOUR_API_KEY_HERE";

    public void LoginUser()
    {
        string loginURL = URL + "/login";
        Debug.Log(loginURL);
        StartCoroutine(ProcessRequest(loginURL));
    }

    private IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
