using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ErrorDataScript
{
    private static string errorText = "Void";
    private static int buttonMode = 1;  //0=Exit, 1=Accept

    public static void setErrorText(string txt)
    {
        errorText = txt;
    }

    public static string getErrorText()
    {
        return errorText;
    }

    public static void setButtonMode(int mode)
    {
        buttonMode = mode;
    }

    public static int getButtonMode()
    {
        return buttonMode;
    }
}
