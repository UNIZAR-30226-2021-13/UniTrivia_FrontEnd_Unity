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
        SceneManager.UnloadSceneAsync("Question Scene");
    }

    private void AnswerTwoButtonOnClick()
    {
        SceneManager.UnloadSceneAsync("Question Scene");
    }

    private void AnswerThreeButtonOnClick()
    {
        SceneManager.UnloadSceneAsync("Question Scene");
    }

    private void AnswerFourButtonOnClick()
    {
        SceneManager.UnloadSceneAsync("Question Scene");
    }
}
