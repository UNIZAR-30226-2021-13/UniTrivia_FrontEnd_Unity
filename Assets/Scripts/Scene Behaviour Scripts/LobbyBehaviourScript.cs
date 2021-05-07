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

    public Text WaitingLeader;
    public Button StartButton;
    public Button CancelButton;

    public Canvas ErrorCanvas;
    public Text ErrorText;
    public Button ErrorButton;

    private int jugadores = 1;

    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();

    // Update is called once per frame
    void Update()
    {
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }
        if(jugadores >= 2)
        {
            StartButton.interactable = true;
        }
    }


    // Start is called before the first frame update
    void Start(){
        ErrorCanvas.enabled = false;

        setLider(true);

        if(SocketioHandler.op.Equals("crearSala"))
        {
            StartCoroutine(setPropio());
        }

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

        SocketioHandler.Start(() => {
            SocketioHandler.socket.Emit("obtenerIdSala", (id) => {
                LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(setIdSala((string)id)));
            });
        }, ()=> {
            LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single));
        },handlers);

        StartButton.onClick.AddListener(StartButtonOnClick);
        CancelButton.onClick.AddListener(CancelButtonOnClick);

        ErrorButton.onClick.AddListener(() => {
            ErrorCanvas.enabled = false;
        });
    }

    IEnumerator cambioLider(JObject data)
    {
        JValue nuevo = (JValue)data.Property("nuevo").Value;
        string nuevoString = (string)nuevo.Value;
        JValue antiguo = (JValue)data.Property("antiguo").Value;
        string antiguoString = (string)antiguo.Value;

        Debug.Log(nuevo);
        Debug.Log(antiguo);
        
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

    IEnumerator nuevoUsuario(JObject usuario)
    {
        JValue tmp = (JValue)usuario.Property("jugador").Value;
        string nombre = (string)tmp.Value;

        tmp = (JValue)usuario.Property("imgs").Value;
        JObject images = (JObject)tmp.Value;

        tmp = (JValue)usuario.Property("avatar").Value;
        string avatar = (string)tmp.Value;
        tmp = (JValue)usuario.Property("banner").Value;
        string banner = (string)tmp.Value;
        tmp = (JValue)usuario.Property("ficha").Value;
        string ficha = (string)tmp.Value;

        //Jugador(string nombre, string banner, string avatar, string ficha, int posicion, string[] quesitos)
        PlayersDataScript.Jugador jugador = new PlayersDataScript.Jugador(nombre, banner, avatar, ficha, 777, new string[]{ });
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

    IEnumerator cargarJugadores(JObject data)
    {
        JArray jugadores = (JArray)data.Property("jugadores").Value;
        Debug.Log("Número de jugadores en la sala: " + jugadores.Count);
        if (jugadores.Count == 1) //Estoy yo solo en la sala
        {
            Debug.Log("Sos lider huevón");
            setLider(true);
        }
        else
        {
            Debug.Log("No sos lider huevón");

            setLider(false);

            foreach (JToken j in jugadores)
            {
                JValue jugador = (JValue)j;
                Debug.Log("klk manin 1");
                JObject userDisplay = (JObject)jugador.Value;
                Debug.Log("Añadiendo: " + userDisplay);
                StartCoroutine(nuevoUsuario(userDisplay));
                Debug.Log("Añadido");
            }
        }
        yield return null;

    }

    IEnumerator setIdSala(String id)
    {

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

        yield return null;
    }

    void setLider(bool lider)
    {
        try
        {
            WaitingLeader.gameObject.SetActive(!lider);
            StartButton.gameObject.SetActive(lider);
        }
        catch (Exception e)
        {
            Debug.Log("Exception setLider" + e.Message);
        }
    }

    void StartButtonOnClick()
    {
        SocketioHandler.socket.Emit("comenzarPartida", (res) => {
            LobbyBehaviourScript.ExecuteOnMainThread.Enqueue(() => comenzarPartida((JObject)res));
        });
    }

    void CancelButtonOnClick()
    {
        //SceneManager.UnloadSceneAsync("Options Scene");
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
            ErrorText.text = info;
            ErrorCanvas.enabled = true;
        }
    }

}
