using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RequestServerHandler
{
    //En construcción. Sin uso.
    //Class for JSON deserializing
    [System.Serializable]
    public class ErrorReturn
    {
        public int code;
        public string message;

        public static ErrorReturn CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ErrorReturn>(jsonString);
        }
    }

    //Class for JSON deserializing
    [System.Serializable]
    public class ProfileReturn
    {
        public string preg;
        public string res;
        public int cns;
        public int ng;
        public string fich;
        public string bnr;
        public string[] rfs;
        public string avtr;
        public int nj;
        public string mail;
        public string _id;

        public static ProfileReturn CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ProfileReturn>(jsonString);
        }
    }

}
