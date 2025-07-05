using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogHolder : MonoBehaviour
{
    public List<Log> logList;
    public GameObject logPrefab;
    public bool isLogDisplaying;
    public Color32 holderColor;

    private void OnEnable()
    {
        logList = new List<Log>();
    }

    public void AddLog(Strikers striker, string message, int team)
    {
        logList.Add(new Log(striker, message, team));
        DisplayNextLog();
    }

    private void DisplayNextLog()
    {
        if (!isLogDisplaying && logList.Count >= 1)
        {
            isLogDisplaying = true;

            GameObject nextLog = Instantiate(logPrefab, this.transform);
            LogDisplay nextLogDisplay = nextLog.GetComponent<LogDisplay>();

            //Debug.Log(logList.Count);
            //Debug.Log(nextLogDisplay);

            nextLogDisplay.logInfo = logList[0];
            nextLogDisplay.logHolder = this;
            nextLogDisplay.DisplayLog(holderColor);

            logList.RemoveAt(0);
        }
    }

    public void LogFinished()
    {
        isLogDisplaying = false;
        DisplayNextLog();
    }
}
