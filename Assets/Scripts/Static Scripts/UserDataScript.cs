using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class UserDataScript
{
    private static int coins = 0;
    private static int played = 0;
    private static int wins = 0;
    private static string[] itemsPlayer = new string[25];
    private static int itemsAdquired = 0;

    private static Dictionary<string, string> userInfo = new Dictionary<string, string>();

    //Player general information
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

    //Player coins
    public static void setCoins(int cns)
    {
        coins = cns;
    }

    public static void addCoins(int n)
    {
        coins += n;
    }

    public static void removeCoins(int n)
    {
        coins -= n;
    }

    public static int getCoins()
    {
        return coins;
    }

    //Player stats
    public static void setStats(int plyd, int wns)
    {
        played = plyd;
        wins = wns;
    }

    public static int getPlayed()
    {
        return played;
    }

    public static void addPlayed(bool win)
    {
        played += 1;

        if (win)
        {
            wins += 1;
        }
    }

    public static int getWins()
    {
        return wins;
    }

    //Player items adquired
    public static void setItems(string[] itms)
    {
        itemsAdquired = 0;
        //Debug.Log("ITEMS.LENGTH = " + itms.Length);
        for (int i = 0; i < itms.Length; i++)
        {
            //Debug.Log("PROFILEITEMS + " + itms[i]);
            itemsPlayer[i] = itms[i];
            itemsAdquired++;
        }
    }

    public static void addItem(string newItem)
    {
        itemsPlayer[itemsAdquired] = newItem;
        itemsAdquired++;
    }

    public static bool isItem(string name)
    {
        for(int i = 0; i < itemsAdquired; i++)
        {
            Debug.Log("ISITEM + " + itemsPlayer[i] + " < " + itemsAdquired + " | " + name);
            if (itemsPlayer[i].Equals(name))
            {
                return true;
            }
        }
        
        return false;
    }

    //No more player
    public static void deleteInfo()
    {
        userInfo = new Dictionary<string, string>();
        coins = 0;
        played = 0;
        wins = 0;
        itemsPlayer = new string[25];
    }
}
