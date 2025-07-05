using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log
{
    public Strikers striker;
    public string message;
    public int team;

    public Log (Strikers stkr, string text, int teamNumber)
    {
        striker = stkr;
        message = text;
        team    = teamNumber;
    }
}
