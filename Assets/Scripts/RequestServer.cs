using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RequestServer : MonoBehaviour
{
    /*
    private const string URL = "https://unitrivia.herokuapp.com/api";
    private const string HOST = "unitrivia.herokuapp.com";
    private const string API_KEY = "";

    public void LoginUser(string username, string password)
    {
        string loginURL = URL + "/login";
        Debug.Log(loginURL);

        UnityWebRequest request = UnityWebRequest.Get(URL + "/login");
        request.SetRequestHeader("unity1", "unity");

        StartCoroutine(GetRequest(request));
    }

    private IEnumerator GetRequest(string )
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            request.SetRequestHeader("unity1", "unity");
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
    */
}
