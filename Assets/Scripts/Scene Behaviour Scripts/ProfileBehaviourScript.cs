using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ProfileBehaviourScript : MonoBehaviour
{
    public Button ManageButton;
    public Button ReturnButton;
    public Button ShopButton;

    //InfoMenu Section
    public Text Coins;
    public Text Username;
    public Image BannerImage;
    public Image AvatarImage;
    public Image FichaImage;

    //Stats Section
    public Text InfoText;
    public Button CloseSessionButton;

    //Manage Section
    public GameObject ManageCanvas;
    public InputField PasswordInput;
    public InputField RepasswordInput;
    public Image RepasswordImage;
    public Button ChangePasswordButton;
    public Button DeleteProfileButton;
    

    // Start is called before the first frame update
    void Start()
    {
        ManageButton.onClick.AddListener(ManageButtonOnClick);
        ReturnButton.onClick.AddListener(ReturnButtonOnClick);
        ShopButton.onClick.AddListener(ShopButtonOnClick);
        CloseSessionButton.onClick.AddListener(CloseSessionButtonOnClick);
        ChangePasswordButton.onClick.AddListener(ChangePasswordButtonOnClick);
        DeleteProfileButton.onClick.AddListener(DeleteProfileButtonOnClick);
  
        //Update user info
        StartCoroutine(ProfileRequest(UserDataScript.getInfo("token")));

        //Show new user info
        InfoText.text = "Usuario: " + UserDataScript.getInfo("username") + "\n"
            + "Email: " + UserDataScript.getInfo("email") + "\n\n"

            + "Pregunta: " + UserDataScript.getInfo("question") + "\n"
            + "Respuesta: " + UserDataScript.getInfo("answer") + "\n\n"

            + "Partidas jugadas: " + UserDataScript.getPlayed() + "\n"
            + "Partidas ganadas:" + UserDataScript.getWins() + "\n";

        Username.text = UserDataScript.getInfo("username");

        ManageCanvas.SetActive(false);
    }

    void Update()
    {
        Coins.text = "" + UserDataScript.getCoins();
        BannerImage.sprite = Resources.Load<Sprite>("Banner/" + UserDataScript.getInfo("banner"));
        AvatarImage.sprite = Resources.Load<Sprite>("Avatar/" + UserDataScript.getInfo("avatar"));
        FichaImage.sprite = Resources.Load<Sprite>("Token/" + UserDataScript.getInfo("ficha"));

        if (!string.IsNullOrEmpty(RepasswordInput.text) & RepasswordInput.text != PasswordInput.text)
        {
            RepasswordImage.enabled = true;
        }
        else
        {
            RepasswordImage.enabled = false;
        }

        if (string.IsNullOrEmpty(PasswordInput.text) | string.IsNullOrEmpty(RepasswordInput.text) |
            RepasswordInput.text != PasswordInput.text)
        {
            ChangePasswordButton.interactable = false;
        }
        else
        {
            ChangePasswordButton.interactable = true;
        }
    }

    void ManageButtonOnClick()
    {
        ManageCanvas.SetActive(!ManageCanvas.active);
    }

    void CloseSessionButtonOnClick()
    {
        //SceneManager.UnloadSceneAsync("Profile Scene");
        SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
    }

    void ChangePasswordButtonOnClick()
    {
        StartCoroutine(ChangePasswordRequest(UserDataScript.getInfo("username"), UserDataScript.getInfo("answer"), PasswordInput.text));
    }

    void DeleteProfileButtonOnClick()
    {
        //SceneManager.UnloadSceneAsync("Profile Scene");
        //SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);

        //ConfirmDataScript.setConfirmText("¿Estas seguro de eliminar tu cuenta?");
        //SceneManager.LoadScene("Confirm Scene", LoadSceneMode.Additive);
        StartCoroutine(DeleteProfileRequest(UserDataScript.getInfo("token")));
    }
    /*
    void ConfirmDeleteProfileButtonOnClick()
    {
        StartCoroutine(DeleteProfileRequest(PlayerPrefs.GetString("Token")));
    }
    */
    void ReturnButtonOnClick()
    {
        //SceneManager.UnloadSceneAsync("Profile Scene");
        SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
    }

    void ShopButtonOnClick()
    {
        SceneManager.LoadScene("Shop Scene", LoadSceneMode.Additive);
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

    private IEnumerator ChangePasswordRequest(string username, string answer, string password)
    {
        UnityWebRequest requestChangePassword = UnityWebRequest.Post("https://unitrivia.herokuapp.com/api/login/recover/password", "");

        requestChangePassword.SetRequestHeader("username", username);
        requestChangePassword.SetRequestHeader("res", answer);
        requestChangePassword.SetRequestHeader("newpassword", password);
        yield return requestChangePassword.SendWebRequest();
        Debug.Log("ResponseCode: " + requestChangePassword.responseCode);

        if (requestChangePassword.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + requestChangePassword.result);

            ErrorDataScript.setErrorText("Error de conexión");
            ErrorDataScript.setButtonMode(0);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else if (requestChangePassword.responseCode != 200)
        {
            Debug.Log("ERROR LOGIN:" + requestChangePassword.downloadHandler.text);
            ServerReturn result = ServerReturn.CreateFromJSON(requestChangePassword.downloadHandler.text);

            ErrorDataScript.setErrorText(result.message);
            ErrorDataScript.setButtonMode(0);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("EXITO LOGIN:" + requestChangePassword.downloadHandler.text);
            //SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
            ErrorDataScript.setErrorText("Contraseña cambiada");
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
    }

    private IEnumerator DeleteProfileRequest(string token)
    {
        UnityWebRequest requestDeleteProfile = UnityWebRequest.Delete("https://unitrivia.herokuapp.com/api/profile");

        requestDeleteProfile.SetRequestHeader("jwt", token);
        yield return requestDeleteProfile.SendWebRequest();
        Debug.Log("ResponseCode: " + requestDeleteProfile.responseCode);

        if (requestDeleteProfile.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + requestDeleteProfile.result);

            ErrorDataScript.setErrorText("Error de conexión");
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else if (requestDeleteProfile.responseCode != 200)
        {
            Debug.Log("ERROR DELETE:" + requestDeleteProfile.downloadHandler.text);
            ServerReturn result = ServerReturn.CreateFromJSON(requestDeleteProfile.downloadHandler.text);

            ErrorDataScript.setErrorText(result.message);
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("EXITO DELETE:" + requestDeleteProfile.downloadHandler.text);
            SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
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
        public string[] rfs;
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
            ServerReturn result = ServerReturn.CreateFromJSON(requestProfile.downloadHandler.text);

            ErrorDataScript.setErrorText(result.message);
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("EXITO PROFILE:" + requestProfile.downloadHandler.text);
            ProfileReturn result = ProfileReturn.CreateFromJSON(requestProfile.downloadHandler.text);

            // Save player data
            UserDataScript.setInfo("username", result._id);
            UserDataScript.setInfo("email", result.mail);
            UserDataScript.setInfo("question", result.preg);
            UserDataScript.setInfo("answer", result.res);
            UserDataScript.setInfo("avatar", result.avtr);
            UserDataScript.setInfo("banner", result.bnr);
            UserDataScript.setInfo("ficha", result.fich);
            UserDataScript.setCoins(result.cns);
            UserDataScript.setStats(result.nj, result.ng);
            UserDataScript.setItems(result.rfs);
        }
    }


    
}
