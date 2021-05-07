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

    private GameObject[] Players;

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
        Players = new GameObject[] { Player1, Player2, Player3, Player4, Player5, Player6 };
        if (SocketioHandler.op.Equals("reconexion"))
        {
            startReconnection();
        }
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

    private void setPlayersAtStart()
    {
        for(int i = 0; i < 6; i++)
        {
            Players[i].SetActive(i < PlayersDataScript.jugadores.Count);
            if(i < PlayersDataScript.jugadores.Count)
            {
                Players[i].GetComponentInChildren<Text>().text = PlayersDataScript.jugadores[i].nombre;
                Players[i].GetComponentsInChildren<Image>(true)[0].sprite = Resources.Load<Sprite>("Avatar/" + PlayersDataScript.jugadores[i].avatar);
                Players[i].GetComponentsInChildren<Image>(true)[1].sprite = Resources.Load<Sprite>("Token/" + PlayersDataScript.jugadores[i].ficha);
                foreach(string img in PlayersDataScript.jugadores[i].quesitos)
                {
                    int idx = Array.FindIndex(categories, (c) => { return c.Equals(img); });
                    Players[i].GetComponentsInChildren<Image>(true)[idx].gameObject.SetActive(true);
                }
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
        QuestionDataScript.setQuestion("¿1+1?","1","2","3","4",2);

        SceneManager.LoadScene("Question Scene", LoadSceneMode.Additive);
    }

    private void startReconnection()
    {

    }
}
