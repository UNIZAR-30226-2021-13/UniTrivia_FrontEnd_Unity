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

        esReconexion();

        RandomGameButton.onClick.AddListener(RandomGameOnClick);
        JoinGameButton.onClick.AddListener(JoinGameOnClick);
        CreateGameButton.onClick.AddListener(ReconnectOnClick);
        ReconnectButton.onClick.AddListener(LoginButtonOnClick);

        ProfileButton.onClick.AddListener(ProfileButtonOnClick);
        LoginButton.onClick.AddListener(LoginButtonOnClick);
        OptionsButton.onClick.AddListener(OptionsButtonOnClick);

        if (PlayerPrefs.GetString("Username").Equals("Invitado"))
        {
            ProfileButton.gameObject.SetActive(false);
        } else
        {
            LoginButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if(string.IsNullOrEmpty(IdentificadorInput.text) | IdentificadorInput.text.Length != 6 )
        {
            InputIDButton.interactable = false;
        }
        else
        {
            InputIDButton.interactable = true;
        }
    }

    private IEnumerator esReconexion()
    {
        UnityWebRequest requestPartida = UnityWebRequest.Get("http://localhost:3000/api/partida/reconexion");
        requestPartida.SetRequestHeader("jwt", PlayerPrefs.GetString("Token"));
        yield return requestPartida.SendWebRequest();

        bool reconexion = requestPartida.downloadHandler.text == "";

        ReconnectButton.gameObject.SetActive(reconexion);
        RandomGameButton.gameObject.SetActive(!reconexion);
        JoinGameButton.gameObject.SetActive(!reconexion);
        CreateGameButton.gameObject.SetActive(!reconexion);

    }

    void JoinGameOnClick()
    {
        IdentificadorInput.enabled = false;
        InputIDButton.onClick.AddListener(InputIDButtonOnClick);
    }

    void RandomGameOnClick()
    {
        Dictionary<string, string> args = new Dictionary<string, string>();
        SocketioHandler.Init("buscarPartida", args);
        SceneManager.LoadScene("Waiting Scene", LoadSceneMode.Single);
    }

    void CreateGameOnClick()
    {
        Dictionary<string, string> args = new Dictionary<string, string>();
        args.Add("priv", "true");
        SocketioHandler.Init("crearSala", args);
        SceneManager.LoadScene("Waiting Scene", LoadSceneMode.Single);
    }

    void ReconnectOnClick()
    {
        Dictionary<string, string> args = new Dictionary<string, string>();
        SocketioHandler.Init("reconexion", args);
        SceneManager.LoadScene("Waiting Scene", LoadSceneMode.Single);
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
        args.Add("sala", "");
        SocketioHandler.Init("unirseSala", args);
        SceneManager.LoadScene("Waiting Scene", LoadSceneMode.Single);

    }
}
