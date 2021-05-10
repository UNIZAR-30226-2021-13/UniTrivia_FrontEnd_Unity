using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class UserDataScript
{
    private static int coins = 0;
    private static int played = 0;
    private static int wins = 0;
    private static string[] items;

    private static Dictionary<string, string> userInfo = new Dictionary<string, string>();

    public static void addCoins(int n)
    {
        coins += n;
    }

    public static void addPlayed(bool win)
    {
        played += 1;

        if(win)
        {
            wins += 1;
        }
    }

    public static void setInfo(string key, string valor)
    {
        if (userInfo.ContainsKey(key))
        {
            userInfo.Remove(key);
        }

        userInfo.Add(key, valor);
    }

    public static string getInfo(string key)
    {
        return userInfo[key];
    }

    public static void setCoins(int cns)
    {
        coins = cns;
    }

    public static int getCoins()
    {
        return coins;
    }

    public static void setStats(int plyd, int wns)
    {
        played = plyd;
        wins = wns;
    }

    public static int getPlayed()
    {
        return played;
    }

    public static int getWins()
    {
        return wins;
    }

    public static void setItems(string[] itms)
    {
        items = itms;
    }

    public static bool isItem(string name)
    {
        if(items.Length > 0)
        {
            foreach (string s in items)
            {
                if (s.Equals(name))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public static void deleteInfo()
    {
        userInfo = new Dictionary<string, string>();
        coins = 0;
        played = 0;
        wins = 0;
    }
}
