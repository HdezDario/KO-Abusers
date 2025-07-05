using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerLogHolder : NetworkBehaviour
{
    public List<Log> logList;
    public GameObject logPrefab;
    public bool isLogDisplaying;
    public Color32 holderColor;

    private void OnEnable()
    {
        logList = new List<Log>();
    }

    [ClientRpc]
    public void AddLogClientRpc(Strikers striker, string message, int team)
    {
        logList.Add(new Log(striker, message, team));
        DisplayNextLog();
    }

    private void DisplayNextLog()
    {
        if (!isLogDisplaying && logList.Count >= 1)
        {
            isLogDisplaying = true;

            GameObject nextLog = Instantiate(logPrefab);
            nextLog.transform.SetParent(this.transform, false);

            MultiplayerLogDisplay nextLogDisplay = nextLog.GetComponent<MultiplayerLogDisplay>();

            nextLogDisplay.logInfo = logList[0];
            nextLogDisplay.logHolder = this;
            nextLogDisplay.DisplayLog(holderColor);

            logList.RemoveAt(0);
        }
    }

    [ServerRpc]
    private void DisplayNextLogServerRpc()
    {
        if (!isLogDisplaying && logList.Count >= 1)
        {
            isLogDisplaying = true;

            GameObject nextLog = Instantiate(logPrefab);
            nextLog.transform.SetParent(this.transform, false);
            nextLog.GetComponent<NetworkObject>().Spawn(true);
            
            MultiplayerLogDisplay nextLogDisplay = nextLog.GetComponent<MultiplayerLogDisplay>();

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
