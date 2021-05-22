using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class SuccessDataScript
{
    private static string successText = "void";
    private static string returnScene = "";

    public static void setSuccessText(string txt)
    {
        successText = txt;
    }

    public static string getSuccessText()
    {
        return successText;
    }

    public static void setReturnScene(string scene)
    {
        returnScene = scene;
    }

    public static string getReturnScene()
    {
        return returnScene;
    }
}
