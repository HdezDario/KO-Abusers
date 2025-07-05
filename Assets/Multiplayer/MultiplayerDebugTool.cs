using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplayerDebugTool : MonoBehaviour
{
    [SerializeField] private MultiplayerMatchManager mm;
    [SerializeField] private TextMeshProUGUI debugText;
    
    void Update()
    {
        debugText.text =
            "Turn: " + mm.turn.Value + "\n" +
            "HighestActionMetter: " + mm.highestActionMetter.Value + "\n" +
            "ActionAchieved: " + mm.isActionAchieved.Value + "\n" +
            "GameFinished: " + mm.isGameFinished.Value + "\n";
    }
}
