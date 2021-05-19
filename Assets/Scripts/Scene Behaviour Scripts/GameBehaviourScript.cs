using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Socket.Newtonsoft.Json.Linq;

public class GameBehaviourScript : MonoBehaviour
{
    public GameObject BoardButtons;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;
    public GameObject Player5;
    public GameObject Player6;
    public GameObject PlayersGO;

    public GameObject TokensGO;
    public GameObject Token_Player1;
    public GameObject Token_Player2;
    public GameObject Token_Player3;
    public GameObject Token_Player4;
    public GameObject Token_Player5;
    public GameObject Token_Player6;

    //DADO
    public Canvas DiceGO;
    public Animation DiceAnimation;
    public Button Dice;

    //CHAT
    public Canvas ChatPannel;
    public Button chatButton;
    public Button send;
    public InputField msg;
    public Text chatLog;
    public Image aviso;

    //ENDGAME
    public GameObject endgame;
    public Image endgameImage;
    public InputField winner;
    public Button returnButton;

    private GameObject[] Players;
    private GameObject[] Tokens;

    private Button[] boardFields;
    private int diceNumber;
    //Images are [Avatar,Token,Frame,qP,qO,qB,qG,qP,qY]
    /*
        Purple  -> Cultura General
        Orange  -> Deportes
        Blue    -> Geografía
        Green   -> Ciencias
        Pink    -> Entretenimiento
        Yellow  -> Historia

     */
    private readonly string[] categories = { "Cultura General", "Deportes", "Geografia", "Ciencias", "Entretenimiento", "Historia" };

    
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            endgame.SetActive(false);
            returnButton.onClick.AddListener(ReturnButtonOnClick);

            //Dado
            DiceGO.enabled = false;
            Dice.interactable = false;
            Dice.onClick.AddListener(useDice);

            //Configurar chat
            ChatPannel.enabled = false;
            aviso.enabled = false;
            chatButton.onClick.AddListener(() => { ChatPannel.enabled = !ChatPannel.enabled; aviso.enabled = false; });
            msg.onValueChanged.AddListener((mensaje) => { send.interactable = !string.IsNullOrEmpty(mensaje) && mensaje.Length > 0; });
            send.onClick.AddListener(SendMsg);

