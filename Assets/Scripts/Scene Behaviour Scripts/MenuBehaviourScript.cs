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

    //InfoMenu
    public Text Coins;
    public Text Username;
    public Image UserAvatar;

    // Canvas id to join game
    public Canvas JoinGameCanvas;
    public Button InputIDButton;
    public InputField IdentificadorInput;
    public Button ReturnButton;

    // Start is called before the first frame update
    void Start()
    {
        //Desactivamos botones hasta gestionar la reconexión si hay
        JoinGameCanvas.enabled = false;
        ReconnectButton.gameObject.SetActive(false);
        RandomGameButton.gameObject.SetActive(false);
        JoinGameButton.gameObject.SetActive(false);
        CreateGameButton.gameObject.SetActive(false);

        //Add Listeners
        RandomGameButton.onClick.AddListener(RandomGameOnClick);
        JoinGameButton.onClick.AddListener(JoinGameOnClick);
        CreateGameButton.onClick.AddListener(CreateGameOnClick);
        ReconnectButton.onClick.AddListener(ReconnectOnClick);

        InputIDButton.onClick.AddListener(InputIDButtonOnClick);
        ReturnButton.onClick.AddListener(ReturnButtonOnClick);

        ProfileButton.onClick.AddListener(ProfileButtonOnClick);
        LoginButton.onClick.AddListener(LoginButtonOnClick);
        OptionsButton.onClick.AddListener(OptionsButtonOnClick);

        //UserInfo
        Username.text = UserDataScript.getInfo("username");

        if (UserDataScript.getInfo("username").StartsWith("Guest_"))
        {
            Resources.Load<Sprite>("Avatar/avatar0");
            ProfileButton.gameObject.SetActive(false);
        }
        else
        {
            Resources.Load<Sprite>("Avatar/" + PlayerPrefs.GetString("avatar"));
            LoginButton.gameObject.SetActive(false);
        }
        StartCoroutine(checkUserReconnection());

        IdentificadorInput.onValueChanged.AddListener((id)=> { 
            InputIDButton.interactable = !string.IsNullOrEmpty(id) && id.Length == 5;
        });
 
    }

    void Update()
    {
        UserAvatar.sprite = Resources.Load<Sprite>("Avatar/" + UserDataScript.getInfo("avatar"));
        Coins.text = "" + UserDataScript.getCoins();
    }

    private IEnumerator checkUserReconnection()
    {
        UnityWebRequest requestPartida = UnityWebRequest.Get("https://unitrivia.herokuapp.com/api/partida/reconexion");
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
        SoundManager.PlayButtonSound();
        JoinGameCanvas.enabled = true;
        
    }

    void RandomGameOnClick()
    {
        SoundManager.PlayButtonSound();
        Dictionary<string, string> args = new Dictionary<string, string>();
        SocketioHandler.Init("buscarPartida", args);
        SceneManager.LoadScene("Lobby Scene", LoadSceneMode.Single);
    }

    void CreateGameOnClick()
    {
        SoundManager.PlayButtonSound();
        Dictionary<string, string> args = new Dictionary<string, string>();
        args.Add("priv", "true");
        SocketioHandler.Init("crearSala", args);
        SceneManager.LoadScene("Lobby Scene", LoadSceneMode.Single);
    }

    void ReconnectOnClick()
    {
        SoundManager.PlayButtonSound();
        Dictionary<string, string> args = new Dictionary<string, string>();
        SocketioHandler.Init("reconexion", args);
        SceneManager.LoadScene("Game Scene", LoadSceneMode.Single);
    }

    void ProfileButtonOnClick()
    {
        SoundManager.PlayButtonSound();
        SceneManager.LoadScene("Profile Scene", LoadSceneMode.Single);
    }

    void LoginButtonOnClick()
    {
        SoundManager.PlayButtonSound();
        SceneManager.LoadScene("Login Scene", LoadSceneMode.Single);
    }

    void OptionsButtonOnClick()
    {
        SoundManager.PlayButtonSound();
        SceneManager.LoadScene("Options Scene", LoadSceneMode.Single);
    }

    void ReturnButtonOnClick()
    {
        JoinGameCanvas.enabled = false;
    }

    void InputIDButtonOnClick()
    {
        SoundManager.PlayButtonSound();
        Dictionary<string, string> args = new Dictionary<string, string>();
        args.Add("sala", IdentificadorInput.text);
        SocketioHandler.Init("unirseSala", args);
        SceneManager.LoadScene("Lobby Scene", LoadSceneMode.Single);
    }
}
