using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginBehaviourScript : MonoBehaviour
{
    //GUI Elements
    public InputField UsernameInput;
    public InputField PasswordInput;
    public Button LoginButton;
    public Button RecoverButton;
    public Button RegisterButton;
    public Button GuestButton;

    public Canvas ErrorCanvas;
    public Text ErrorMessage;
    public Button ErrorButton;

    // Start is called before the first frame update
    void Start()
    {
        LoginButton.onClick.AddListener(LoginButtonOnClick);
        RecoverButton.onClick.AddListener(RecoverButtonOnClick);
        RegisterButton.onClick.AddListener(RegisterButtonOnClick);
        GuestButton.onClick.AddListener(GuestButtonOnClick);
        ErrorButton.onClick.AddListener(ErrorButtonOnClick);

        ErrorCanvas.enabled = false;

        PlayerPrefs.DeleteAll(); //Delete all previous player data
    }

    void Update()
    {
        if (string.IsNullOrEmpty(UsernameInput.text) | string.IsNullOrEmpty(PasswordInput.text))
        {
            LoginButton.interactable = false;
        }
        else
        {
            LoginButton.interactable = true;
        }
    }

    void LoginButtonOnClick()
    {
        StartCoroutine(LoginRequest(UsernameInput.text, PasswordInput.text));
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

    void ErrorButtonOnClick()
    {
        ErrorCanvas.enabled = false;
    }

    //Class for JSON deserializing
    [System.Serializable]
    public class ErrorReturn
    {
        public int code;
        public string message;

        public static ErrorReturn CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ErrorReturn>(jsonString);
        }
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

            //Show error in the auxiliar window
            ErrorMessage.GetComponent<Text>().text = "Error de conexión";
            ErrorCanvas.enabled = true;
        }
        else if (requestLogin.responseCode != 200)
        {
            Debug.Log("ERROR LOGIN:" + requestLogin.downloadHandler.text);
            ErrorReturn result = ErrorReturn.CreateFromJSON(requestLogin.downloadHandler.text);

            //Show error in the auxiliar window
            ErrorMessage.GetComponent<Text>().text = result.message;
            ErrorCanvas.enabled = true;
        }
        else
        {
            Debug.Log("EXITO LOGIN:" + requestLogin.downloadHandler.text);
            
            //Obtenemos la información del usuario
            StartCoroutine(ProfileRequest(requestLogin.downloadHandler.text));
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

            //Show error in the auxiliar window
            ErrorMessage.GetComponent<Text>().text = "Error de conexión";
            ErrorCanvas.enabled = true;
        }
        else
        {
            Debug.Log("EXITO GUEST:" + requestGuest.downloadHandler.text);

            // Save guest data
            PlayerPrefs.SetString("Username", "Invitado");
            PlayerPrefs.SetString("Token", requestGuest.downloadHandler.text);

            SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
        }
    }

    //Class for JSON deserializing
    [System.Serializable]
    public class ProfileReturn
    {
        public string preg;
        public string res;
        public int cns;
        public int ng;
        public string fich;
        public string bnr;
        public string[] comprados;
        public string avtr;
        public int nj;
        public string mail;
        public string _id;

        public static ProfileReturn CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ProfileReturn>(jsonString);
        }
    }

    private IEnumerator ProfileRequest(string token)
    {
        UnityWebRequest requestProfile = UnityWebRequest.Get("https://unitrivia.herokuapp.com/api/profile");
        requestProfile.SetRequestHeader("jwt", token);
        yield return requestProfile.SendWebRequest();

        if (requestProfile.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + requestProfile.result);

            //Show error in the auxiliar window
            ErrorMessage.GetComponent<Text>().text = "Error de conexión";
            ErrorCanvas.enabled = true;
        }
        else if (requestProfile.responseCode != 200)
        {
            Debug.Log("ERROR PROFILE:" + requestProfile.downloadHandler.text);
            ErrorReturn result = ErrorReturn.CreateFromJSON(requestProfile.downloadHandler.text);

            //Show error in the auxiliar window
            ErrorMessage.GetComponent<Text>().text = result.message;
            ErrorCanvas.enabled = true;
        }
        else
        {
            Debug.Log("EXITO PROFILE:" + requestProfile.downloadHandler.text);
            ProfileReturn result = ProfileReturn.CreateFromJSON(requestProfile.downloadHandler.text);

            // Save player data
            PlayerPrefs.SetString("Token", token);
            PlayerPrefs.SetString("Username", result._id);
            PlayerPrefs.SetString("Email", result.mail);
            PlayerPrefs.SetString("Question", result.preg);
            PlayerPrefs.SetString("Answer", result.res);
            PlayerPrefs.SetString("Avatar", result.avtr);
            PlayerPrefs.SetString("Banner", result.bnr);
            PlayerPrefs.SetString("Ficha", result.fich);
            PlayerPrefs.SetInt("Coins", result.cns);
            PlayerPrefs.SetInt("Played", result.nj);
            PlayerPrefs.SetInt("Wins", result.ng);

            SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
        }
    }
}