            Players = new GameObject[] { Player1, Player2, Player3, Player4, Player5, Player6 };
            Tokens = new GameObject[] { Token_Player1, Token_Player2, Token_Player3, Token_Player4, Token_Player5, Token_Player6 };
            if (SocketioHandler.op.Equals("reconexion"))
            {
                startReconnection();
            }
            setConnectionHandlers();
            setPlayersAtStart();
        }
        catch(Exception e)
        {
            Debug.Log("Exception " + e);
        }
    }


    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();
    // Update is called once per frame
    void Update()
    {
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }

    }

    void OnApplicationQuit()
    {
        SocketioHandler.End();
    }

    private void setPlayersAtStart()
    {
        for(int i = 0; i < 6; i++)
        {
            Players[i].SetActive(i < PlayersDataScript.jugadores.Count);
            Tokens[i].SetActive(i < PlayersDataScript.jugadores.Count);
            if (i < PlayersDataScript.jugadores.Count)
            {
                Players[i].name = PlayersDataScript.jugadores[i].nombre;
                Players[i].GetComponentInChildren<Text>().text = PlayersDataScript.jugadores[i].nombre;
                Players[i].GetComponentsInChildren<Image>(true)[0].sprite = Resources.Load<Sprite>("Avatar/" + PlayersDataScript.jugadores[i].avatar);
                Players[i].GetComponentsInChildren<Image>(true)[1].sprite = Resources.Load<Sprite>("Token/" + PlayersDataScript.jugadores[i].ficha);
                Tokens[i].name = PlayersDataScript.jugadores[i].nombre;
                Tokens[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Token/" + PlayersDataScript.jugadores[i].ficha);
                foreach (string img in PlayersDataScript.jugadores[i].quesitos)
                {
                    int idx = Array.FindIndex(categories, (c) => { return c.Equals(img); });
                    Players[i].GetComponentsInChildren<Image>(true)[3+idx].gameObject.SetActive(true);
                }
                setTokenInPosition(Tokens[i], PlayersDataScript.jugadores[i].posicion);
            }
        }
        StartCoroutine(turno(PlayersDataScript.turno));
    }

    private void setTokenInPosition(GameObject token, int position)
    {
        //¿TODO ANIMATION y MULTIPOSICION?
        Debug.Log("BoardButton (" + position + ")");
        Transform c = BoardButtons.transform;
        Transform a = c.Find("BoardButton (" + position + ")");
        Transform b = token.transform;
        b.position = a.position;
    }

    private void startReconnection()
    {
        Dictionary<string, Action<object>> handlers = new Dictionary<string, Action<object>>();
        handlers.Add("estadoPartida", (data) => { GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(estadoPartida((JObject)data))); });

        SocketioHandler.Start(() => { return; }, () =>
        {
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single));
        }, handlers);
    }

    private void setConnectionHandlers()
    {
        SocketioHandler.AddHandler("turno", (data) =>
        {
            // A quién le toca jugar
            // data = (string)
            Debug.Log("turno " + data);
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(turno((string)data)));
        });
        SocketioHandler.AddHandler("jugada", (data) =>
        {
            // Un jugador acaba de realizar una jugada
            // data = {
            //      user: (string),
            //      casilla: (int),
            //      ques: (string - vacío si no consigue)
            // }
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(jugada((JObject)data)));
        });
        SocketioHandler.AddHandler("finDelJuego", (data) =>
        {
            // Fin del juego con el ganador
            // data = (string)
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(findDelJuego((string)data)));
        });
        SocketioHandler.AddHandler("chat", (data) => 
        {
            // Mensaje del chat
            // data = {
            //      usuario: (string),
            //      msg: (string)
            // }
            Debug.Log("Se lanza el evento de chat");
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(chat((JObject)data)));
        });
        SocketioHandler.AddHandler("jugadorSale", (data) =>
        {
            // Un jugador acaba de abandonar la partida
            // data = (string)
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(jugadorSale((string)data)));
        });
        SocketioHandler.AddHandler("reconexionJugador", (data) =>
        {
            // Un jugador que había salido vuelve
            // data = {
            //      user: (string),
            //      imgs: {
            //          avatar: (string),
            //          banner: (string),
            //          ficha: (string),
            //      }
            // }
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(reconexionJugador((JObject)data)));
        });
    }

    IEnumerator estadoPartida(JObject data)
    { 
        JArray jugadores = (JArray)data.Property("jugadores").Value;

        foreach (JToken j in jugadores)
        {
            JObject jugadorJO = (JObject)j;

            JValue tmp = (JValue)jugadorJO.Property("usuario").Value;
            string nombre = (string)tmp.Value;

            tmp = (JValue)jugadorJO.Property("casilla").Value;
            Debug.Log(tmp.Value.GetType());
            long posLong = (long)tmp.Value;
            int posicion = unchecked((int)posLong);

            JObject images = (JObject)jugadorJO.Property("imgs").Value;


            tmp = (JValue)images.Property("avatar").Value;
            string avatar = (string)tmp.Value;
            tmp = (JValue)images.Property("banner").Value;
            string banner = (string)tmp.Value;
            tmp = (JValue)images.Property("ficha").Value;
            string ficha = (string)tmp.Value;

            JArray quesitosJA = (JArray)jugadorJO.Property("quesitos").Value;
            List<string> quesitos = new List<string> { };
            foreach(JToken q in quesitosJA)
            {
                string queso = (string)((JValue)q).Value;
                Debug.Log(queso);
                quesitos.Add(queso);
            }

            //Jugador(string nombre, string banner, string avatar, string ficha, int posicion, string[] quesitos)
            PlayersDataScript.Jugador jugador = new PlayersDataScript.Jugador(nombre, banner, avatar, ficha, posicion, quesitos.ToArray());
            PlayersDataScript.nuevoJugador(jugador);
        }

        setPlayersAtStart();
        yield return null;
    }

    IEnumerator jugada(JObject data)
    {
        JValue userJV = (JValue)data.Property("user").Value;
        JValue casillaJV = (JValue)data.Property("casilla").Value;
        JValue quesJV = (JValue)data.Property("ques").Value;

        string user = (string)userJV.Value;
        long casillaLong = (long)casillaJV.Value;
        int casilla = unchecked((int)casillaLong);
        string ques = (string)quesJV.Value;

        //Actualizar quesito
        PlayersDataScript.jugadores[PlayersDataScript.index(user)].quesitos.Add(ques);
        int idx = Array.FindIndex(categories, (c) => { return c.Equals(ques); });
        PlayersGO.transform.Find(user).GetComponentsInChildren<Image>(true)[3 + idx].gameObject.SetActive(true);

        //Mover ficha del jugador
        PlayersDataScript.jugadores[PlayersDataScript.index(user)].posicion = casilla;
        setTokenInPosition(TokensGO.transform.Find(user).gameObject, casilla);

        yield return null;
    }

    IEnumerator findDelJuego(string jugador)
    {
        winner.text = jugador;

        if (jugador.Equals(UserDataScript.getInfo("username")))
        {
            endgameImage.sprite = Resources.Load<Sprite>("victory");
        } else
        {
            endgameImage.sprite = Resources.Load<Sprite>("defeat");
        }

        endgame.SetActive(true);

        yield return null;
    }

    IEnumerator jugadorSale(string jugador)
    {
        //Need debug
        checkColorPlayername(Player1, jugador, "desconexion");
        checkColorPlayername(Player2, jugador, "desconexion");
        checkColorPlayername(Player3, jugador, "desconexion");
        checkColorPlayername(Player4, jugador, "desconexion");
        checkColorPlayername(Player5, jugador, "desconexion");
        checkColorPlayername(Player6, jugador, "desconexion");

        yield return null;
    }

    IEnumerator reconexionJugador(JObject data)
    {
        JValue tmp = (JValue)data.Property("jugador").Value;
        string user = (string)tmp.Value;

        JObject images = (JObject)data.Property("imgs").Value;

        tmp = (JValue)images.Property("avatar").Value;
        string avatar = (string)tmp.Value;
        tmp = (JValue)images.Property("banner").Value;
        string banner = (string)tmp.Value;
        tmp = (JValue)images.Property("ficha").Value;
        string ficha = (string)tmp.Value;

        //TODO implementar
        yield return null;
    }


    ///////////////////////////////////////////////////////////////////////////////
    //
    //                             TURNO DEL JUGADOR
    //
    ///////////////////////////////////////////////////////////////////////////////
    
    // Gestión del evento de turno del socket
    IEnumerator turno(string jugador)
    {
        //Need debug
        checkColorPlayername(Player1, jugador, "turno");
        checkColorPlayername(Player2, jugador, "turno");
        checkColorPlayername(Player3, jugador, "turno");
        checkColorPlayername(Player4, jugador, "turno");
        checkColorPlayername(Player5, jugador, "turno");
        checkColorPlayername(Player6, jugador, "turno");

        //Our turn
        if (jugador.Equals(UserDataScript.getInfo("username")))
        {
            try
            {
                Debug.Log("Enters here - turno");
                DiceGO.enabled = true;
                Dice.interactable = true;
                DiceAnimation.Play(); //Lanzar animación
                Debug.Log("Exits - turno");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        yield return null;
    }

    // Función de pulsar el dado
    void useDice()
    {
        Debug.Log("Enters here - useDice");
        Dice.interactable = false;
        diceNumber = UnityEngine.Random.Range(1, 7);    //(minInclusive..maxExclusive)
        SocketioHandler.socket.Emit("posiblesJugadas", (jugadas) => {
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(posiblesJugadasCallback((JObject)jugadas))); 
        }, diceNumber);
    }

    IEnumerator posiblesJugadasCallback(JObject jugadas)
    {
        JValue resJV = (JValue)jugadas.Property("res").Value;
        string res = (string)resJV.Value;

        if (res.Equals("ok"))
        {
            JArray movimientos = (JArray)jugadas.Property("info").Value;
            foreach(JToken j in movimientos)
            {
                // j = {
                //      casilla: {
                //          num: <integer>,
                //          categoria: <string>,
                //          tipo: <string>
                //      },
                //      pregunta: {
                //          categoria: <string>,
                //          pregunta: <string>,
                //          resp_c: <string>,
                //          resp_inc: <Array<string>>,
                //          _id: <string>,
                //      }
                //  }
                JObject tmp = (JObject)j;
                JObject casillaJO = (JObject)tmp.Property("casilla").Value;
                JObject preguntaJO = (JObject)tmp.Property("pregunta").Value;

                int casilla = (int)((JValue)casillaJO.Property("num").Value).Value;
                string tipo = (string)((JValue)casillaJO.Property("tipo").Value).Value;
                if (tipo.Equals("dado"))
                {
                    BoardButtons.transform.Find("BoardButton (" + casilla + ")").GetComponent<Button>().onClick.AddListener(()=> {
                        hideBoardButtons();
                        SendJugada(casilla, "", false);
                        turno(UserDataScript.getInfo("username"));
                    });
                }
                else
                {
                    string categoria = (string)((JValue)casillaJO.Property("categoria").Value).Value;
                    string question = (string)((JValue)casillaJO.Property("question").Value).Value;
                    string resp_c = (string)((JValue)casillaJO.Property("resp_c").Value).Value;
                    JArray resp_incJA = (JArray)casillaJO.Property("resp_inc").Value;
                    List<string> resp_inc = new List<string>();
                    foreach(JToken r in resp_incJA)
                    {
                        resp_inc.Add((string)((JValue)r).Value);
                    }

                    BoardButtons.transform.Find("BoardButton (" + casilla + ")").GetComponent<Button>().onClick.AddListener(()=> {
                        hideBoardButtons();
                        newQuestion(tipo.Equals("quesito"), categoria, question, resp_c, resp_inc, casilla);
                    });
                }

                BoardButtons.transform.Find("BoardButton (" + casilla + ")").GetComponent<Button>().interactable = true;
                BoardButtons.transform.Find("BoardButton (" + casilla + ")").GetComponent<Image>().sprite = Resources.Load<Sprite>("Dice/dado_" + diceNumber);
            }
        }
        else
        {
            //TODO GESITONAR EL ERROR
            JValue infoJV = (JValue)jugadas.Property("info").Value;
            string info = (string)infoJV.Value;
        }

        yield return null;
    }

    private void hideBoardButtons()
    {
        foreach(Transform child in BoardButtons.transform) //Magically Unity provides the children of Board only in a foreach of its transform
        {
            child.GetComponent<Button>().interactable = false;
            child.GetComponent<Image>().sprite = Resources.Load<Sprite>("void");
        }
    }

    private void newQuestion(bool quesito, string category, string question, string correct, List<string> incorrects, int position)
    {
        //Lo mezcla, genera la posicion e inserta la respuesta correcta ahí
        Shuffle(ref incorrects);
        int idCorrect = UnityEngine.Random.Range(0, incorrects.Count + 1);
        incorrects.Insert(idCorrect, correct);

        QuestionDataScript.setQuestion(question,incorrects, idCorrect, quesito, category, position);

        SceneManager.LoadScene("Question Scene", LoadSceneMode.Additive);
    }
    public static void SendJugada(int casilla, string quesito, bool finTurno)
    {
        SocketioHandler.socket.Emit("actualizarJugada",casilla, quesito,finTurno);
    }

    ///////////////////////////////////////////////////////////////////////////////
    //
    //                              GESTION DEL CHAT
    //
    ///////////////////////////////////////////////////////////////////////////////

    //Cuando se pulsa el botón para enviar un mensaje
    void SendMsg()
    {
        chatLog.text = chatLog.text + UserDataScript.getInfo("username") + ": " + msg.text + "\n";
        SocketioHandler.socket.Emit("mensaje",msg.text);
        msg.text = "";
    }

    //Cuando llega un mensaje por el socket
    IEnumerator chat(JObject data)
    {
        Debug.Log("Nuevo mensaje");
        JValue userJV = (JValue)data.Property("usuario").Value;
        JValue msgJV = (JValue)data.Property("msg").Value;

        string user = (string)userJV.Value;
        string msg = (string)msgJV.Value;

        chatLog.text = chatLog.text + user + ": " + msg + "\n";
        aviso.enabled = !ChatPannel.enabled;

        yield return null;
    }

    void LocalMsg(string msg)
    {
        chatLog.text = chatLog.text + "..." + msg + "..." + "\n";
    }

    ///////////////////////////////////////////////////////////////////////////////
    //
    //                              UTILS
    //
    ///////////////////////////////////////////////////////////////////////////////
    
    //Source https://github.com/pdo400/smooth.foundations/blob/132960d2238f08b333035d9374569cbcc9b1b728/Assets/Smooth/Foundations/Collections/IListExtensions.cs#L26
    public static void Shuffle(ref List<string> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    void checkColorPlayername(GameObject player, string jugador, string func)
    {
        Text playername = player.GetComponentInChildren<Text>();

        switch (func)
        {
            case "turno":
                Debug.Log("COLOR.TURNO: " + playername.text + jugador);
                if(playername.text == jugador)
                {
                    playername.color = Color.green;
                } else if (playername.color != Color.gray)
                {
                    playername.color = Color.white;
                }
                break;
            case "desconexion":
                Debug.Log("COLOR.DESCNXN: " + playername.text + jugador);
                if (playername.text == jugador)
                {
                    playername.color = Color.gray;
                }
                break;
            default:
                break;
        }
    }

    void ReturnButtonOnClick()
    {
        //SceneManager.UnloadSceneAsync("Profile Scene");
        SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single);
    }
}

