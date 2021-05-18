using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class QuestionBehaviourScript : MonoBehaviour
{
    public Text timerText;
    public Text questionText;
    public Button answerOneButton;
    public Button answerTwoButton;
    public Button answerThreeButton;
    public Button answerFourButton;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = QuestionDataScript.getTimer().ToString();
        questionText.text = QuestionDataScript.getQuestion();

        string[] answers = QuestionDataScript.getAnswers();
        answerOneButton.GetComponentInChildren<Text>().text = answers[0];
        answerTwoButton.GetComponentInChildren<Text>().text = answers[1];
        answerThreeButton.GetComponentInChildren<Text>().text = answers[2];
        answerFourButton.GetComponentInChildren<Text>().text = answers[3];

        answerOneButton.onClick.AddListener(AnswerOneButtonOnClick);
        answerTwoButton.onClick.AddListener(AnswerTwoButtonOnClick);
        answerThreeButton.onClick.AddListener(AnswerThreeButtonOnClick);
        answerFourButton.onClick.AddListener(AnswerFourButtonOnClick);
    }

    private void AnswerOneButtonOnClick()
    {
        if(QuestionDataScript.getCorrectAnswer() == 1)
        {
            string quesito = QuestionDataScript.getCategory();
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), QuestionDataScript.getQuesito()? quesito : "", !QuestionDataScript.getQuesito());
            addCoin();
        }
        else
        {
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(),"",false);
        }
        SceneManager.UnloadSceneAsync("Question Scene");
    }

    private void AnswerTwoButtonOnClick()
    {
        if (QuestionDataScript.getCorrectAnswer() == 2)
        {
            string quesito = QuestionDataScript.getCategory();
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), QuestionDataScript.getQuesito() ? quesito : "", !QuestionDataScript.getQuesito());
            addCoin();
        }
        else
        {
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), "", false);
        }
        SceneManager.UnloadSceneAsync("Question Scene");
    }

    private void AnswerThreeButtonOnClick()
    {
        if (QuestionDataScript.getCorrectAnswer() == 3)
        {
            string quesito = QuestionDataScript.getCategory();
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), QuestionDataScript.getQuesito() ? quesito : "", !QuestionDataScript.getQuesito());
            addCoin();
        }
        else
        {
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), "", false);
        }
        SceneManager.UnloadSceneAsync("Question Scene");
    }

    private void AnswerFourButtonOnClick()
    {
        if (QuestionDataScript.getCorrectAnswer() == 4)
        {
            string quesito = QuestionDataScript.getCategory();
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), QuestionDataScript.getQuesito() ? quesito : "", !QuestionDataScript.getQuesito());
            addCoin();
        }
        else
        {
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), "", false);
        }
        SceneManager.UnloadSceneAsync("Question Scene");
    }

    private void addCoin()
    {
        StartCoroutine(AddCoinRequest(UserDataScript.getInfo("token")));
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
