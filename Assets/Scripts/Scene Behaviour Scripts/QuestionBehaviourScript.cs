using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    void Update()
    {

    }

    private void AnswerOneButtonOnClick()
    {
        if(QuestionDataScript.getCorrectAnswer() == 1)
        {
            string quesito = QuestionDataScript.getCategory();
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), QuestionDataScript.getQuesito()? quesito : "", !QuestionDataScript.getQuesito());
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
        }
        else
        {
            GameBehaviourScript.SendJugada(QuestionDataScript.getPosition(), "", false);
        }
        SceneManager.UnloadSceneAsync("Question Scene");
    }
}
