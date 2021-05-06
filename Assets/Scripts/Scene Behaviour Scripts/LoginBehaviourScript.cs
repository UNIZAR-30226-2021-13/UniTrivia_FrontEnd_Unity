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

    // Start is called before the first frame update
    void Start()
    {
        LoginButton.onClick.AddListener(LoginButtonOnClick);
        RecoverButton.onClick.AddListener(RecoverButtonOnClick);
        RegisterButton.onClick.AddListener(RegisterButtonOnClick);
        GuestButton.onClick.AddListener(GuestButtonOnClick);

        UserDataScript.deleteInfo();
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

            ErrorDataScript.setErrorText("Error de conexión");
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else if (requestLogin.responseCode != 200)
        {
            Debug.Log("ERROR LOGIN:" + requestLogin.downloadHandler.text);
            ErrorReturn result = ErrorReturn.CreateFromJSON(requestLogin.downloadHandler.text);

            ErrorDataScript.setErrorText(result.message);
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
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

            ErrorDataScript.setErrorText("Error de conexión");
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("EXITO GUEST:" + requestGuest.downloadHandler.text);

            // Save guest data
            UserDataScript.setInfo("username", "Invitado");
            UserDataScript.setInfo("token", requestGuest.downloadHandler.text);

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

            ErrorDataScript.setErrorText("Error de conexión");
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else if (requestProfile.responseCode != 200)
        {
            Debug.Log("ERROR PROFILE:" + requestProfile.downloadHandler.text);
            ErrorReturn result = ErrorReturn.CreateFromJSON(requestProfile.downloadHandler.text);

            ErrorDataScript.setErrorText(result.message);
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("EXITO PROFILE:" + requestProfile.downloadHandler.text);
            ProfileReturn result = ProfileReturn.CreateFromJSON(requestProfile.downloadHandler.text);

            // Save player data
            UserDataScript.setInfo("token", token);
            UserDataScript.setInfo("username", result._id);
            UserDataScript.setInfo("email", result.mail);
            UserDataScript.setInfo("question", result.preg);
            UserDataScript.setInfo("answer", result.res);
            UserDataScript.setInfo("avatar", result.avtr);
            UserDataScript.setInfo("banner", result.bnr);
            UserDataScript.setInfo("ficha", result.fich);
            UserDataScript.setCoins(result.cns);
            UserDataScript.setStats(result.nj, result.ng);

            SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
        }
    }
}
