using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MenuBehaviourScript : MonoBehaviour
{
    public Button RandomGameButton;
    public Button JoinGameButton;
    public Button CreateGameButton;
    public Button ProfileButton;
    public Button LoginButton;
    public Button OptionsButton;
    public Button ReconnectButton;

    // Canvas id to join game
    public Canvas JoinGameCanvas;
    public Button InputIDButton;
    public InputField IdentificadorInput;

    // Start is called before the first frame update
    void Start()
    {
        JoinGameCanvas.enabled = false;

        RandomGameButton.onClick.AddListener(RandomGameOnClick);
        JoinGameButton.onClick.AddListener(JoinGameOnClick);
        CreateGameButton.onClick.AddListener(CreateGameOnClick);
        ReconnectButton.onClick.AddListener(ReconnectOnClick);

        ProfileButton.onClick.AddListener(ProfileButtonOnClick);
        LoginButton.onClick.AddListener(LoginButtonOnClick);
        OptionsButton.onClick.AddListener(OptionsButtonOnClick);

        ReconnectButton.gameObject.SetActive(false);

        IdentificadorInput.onValueChanged.AddListener((id)=> { 
            InputIDButton.interactable = !string.IsNullOrEmpty(id) && id.Length == 5;
        });

        if (PlayerPrefs.GetString("Username").Equals("Invitado"))
        {
            ProfileButton.gameObject.SetActive(false);
        } else
        {
            LoginButton.gameObject.SetActive(false);
            StartCoroutine(checkUserReconnection());
        }
    }

    private IEnumerator checkUserReconnection()
    {
        UnityWebRequest requestPartida = UnityWebRequest.Get("http://unitrivia.herokuapp.com/api/partida/reconexion");
        requestPartida.SetRequestHeader("jwt", UserDataScript.getInfo("token"));
        yield return requestPartida.SendWebRequest();

        bool reconexion = requestPartida.downloadHandler.text != "";
        Debug.Log(requestPartida.downloadHandler.text);

        ReconnectButton.gameObject.SetActive(reconexion);
        RandomGameButton.gameObject.SetActive(!reconexion);
        JoinGameButton.gameObject.SetActive(!reconexion);
        CreateGameButton.gameObject.SetActive(!reconexion);
    }

    void JoinGameOnClick()
    {
        JoinGameCanvas.enabled = true;
        InputIDButton.onClick.AddListener(InputIDButtonOnClick);
    }

    void RandomGameOnClick()
    {
        Dictionary<string, string> args = new Dictionary<string, string>();
        SocketioHandler.Init("buscarPartida", args);
        SceneManager.LoadScene("Lobby Scene", LoadSceneMode.Single);
    }

    void CreateGameOnClick()
    {
        Dictionary<string, string> args = new Dictionary<string, string>();
        args.Add("priv", "true");
        SocketioHandler.Init("crearSala", args);
        SceneManager.LoadScene("Lobby Scene", LoadSceneMode.Single);
    }

    void ReconnectOnClick()
    {
        Dictionary<string, string> args = new Dictionary<string, string>();
        SocketioHandler.Init("reconexion", args);
        SceneManager.LoadScene("Game Scene", LoadSceneMode.Single);
    }

    void ProfileButtonOnClick()
    {
        SceneManager.LoadScene("Profile Scene", LoadSceneMode.Single);
    }

    void LoginButtonOnClick()
    {
        SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
    }

    void OptionsButtonOnClick()
    {
        SceneManager.LoadScene("Options Scene", LoadSceneMode.Single);
    }

    void InputIDButtonOnClick()
    {

        Dictionary<string, string> args = new Dictionary<string, string>();
        args.Add("sala", IdentificadorInput.text);
        SocketioHandler.Init("unirseSala", args);
        SceneManager.LoadScene("Lobby Scene", LoadSceneMode.Single);

    }
}
