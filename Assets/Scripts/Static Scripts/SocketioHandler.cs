using System.Collections.Generic;
using System;
using UnityEngine;
using Socket.Quobject.SocketIoClientDotNet.Client;

public static class SocketioHandler {
    public static string op;
    private static Dictionary<string, string> args;
    private const string ENDPOINT = "http://unitrivia.herokuapp.com/api/partida";

    //Socketio Socket
    public static QSocket socket = null;

    public static void Init(string operacion, Dictionary<string, string> argumentos)
    {
        op = operacion;
        args = argumentos;
        socket = null;
    }

    public static void AddHandler(string evento, Action<object> handler)
    {
        if(socket != null)
        {
            socket.On(evento, handler);
        }
    }

    public static bool Start(Action fnConexion, Action fnEnd, Dictionary<string, Action<object>> handlers)
    {
        if(handlers.Count == 0)
        {
            return false;
        }

        Debug.Log("Comienza initSocketio()");
        try
        {
            IO.Options opciones = new IO.Options();
            opciones.ExtraHeaders.Add("jwt", UserDataScript.getInfo("token"));
            opciones.ExtraHeaders.Add("operacion", op);

            foreach (KeyValuePair<string, string> entry in args)
            {
                opciones.ExtraHeaders.Add(entry.Key, entry.Value);
            }

            socket = IO.Socket(ENDPOINT, opciones);

            foreach (KeyValuePair<string, Action<object>> entry in handlers)
            {
                socket.On(entry.Key, entry.Value);
            }

            socket.On(QSocket.EVENT_DISCONNECT, (reason) => { Debug.Log("Disconnected: " + reason + " \n" + Environment.StackTrace); PlayersDataScript.eliminarPartida();  fnEnd();  });
            socket.On(QSocket.EVENT_RECONNECT, () => { Debug.Log("Reconnected"); });
            socket.On(QSocket.EVENT_CONNECT, () => { Debug.Log("Connected"); fnConexion(); });
        }catch (Exception e) { 
            Debug.Log("Exception: " + e); 
        }
        return true;
    }

    public static void End()
    {
        socket.Disconnect();
        op = "";
        args.Clear();
        socket = null;
    }

}