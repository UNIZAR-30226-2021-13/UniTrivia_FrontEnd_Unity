using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ConfirmDataScript
{
    private static string confirmText = "Void";

    public static void setConfirmText(string txt)
    {
        confirmText = txt;
    }

    public static string getConfirmText()
    {
        return confirmText;
    }
}
