using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginBehaviourScript : MonoBehaviour
{
    //GUI Elements
    public Text IncorrectLogin;
    public InputField UserInput;
    public InputField PasswordInput;
    public Button LoginButton;
    public Button RecoverButton;
    public Button RegisterButton;
    public Button GuestButton;

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
        Button loginButton = LoginButton.GetComponent<Button>();
        Button recoverButton = RecoverButton.GetComponent<Button>();
        Button registerButton = RegisterButton.GetComponent<Button>();
        Button guestButton = GuestButton.GetComponent<Button>();

        loginButton.onClick.AddListener(LoginButtonOnClick);
        recoverButton.onClick.AddListener(RecoverButtonOnClick);
        registerButton.onClick.AddListener(RegisterButtonOnClick);
        guestButton.onClick.AddListener(GuestButtonOnClick);

        IncorrectLogin.gameObject.SetActive(false);
    }

    void LoginButtonOnClick()
    {
        IncorrectLogin.gameObject.SetActive(false);
        StartCoroutine(LoginRequest(UserInput.text, PasswordInput.text));
    }

    void RecoverButtonOnClick()
    {
        SceneManager.LoadScene("Recover Scene", LoadSceneMode.Single);
    }

    void RegisterButtonOnClick()
    {
        SceneManager.LoadScene("Register Scene", LoadSceneMode.Single);
    }

    void GuestButtonOnClick()
    {
        StartCoroutine(GuestRequest());
    }

    //Request to the server for Login
    private IEnumerator LoginRequest(string username, string password)
    {
        UnityWebRequest requestLogin = UnityWebRequest.Get("https://unitrivia.herokuapp.com/api/login");

        requestLogin.SetRequestHeader("username", username);
        requestLogin.SetRequestHeader("password", password);
        yield return requestLogin.SendWebRequest();
        Debug.Log("ResponseCode: " + requestLogin.responseCode);

        if (requestLogin.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + requestLogin.result);
        }
        else if (requestLogin.responseCode != 200)
        {
            IncorrectLogin.gameObject.SetActive(true);
            Debug.Log("ERROR LOGIN:" + requestLogin.downloadHandler.text);
            //ServerReturn result = ServerReturn.CreateFromJSON(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("EXITO LOGIN:" + requestLogin.downloadHandler.text);
            //Guardar Token usuario
            SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
        }
    }

    //Request to the server for Guest
    private IEnumerator GuestRequest()
    {
        UnityWebRequest requestGuest = UnityWebRequest.Get("https://unitrivia.herokuapp.com/api/logAsGuest");

        yield return requestGuest.SendWebRequest();
        Debug.Log("ResponseCode: " + requestGuest.responseCode);

        if (requestGuest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + requestGuest.result);
        }
        else
        {
            Debug.Log("EXITO LOGIN:" + requestGuest.downloadHandler.text);
            //Guardar Token usuario
            SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
        }
    }
}
