using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionDataScript : MonoBehaviour
{
    private static int timer = 60;
    private static string question = "";
    private static string[] answers = null;
    private static int correctAnswer = 0;
    private static bool quesito = false;
    private static string category = "";
    private static int position = 0;

    public static void setQuestion(string q, List<string> Answers, int aCorrect, bool Quesito, string Category, int Position)
    {
        question = q;
        answers = Answers.ToArray();
        correctAnswer = aCorrect;
        quesito = Quesito;
        category = Category;
        position = Position;
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
        return answers;
    }

    public static bool getQuesito()
    {
        return quesito;
    }

    public static int getPosition()
    {
        return position;
    }

    public static string getCategory()
    {
        return category;
    }
}
