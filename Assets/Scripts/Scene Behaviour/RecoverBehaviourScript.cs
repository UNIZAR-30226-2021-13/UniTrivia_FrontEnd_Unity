using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RecoverBehaviourScript : MonoBehaviour
{
    public InputField UsernameInput;
    public Button VerifyUserButton;

    public GameObject SecondContainer;
    public InputField QuestionInput;
    public InputField AnswerInput;
    public InputField PasswordInput;
    public InputField RepasswordInput;
    public Image RepasswordImage;
    
    public Button ConfirmButton;
    public Button ReturnButton;

    public Canvas ErrorCanvas;
    public Text ErrorMessage;
    public Button ErrorButton;

    // Start is called before the first frame update
    void Start()
    {
        VerifyUserButton.onClick.AddListener(VerifyUserButtonOnClick);
        ConfirmButton.onClick.AddListener(ConfirmButtonOnClick);
        ReturnButton.onClick.AddListener(ReturnButtonOnClick);
        ErrorButton.onClick.AddListener(ErrorButtonOnClick);

        SecondContainer.SetActive(false);

        ErrorCanvas.enabled = false;
    }

    void Update()
    {
        if (string.IsNullOrEmpty(UsernameInput.text))
        {
            VerifyUserButton.interactable = false;
        }
        else
        {
            VerifyUserButton.interactable = true;
        }

        if (!string.IsNullOrEmpty(RepasswordInput.text) & RepasswordInput.text != PasswordInput.text)
        {
            RepasswordImage.enabled = true;
        }
        else
        {
            RepasswordImage.enabled = false;
        }

        if (string.IsNullOrEmpty(UsernameInput.text) | string.IsNullOrEmpty(AnswerInput.text) | 
            string.IsNullOrEmpty(PasswordInput.text) | PasswordInput.text != RepasswordInput.text)
        {
            ConfirmButton.interactable = false;
        } else
        {
            ConfirmButton.interactable = true;
        }
    }

    void VerifyUserButtonOnClick()
    {
        StartCoroutine(RecoverQuestionRequest(UsernameInput.text));
    }

    void ConfirmButtonOnClick()
    {
        StartCoroutine(ChangePasswordRequest(UsernameInput.text, AnswerInput.text, PasswordInput.text));
    }

    void ReturnButtonOnClick()
    {
        SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
    }

    void ErrorButtonOnClick()
    {
        ErrorCanvas.enabled = false;
    }

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

    private IEnumerator RecoverQuestionRequest(string username)
    {
        UnityWebRequest recoverQuestionRequest = UnityWebRequest.Get("http://localhost:3000/api/login/recover/question");

        recoverQuestionRequest.SetRequestHeader("username", username);
        yield return recoverQuestionRequest.SendWebRequest();
        Debug.Log("ResponseCode: " + recoverQuestionRequest.responseCode);

        if (recoverQuestionRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + recoverQuestionRequest.result);

            ErrorMessage.GetComponent<Text>().text = "Error de conexión";
            ErrorCanvas.enabled = true;
        }
        else if (recoverQuestionRequest.responseCode != 200)
        {
            Debug.Log("ERROR LOGIN:" + recoverQuestionRequest.downloadHandler.text);
            ServerReturn result = ServerReturn.CreateFromJSON(recoverQuestionRequest.downloadHandler.text);

            ErrorMessage.GetComponent<Text>().text = result.message;
            ErrorCanvas.enabled = true;
        }
        else
        {
            Debug.Log("EXITO RECOVER:" + recoverQuestionRequest.downloadHandler.text);

            QuestionInput.text = recoverQuestionRequest.downloadHandler.text;
            UsernameInput.interactable = false;
            SecondContainer.SetActive(true);
        }
    }

    private IEnumerator ChangePasswordRequest(string username, string answer, string password)
    {
        UnityWebRequest requestChangePassword = UnityWebRequest.Post("http://localhost:3000/api/login/recover/password", "");

        requestChangePassword.SetRequestHeader("username", username);
        requestChangePassword.SetRequestHeader("res", answer);
        requestChangePassword.SetRequestHeader("newpassword", password);
        yield return requestChangePassword.SendWebRequest();
        Debug.Log("ResponseCode: " + requestChangePassword.responseCode);

        if (requestChangePassword.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + requestChangePassword.result);

            ErrorMessage.GetComponent<Text>().text = "Error de conexión";
            ErrorCanvas.enabled = true;
        }
        else if (requestChangePassword.responseCode != 200)
        {
            Debug.Log("ERROR LOGIN:" + requestChangePassword.downloadHandler.text);
            ServerReturn result = ServerReturn.CreateFromJSON(requestChangePassword.downloadHandler.text);

            ErrorMessage.GetComponent<Text>().text = result.message;
            ErrorCanvas.enabled = true;
        }
        else
        {
            Debug.Log("EXITO LOGIN:" + requestChangePassword.downloadHandler.text);
            SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
        }
    }
}
