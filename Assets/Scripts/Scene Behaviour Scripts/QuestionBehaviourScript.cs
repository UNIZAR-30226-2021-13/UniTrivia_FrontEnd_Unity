using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class QuestionBehaviourScript : MonoBehaviour
{
    public Text questionText;
    public Image timerBar;
    public Button answerOneButton;
    public Button answerTwoButton;
    public Button answerThreeButton;
    public Button answerFourButton;

    private float timeQuestion = 60.0f;
    private bool answered = false;

    // Start is called before the first frame update
    void Start()
    {
        questionText.text = QuestionDataScript.getQuestion();

        string[] answers = QuestionDataScript.getAnswers();
        answerOneButton.GetComponentInChildren<Text>().text = answers[0];
        answerTwoButton.GetComponentInChildren<Text>().text = answers[1];

        if(answers.Length > 2)
        {
            answerThreeButton.GetComponentInChildren<Text>().text = answers[2];
            if(answers.Length > 3)
            {
                answerFourButton.GetComponentInChildren<Text>().text = answers[3];
            } else
            {
                answerFourButton.GetComponentInChildren<Text>().text = "";
                answerFourButton.interactable = false;
            }
        } else
        {
            answerThreeButton.GetComponentInChildren<Text>().text = "";
            answerThreeButton.interactable = false;
            answerFourButton.GetComponentInChildren<Text>().text = "";
            answerFourButton.interactable = false;
        }
        
        answerOneButton.onClick.AddListener(AnswerOneButtonOnClick);
        answerTwoButton.onClick.AddListener(AnswerTwoButtonOnClick);
        answerThreeButton.onClick.AddListener(AnswerThreeButtonOnClick);
        answerFourButton.onClick.AddListener(AnswerFourButtonOnClick);
    }

    void Update()
    {
        if(!answered)
        {
            timeQuestion -= Time.deltaTime;
            timerBar.fillAmount = timeQuestion / 60.0f;

            if (timeQuestion >= 30.0f)
            {
                timerBar.color = Color.green;
            }
            else if (timeQuestion >= 15.0f)
            {
                timerBar.color = Color.yellow;
            }
            else if (timeQuestion >= 0.0f)
            {
                timerBar.color = Color.red;
            }
            else
            {
                SoundManager.PlayAnswerSound(false);
                answersNonInteractable();
                GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), "", false);
                StartCoroutine(WaitAndExit(3));
            }
        }
    }

    private void AnswerOneButtonOnClick()
    {
        answersNonInteractable();
        answered = true;
        if (QuestionDataScript.getCorrectAnswer() == 1)
        {
            string quesito = QuestionDataScript.getCategory();
            answerOneButton.GetComponent<Image>().color = Color.green;
            SoundManager.PlayAnswerSound(true);
            addCoin();
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), QuestionDataScript.getQuesito()? quesito : "", !QuestionDataScript.getQuesito());
            
        }
        else
        {
            answerOneButton.GetComponent<Image>().color = Color.red;
            SoundManager.PlayAnswerSound(false);
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(),"",false);
        }
        StartCoroutine(WaitAndExit(3));
    }

    private void AnswerTwoButtonOnClick()
    {
        answersNonInteractable();
        answered = true;
        if (QuestionDataScript.getCorrectAnswer() == 2)
        {
            string quesito = QuestionDataScript.getCategory();
            answerTwoButton.GetComponent<Image>().color = Color.green;
            SoundManager.PlayAnswerSound(true);
            addCoin();
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), QuestionDataScript.getQuesito() ? quesito : "", !QuestionDataScript.getQuesito());
        }
        else
        {
            answerTwoButton.GetComponent<Image>().color = Color.red;
            SoundManager.PlayAnswerSound(false);
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), "", false);
        }
        StartCoroutine(WaitAndExit(3));
    }

    private void AnswerThreeButtonOnClick()
    {
        answersNonInteractable();
        answered = true;
        if (QuestionDataScript.getCorrectAnswer() == 3)
        {
            string quesito = QuestionDataScript.getCategory();
            answerThreeButton.GetComponent<Image>().color = Color.green;
            SoundManager.PlayAnswerSound(true);
            addCoin();
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), QuestionDataScript.getQuesito() ? quesito : "", !QuestionDataScript.getQuesito());
        }
        else
        {
            answerThreeButton.GetComponent<Image>().color = Color.red;
            SoundManager.PlayAnswerSound(false);
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), "", false);
        }
        StartCoroutine(WaitAndExit(3));
    }

    private void AnswerFourButtonOnClick()
    {
        answersNonInteractable();
        answered = true;
        if (QuestionDataScript.getCorrectAnswer() == 4)
        {
            string quesito = QuestionDataScript.getCategory();
            answerFourButton.GetComponent<Image>().color = Color.green;
            SoundManager.PlayAnswerSound(true);
            addCoin();
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), QuestionDataScript.getQuesito() ? quesito : "", !QuestionDataScript.getQuesito());
        }
        else
        {
            answerFourButton.GetComponent<Image>().color = Color.red;
            SoundManager.PlayAnswerSound(false);
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), "", false);
        }
        StartCoroutine(WaitAndExit(3));
    }

    private void addCoin()
    {
        StartCoroutine(AddCoinRequest(UserDataScript.getInfo("token")));
    }

    private void answersNonInteractable()
    {
        answerOneButton.interactable = false;
        answerTwoButton.interactable = false;
        answerThreeButton.interactable = false;
        answerFourButton.interactable = false;

    }

    IEnumerator WaitAndExit(int time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);
        
        SceneManager.UnloadSceneAsync("Question Scene");
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

    private IEnumerator AddCoinRequest(string token)
    {
        UnityWebRequest addCoinRequest = UnityWebRequest.Post("https://unitrivia.herokuapp.com/api/tienda/insertarMonedas", "");

        addCoinRequest.SetRequestHeader("jwt", token);
        addCoinRequest.SetRequestHeader("cantidad", "1");
        yield return addCoinRequest.SendWebRequest();
        Debug.Log("ResponseCode: " + addCoinRequest.responseCode);

        if (addCoinRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ERROR CONNECTION:" + addCoinRequest.result);

            ErrorDataScript.setErrorText("Error de conexión");
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else if (addCoinRequest.responseCode != 200)
        {
            Debug.Log("ERROR ADDCOIN:" + addCoinRequest.downloadHandler.text);
            ServerReturn result = ServerReturn.CreateFromJSON(addCoinRequest.downloadHandler.text);

            ErrorDataScript.setErrorText(result.message);
            ErrorDataScript.setButtonMode(1);
            SceneManager.LoadScene("Error Scene", LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("EXITO ADDCOIN:" + addCoinRequest.downloadHandler.text);
            UserDataScript.addCoins(1);
            Debug.Log("Insertada 1 moneda");
        }
    }
}
