using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Socket.Newtonsoft.Json.Linq;

public class LobbyBehaviourScript : MonoBehaviour
{
    public Text idSala;

    public VerticalLayoutGroup Usuarios;

    public Button StartButton;
    public Button CancelButton;
    public GameObject LoadingSprite;

    private int jugadores = 0;
    private bool cargando = false;

    public static Queue<Action> ExecuteOnMainThread = null;

    // Update is called once per frame
    void Update()
    {
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }
        if(jugadores >= 2 && !cargando)
        {
            StartButton.interactable = true;
        }
        else
        {
            StartButton.interactable = false;
        }
    }

    void OnApplicationQuit()
    {
        SocketioHandler.End();
    }

    // Start is called before the first frame update
    void Start(){

        if(ExecuteOnMainThread == null)
        {
            ExecuteOnMainThread = new Queue<Action>();
        }
        
        setLider(false);
        PlayersDataScript.turno = "";

        Dictionary<string, Action<object>> handlers = new Dictionary<string, Action<object>>();
        handlers.Add("nuevoJugador", (user) =>
        {
            JObject usuario = (JObject)user;
            LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(nuevoUsuario(usuario)))    ;
        });

        handlers.Add("cargarJugadores", (usuarios) => {
            LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(cargarJugadores((JObject)usuarios)));
        });

        handlers.Add("cambioLider", (data) =>
        {
            LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(cambioLider((JObject)data))) ;
        });

        handlers.Add("abandonoSala", (user) => {
            LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(salidaUsuario((string)user)));
        });

        handlers.Add("comienzoPartida", (_) => {
            LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => SceneManager.LoadScene("Game Scene", LoadSceneMode.Single));
        });

        handlers.Add("turno", (usuario) => { PlayersDataScript.turno = (string)usuario; });

        if(SocketioHandler.op.Equals("crearSala"))
        {
            StartCoroutine(setPropio());
            setLider(true);
        }

        SocketioHandler.Start(() => {
            if (SocketioHandler.op.Equals("crearSala"))
            {
                SocketioHandler.socket.Emit("obtenerIdSala", (id) => {
                    LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(setIdSala((string)id)));
                });
            }
        }, ()=> {
            if (LobbyBehaviourScript.ExecuteOnMainThread != null)
            {
                LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single));
            }
            if (GameBehaviourScript.ExecuteOnMainThread != null)
            {
                GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single));
            }
            SocketioHandler.End();
        },handlers);

        StartButton.onClick.AddListener(StartButtonOnClick);
        CancelButton.onClick.AddListener(CancelButtonOnClick);
    }

    IEnumerator cambioLider(JObject data)
    {
        JValue nuevo = (JValue)data.Property("nuevo").Value;
        string nuevoString = (string)nuevo.Value;
        JValue antiguo = (JValue)data.Property("antiguo").Value;
        string antiguoString = (string)antiguo.Value;
        
        if (nuevoString.Equals(UserDataScript.getInfo("username")))
        {
            setLider(true);
        }

        yield return salidaUsuario(antiguoString);

    }

    IEnumerator salidaUsuario(string nombre)
    {
        PlayersDataScript.eliminarJugador(nombre);
        jugadores--;
        GameObject child = Usuarios.transform.Find(nombre).gameObject;
        Destroy(child);
        
        yield return null;
    }

    void setUsuario(JObject usuario, string fieldName)
    {
        Debug.Log("Nuevo Usuario: " + usuario);
        JValue tmp = (JValue)usuario.Property(fieldName).Value;
        string nombre = (string)tmp.Value;

        string avatar = "avatar0", banner = "banner0", ficha = "ficha0";
        if (usuario.Property("imgs").Value.Type != JTokenType.Null)
        {
            JObject images = (JObject)usuario.Property("imgs").Value;

            tmp = (JValue)images.Property("avatar").Value;
            avatar = (string)tmp.Value;
            tmp = (JValue)images.Property("banner").Value;
            banner = (string)tmp.Value;
            tmp = (JValue)images.Property("ficha").Value;
            ficha = (string)tmp.Value;
        }

        //Jugador(string nombre, string banner, string avatar, string ficha, int posicion, string[] quesitos)
        PlayersDataScript.Jugador jugador = new PlayersDataScript.Jugador(nombre, banner, avatar, ficha, 777, new string[] { });
        PlayersDataScript.nuevoJugador(jugador);


        GameObject usuarioGO = new GameObject(nombre);
        usuarioGO.transform.SetParent(Usuarios.transform);

        RectTransform usuarioRT = usuarioGO.AddComponent<RectTransform>();
        usuarioRT.sizeDelta = new Vector2(450, 51);
        usuarioRT.localScale = new Vector3(1, 1, 1);

        Text usuarioText = usuarioGO.AddComponent<Text>();
        usuarioText.text = nombre;
        usuarioText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        usuarioText.alignment = TextAnchor.MiddleLeft;
        usuarioText.fontSize = 36;
        usuarioText.fontStyle = FontStyle.Bold;

        jugadores++;

    }

    IEnumerator nuevoUsuario(JObject usuario)
    {
        cargando = true;
        setUsuario(usuario, "jugador");
        yield return new WaitForSeconds(3);
        cargando = false;
    }

    IEnumerator cargarJugadores(JObject data)
    {
        JArray jugadores = (JArray)data.Property("jugadores").Value;
        Debug.Log("Número de jugadores en la sala: " + jugadores.Count);
        setLider(jugadores.Count <= 1);
        foreach (JToken j in jugadores)
        {
            JObject usuario = (JObject)j;
            setUsuario(usuario, "usuario");
        }

        SocketioHandler.socket.Emit("obtenerIdSala", (id) => {
            LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(setIdSala((string)id)));
        });
        yield return null;

    }

    IEnumerator setIdSala(String id)
    {
        LoadingSprite.SetActive(false);
        idSala.text = id;
        yield return null;
    }

    IEnumerator setPropio()
    {
        //Jugador(string nombre, string banner, string avatar, string ficha, int posicion, string[] quesitos)
        string nombre = UserDataScript.getInfo("username");
        string banner = UserDataScript.getInfo("banner");
        string avatar = UserDataScript.getInfo("avatar");
        string ficha  = UserDataScript.getInfo("token");

        PlayersDataScript.Jugador jugador = new PlayersDataScript.Jugador(nombre, banner, avatar, ficha, 777, new string[] { });
        PlayersDataScript.nuevoJugador(jugador);


        GameObject usuarioGO = new GameObject(nombre);
        usuarioGO.transform.SetParent(Usuarios.transform);

        RectTransform usuarioRT = usuarioGO.AddComponent<RectTransform>();
        usuarioRT.sizeDelta = new Vector2(450, 51);
        usuarioRT.localScale = new Vector3(1, 1, 1);

        Text usuarioText = usuarioGO.AddComponent<Text>();
        usuarioText.text = nombre;
        usuarioText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        usuarioText.alignment = TextAnchor.MiddleLeft;
        usuarioText.fontSize = 36;
        usuarioText.fontStyle = FontStyle.Bold;

        jugadores++;

        yield return null;
    }

    void setLider(bool lider)
    {
        try
        {
            if(lider)
            {
                StartButton.gameObject.SetActive(true);
            } else
            {
                StartButton.gameObject.SetActive(false);
            }
            
        }
        catch (Exception e)
        {
            Debug.Log("Exception setLider" + e.Message);
        }
    }

    void StartButtonOnClick()
    {
        SoundManager.PlayButtonSound();
        SocketioHandler.socket.Emit("comenzarPartida", (res) => {
            LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => comenzarPartida((JObject)res));
        });
    }

    void CancelButtonOnClick()
    {
        SoundManager.PlayButtonSound();
        //SceneManager.UnloadSceneAsync("Options Scene");
        SocketioHandler.End();
        SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
    }

    void comenzarPartida(JObject res)
    {
        JValue okJV = (JValue)res.Property("res").Value;
        string ok = (string)okJV.Value;
        JValue infoJV = (JValue)res.Property("info").Value;
        string info = (string)infoJV.Value;

        if (ok.Equals("ok")) //Ha ido bien
        {
            SceneManager.LoadScene("Game Scene", LoadSceneMode.Single);
        }
        else //Ha habido un error
        {
            ErrorDataScript.setErrorText(info);
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
    }
}
