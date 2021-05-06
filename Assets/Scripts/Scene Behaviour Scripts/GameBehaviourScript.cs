using System.Collections;
using System.Collections.Generic;
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

    private Button[] boardFields;
    private int diceNumber;
    //Images are [Avatar,Token,Frame,qP,qO,qB,qG,qP,qY]
    private Text nameP1;
    private Image[] imgP1;
    private Text nameP2;
    private Image[] imgP2;
    private Text nameP3;
    private Image[] imgP3;
    private Text nameP4;
    private Image[] imgP4;
    private Text nameP5;
    private Image[] imgP5;
    private Text nameP6;
    private Image[] imgP6;

    // Start is called before the first frame update
    void Start()
    {
        setPlayersAtStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setPlayersAtStart()
    {
        nameP1 = Player1.GetComponentInChildren<Text>();
        imgP1 = Player1.GetComponentsInChildren<Image>(true);
        nameP2 = Player2.GetComponentInChildren<Text>();
        imgP2 = Player2.GetComponentsInChildren<Image>(true);
        nameP3 = Player3.GetComponentInChildren<Text>();
        imgP3 = Player3.GetComponentsInChildren<Image>(true);
        nameP4 = Player4.GetComponentInChildren<Text>();
        imgP4 = Player4.GetComponentsInChildren<Image>(true);
        nameP5 = Player5.GetComponentInChildren<Text>();
        imgP5 = Player5.GetComponentsInChildren<Image>(true);
        nameP6 = Player6.GetComponentInChildren<Text>();
        imgP6 = Player6.GetComponentsInChildren<Image>(true);

        //Test modificación jugador. Sustituir.
        nameP1.text = "TestPlayer1";
        imgP1[0].sprite = Resources.Load<Sprite>("Avatar/avatar1");
        imgP1[1].sprite = Resources.Load<Sprite>("Token/ficha1");
        imgP1[5].gameObject.SetActive(true);
    }

    private void playTurn(int player)
    {
        //Aqui habra que hacer waitTurn o similar cuando sea multi

    }

    private void useDice()
    {
        diceNumber = Random.Range(2, 7);    //(minInclusive..maxExclusive)
    }

    private void newQuestion(string category)
    {
        //Solicitar nueva pregunta y gestionarlo
        QuestionDataScript.setQuestion("¿1+1?","1","2","3","4",2);

        SceneManager.LoadScene("Question Scene", LoadSceneMode.Additive);
    }
}
