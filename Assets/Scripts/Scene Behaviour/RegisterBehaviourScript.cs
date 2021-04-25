using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RegisterBehaviourScript : MonoBehaviour
{
    //GUI Elements
    public InputField UsernameInput;
    public InputField EmailInput;
    public InputField PasswordInput;
    public InputField RepasswordInput;
    public Image RepasswordImage;
    public InputField QuestionInput;
    public InputField AnswerInput;
    public Button RegisterButton;
    public Button ReturnButton;

    public Canvas ErrorCanvas;
    public Text ErrorMessage;
    public Button ErrorButton;

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
        RegisterButton.onClick.AddListener(RegisterButtonOnClick);
        ReturnButton.onClick.AddListener(ReturnButtonOnClick);
        ErrorButton.onClick.AddListener(ErrorButtonOnClick);

        RepasswordImage.enabled = false;
        ErrorCanvas.enabled = false;
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(RepasswordInput.text) & RepasswordInput.text != PasswordInput.text)
        {
            RepasswordImage.enabled = true;
        }
        else
        {
            RepasswordImage.enabled = false;
        }

        if (string.IsNullOrEmpty(UsernameInput.text) | string.IsNullOrEmpty(EmailInput.text) | 
            string.IsNullOrEmpty(PasswordInput.text) | string.IsNullOrEmpty(RepasswordInput.text) | 
            string.IsNullOrEmpty(QuestionInput.text) | string.IsNullOrEmpty(AnswerInput.text) |
            RepasswordInput.text != PasswordInput.text)
        {
            RegisterButton.interactable = false;
        } else
        {
            RegisterButton.interactable = true;
        }
    }

    void RegisterButtonOnClick()
    {
        //verifyButton.interactable = true;
        StartCoroutine(RegisterRequest(UsernameInput.text, PasswordInput.text, EmailInput.text, QuestionInput.text, AnswerInput.text));
    }

    void ReturnButtonOnClick()
    {
        SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
    }

    void ErrorButtonOnClick()
    {
        ErrorCanvas.enabled = false;
    }

    private IEnumerator RegisterRequest(string username, string password, string email, string preg, string res)
    {
        UnityWebRequest requestRegister = UnityWebRequest.Post("http://localhost:3000/api/register", "");

        requestRegister.SetRequestHeader("username", username);
        requestRegister.SetRequestHeader("password", password);
        requestRegister.SetRequestHeader("email", email);
        requestRegister.SetRequestHeader("preg", preg);
        requestRegister.SetRequestHeader("res", res);
        yield return requestRegister.SendWebRequest();
        Debug.Log("ResponseCode: " + requestRegister.responseCode);

        if (requestRegister.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + requestRegister.result);

            ErrorMessage.GetComponent<Text>().text = "Error de conexión";
            ErrorCanvas.enabled = true;
        }
        else if (requestRegister.responseCode != 200)
        {
            Debug.Log("ERROR REGISTRO:" + requestRegister.downloadHandler.text);
            ServerReturn result = ServerReturn.CreateFromJSON(requestRegister.downloadHandler.text);

            ErrorMessage.GetComponent<Text>().text = result.message;
            ErrorCanvas.enabled = true;
        }
        else
        {
            Debug.Log("EXITO REGISTRO:" + requestRegister.downloadHandler.text);
            //Guardar Token usuario
            SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
        }
    }
}
