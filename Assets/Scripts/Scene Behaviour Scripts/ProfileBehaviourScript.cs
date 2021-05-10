using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ProfileBehaviourScript : MonoBehaviour
{
    public Button CloseSessionButton;
    public Button ReturnButton;

    //Content Tab One
    public Text InfoText;

    //Content Tab Two
    public GameObject ContentTabTwo;
    public InputField PasswordInput;
    public InputField RepasswordInput;
    public Image RepasswordImage;
    public Button ChangePasswordButton;
    public Button DeleteProfileButton;
    public Button ShopButton;

    // Start is called before the first frame update
    void Start()
    {
        CloseSessionButton.onClick.AddListener(CloseSessionButtonOnClick);
        ChangePasswordButton.onClick.AddListener(ChangePasswordButtonOnClick);
        DeleteProfileButton.onClick.AddListener(DeleteProfileButtonOnClick);
        ReturnButton.onClick.AddListener(ReturnButtonOnClick);
        ShopButton.onClick.AddListener(ShopButtonOnClick);

        InfoText.text = "Usuario: " + UserDataScript.getInfo("username") + "\n"
            + "Email: " + UserDataScript.getInfo("email") + "\n\n"
            + "Pregunta: " + UserDataScript.getInfo("question") + "\n"
            + "Respuesta: " + UserDataScript.getInfo("answer") + "\n\n"
            + "Monedas: " + UserDataScript.getCoins() + "\n\n"
            + "Partidas jugadas: " + UserDataScript.getPlayed() + "\n"
            + "Partidas ganadas:" + UserDataScript.getWins() + "\n";

        ContentTabTwo.SetActive(false);
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

    void CloseSessionButtonOnClick()
    {
        //SceneManager.UnloadSceneAsync("Profile Scene");
        SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
    }

    void ChangePasswordButtonOnClick()
    {
        StartCoroutine(ChangePasswordRequest(PlayerPrefs.GetString("Username"), PlayerPrefs.GetString("Answer"), PasswordInput.text));
    }

    void DeleteProfileButtonOnClick()
    {
        //SceneManager.UnloadSceneAsync("Profile Scene");
        //SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);

        ConfirmDataScript.setConfirmText("¿Estas seguro de eliminar tu cuenta?");
        SceneManager.LoadScene("Confirm Scene", LoadSceneMode.Additive);
    }

    void ConfirmDeleteProfileButtonOnClick()
    {
        StartCoroutine(DeleteProfileRequest(PlayerPrefs.GetString("Token")));
    }

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
        UnityWebRequest requestChangePassword = UnityWebRequest.Post("http://localhost:3000/api/login/recover/password", "");

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
            SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
        }
    }

    private IEnumerator DeleteProfileRequest(string token)
    {
        UnityWebRequest requestDeleteProfile = UnityWebRequest.Delete("http://localhost:3000/api/profile");

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
}
