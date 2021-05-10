using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersDataScript
{
    public class Jugador
    {
        public readonly string nombre;
        public readonly string banner;
        public readonly string avatar;
        public readonly string ficha;
        public List<string> quesitos;
        public int posicion { get; set; }

        public Jugador(string nombre, string banner, string avatar, string ficha, int posicion, string[] quesitos)
        {
            this.nombre = nombre;
            this.banner = banner;
            this.avatar = avatar;
            this.ficha = ficha;
            this.posicion = posicion;
            this.quesitos = new List<string>(quesitos);
        }
    }

    public static readonly List<Jugador> jugadores = new List<Jugador>();
    public static void nuevoJugador(Jugador nuevo)
    {
        jugadores.Add(nuevo);
    }
    public static int index(string nombre)
    {
        int i = -1; bool found = false;
        while (!found && i < jugadores.Count)
            found = jugadores[++i].nombre == nombre;

        return found ? i : -1;
    }
    public static void eliminarJugador(string nombre)
    {
        int i = 0;
        jugadores.RemoveAt(index(nombre));
    }

}
