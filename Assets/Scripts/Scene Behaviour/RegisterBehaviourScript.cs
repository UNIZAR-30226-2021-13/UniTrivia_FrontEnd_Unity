using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RegisterBehaviourScript : MonoBehaviour
{
    //GUI Elements
    public InputField UserInput;
    public InputField EmailInput;
    public InputField PasswordInput;
    public InputField RepasswordInput;
    public InputField QuestionInput;
    public InputField AnswerInput;
    public Button RegisterButton;

    //Class for JSON deserializing
    [System.Serializable]
    public class ServerReturn
    {
        public int code;
        public string message;

        public static ServerReturn CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ServerReturn>(jsonString);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Button registerButton  = RegisterButton.GetComponent<Button>();

        registerButton.onClick.AddListener(RegisterButtonOnClick);
    }

    void RegisterButtonOnClick()
    {
        //verifyButton.interactable = true;
        //SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
        StartCoroutine(RegisterRequest(UserInput.text, PasswordInput.text, EmailInput.text, QuestionInput.text, AnswerInput.text));
    }

    private IEnumerator RegisterRequest(string username, string password, string email, string preg, string res)
    {
        UnityWebRequest request = UnityWebRequest.Post("https://unitrivia.herokuapp.com/api/register", "");

        request.SetRequestHeader("username", username);
        request.SetRequestHeader("password", password);
        request.SetRequestHeader("email", email);
        request.SetRequestHeader("preg", preg);
        request.SetRequestHeader("res", res);
        yield return request.SendWebRequest();
        Debug.Log("ResponseCode: " + request.responseCode);

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + request.result);
        }
        else if (request.responseCode != 200)
        {
            Debug.Log("ERROR REGISTRO:" + request.downloadHandler.text);
            //ServerReturn result = ServerReturn.CreateFromJSON(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("EXITO REGISTRO:" + request.downloadHandler.text);
            //Guardar Token usuario
            SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
        }
    }
}
