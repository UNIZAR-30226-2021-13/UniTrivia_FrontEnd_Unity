using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionDataScript : MonoBehaviour
{
    private static int timer = 60;
    private static string question = " ";
    private static string answerOne = " ";
    private static string answerTwo = " ";
    private static string answerThree = " ";
    private static string answerFour = " ";
    private static int correctAnswer = 0;

    public static void setQuestion(string q, string a1, string a2, string a3, string a4, int aCorrect)
    {
        question = q;
        answerOne = a1;
        answerTwo = a2;
        answerThree = a3;
        answerFour = a4;
        correctAnswer = aCorrect;
    }

    public static int getCorrectAnswer()
    {
        return correctAnswer;
    }

    public static int getTimer()
    {
        return timer;
    }

    public static string getQuestion()
    {
        return question;
    }

    public static string[] getAnswers()
    {
        string[] answers = { answerOne, answerTwo, answerThree, answerFour };
        return answers;
    }
}
