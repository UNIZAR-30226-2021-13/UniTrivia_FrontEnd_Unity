using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBehaviourScript : MonoBehaviour
{
    public GameObject Board;
    public Button Dice;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;
    public GameObject Player5;
    public GameObject Player6;
    public GameObject PlayersGO;

    private GameObject[] Players;

    private Button[] boardFields;
    private int diceNumber;
    //Images are [Avatar,Token,Frame,qP,qO,qB,qG,qP,qY]
    /*
        Purple  -> Cultura General
        Orange  -> Deportes
        Blue    -> Geograf�a
        Green   -> Ciencias
        Pink    -> Entretenimiento
        Yellow  -> Historia

     */
    private readonly string[] categories = { "Cultura General", "Deportes", "Geografia", "Ciencias", "Entretenimiento", "Historia" };


    // Start is called before the first frame update
    void Start()
    {
        Players = new GameObject[] { Player1, Player2, Player3, Player4, Player5, Player6 };
        if (SocketioHandler.op.Equals("reconexion"))
        {
            startReconnection();
        }
        setConnectionHandlers();
        setPlayersAtStart();
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
            if(i < PlayersDataScript.jugadores.Count)
            {
                Players[i].name = PlayersDataScript.jugadores[i].nombre;
                Players[i].GetComponentInChildren<Text>().text = PlayersDataScript.jugadores[i].nombre;
                Players[i].GetComponentsInChildren<Image>(true)[0].sprite = Resources.Load<Sprite>("Avatar/" + PlayersDataScript.jugadores[i].avatar);
                Players[i].GetComponentsInChildren<Image>(true)[1].sprite = Resources.Load<Sprite>("Token/" + PlayersDataScript.jugadores[i].ficha);
                foreach(string img in PlayersDataScript.jugadores[i].quesitos)
                {
                    int idx = Array.FindIndex(categories, (c) => { return c.Equals(img); });
                    Players[i].GetComponentsInChildren<Image>(true)[3+idx].gameObject.SetActive(true);
                }
                //TODO Posicionar la casilla
            }
        }

    }

    private void playTurn(int player)
    {
        //Aqui habra que hacer waitTurn o similar cuando sea multi

    }

    private void useDice()
    {
        diceNumber = UnityEngine.Random.Range(1, 7);    //(minInclusive..maxExclusive)
    }

    private void newQuestion(string category)
    {
        //Solicitar nueva pregunta y gestionarlo
        QuestionDataScript.setQuestion("�1+1?","1","2","3","4",2);

        SceneManager.LoadScene("Question Scene", LoadSceneMode.Additive);
    }

    private void startReconnection()
    {
        Dictionary<string, Action<object>> handlers = new Dictionary<string, Action<object>>();
        handlers.Add("estadoPartida", (data) => { GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(estadoPartida((JObject)user))); });

        SocketioHandler.Start(() => { return; }, () =>
        {
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => SceneManager.LoadScene("Menu Scene", LoadSceneMode.Single));
        }, handlers);
    }

    private void setConnectionHandlers()
    {
        SocketioHandler.AddHandler("turno", (data) =>
        {
            // A qui�n le toca jugar
            // data = (string)
            GameBehaviourScript.ExecuteOnMainThread.Enqueue(() => StartCoroutine(turno((string)data)));
        });
        SocketioHandler.AddHandler("jugada", (data) =>
        {
            // Un jugador acaba de realizar una jugada
            // data = {
            //      user: (string),
            //      casilla: (int),
            //      ques: (string - vac�o si no consigue)
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
            // Un jugador que hab�a salido vuelve
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
            JValue jugadorJV = (JValue)j;
            JObject jugadorJO = (JObject)jugadorJV.Value;

            JValue tmp = (JValue)jugadorJO.Property("usuario").Value;
            string nombre = (string)tmp.Value;

            tmp = (JValue)jugadorJO.Property("casilla").Value;
            int posicion = (int)tmp.Value;

            tmp = (JValue)jugadorJO.Property("imgs").Value;
            JObject images = (JObject)tmp.Value;

            tmp = (JValue)images.Property("avatar").Value;
            string avatar = (string)tmp.Value;
            tmp = (JValue)images.Property("banner").Value;
            string banner = (string)tmp.Value;
            tmp = (JValue)images.Property("ficha").Value;
            string ficha = (string)tmp.Value;

            JArray quesitosJA = (JArray)jugadorJO.Property("quesitos").Value;
            string[] quesitos = new string[quesitosJA.Count] { };
            int i = 0;
            foreach(JToken q in quesitosJA)
            {
                quesitos[i++] = (string)((JValue)q).Value;
            }

            //Jugador(string nombre, string banner, string avatar, string ficha, int posicion, string[] quesitos)
            PlayersDataScript.Jugador jugador = new PlayersDataScript.Jugador(nombre, banner, avatar, ficha, posicion, quesitos);
            PlayersDataScript.nuevoJugador(jugador);
        }

        setPlayersAtStart();
        yield return null;
    }

    IEnumerator turno(string jugador)
    {
        //TODO implementar
        yield return null;
    }
    IEnumerator jugada(JObject data)
    {
        JValue userJV = (JValue)data.Property("user").Value;
        JValue casillaJV = (JValue)data.Property("casilla").Value;
        JValue quesJV = (JValue)data.Property("ques").Value;

        string user = (string)userJV.Value;
        string casilla = (string)casillaJV.Value;
        string ques = (string)quesJV.Value;

        //Actualizar quesito
        PlayersDataScript.jugadores[PlayersDataScript.index(user)].quesitos.Add(ques);
        int idx = Array.FindIndex(categories, (c) => { return c.Equals(ques); });
        PlayersGO.Find(user).GetComponentsInChildren<Image>(true)[3 + idx].gameObject.SetActive(true);

        //TODO mover ficha del jugador

        yield return null;
    }
    IEnumerator findDelJuego(string jugador)
    {
        //TODO implementar
        yield return null;
    }
    IEnumerator chat(JObject data)
    {
        JValue userJV = (JValue)data.Property("usuario").Value;
        JValue msgJV = (JValue)data.Property("msg").Value;

        string user = (string)userJV.Value;
        string msg = (string)msgJV.Value;


        //TODO implementar
        yield return null;
    }
    IEnumerator jugadorSale(string jugador)
    {
        //TOOD decidir qu� hace
        yield return null;
    }
    IEnumerator reconexionJugador(JObject data)
    {
        JValue userJV = (JValue)data.Property("user").Value;
        string user = (string)userJV.Value;

        JValue tmp = (JValue)data.Property("imgs").Value;
        JObject images = (JObject)tmp.Value;

        tmp = (JValue)images.Property("avatar").Value;
        string avatar = (string)tmp.Value;
        tmp = (JValue)images.Property("banner").Value;
        string banner = (string)tmp.Value;
        tmp = (JValue)images.Property("ficha").Value;
        string ficha = (string)tmp.Value;

        //TODO implementar
        yield return null;
    }
}