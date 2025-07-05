using System.Collections;
using System.Collections.Generic;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardItemDisplay : MonoBehaviour
{
    [SerializeField] private Metadata info;

    [Header("Display")]
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image rankIcon;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Player Items")]
    [SerializeField] private LeaderboardPlayerItemDisplay glItem;
    [SerializeField] private LeaderboardPlayerItemDisplay mfItem;
    [SerializeField] private LeaderboardPlayerItemDisplay fwItem;

    public void ScoreDisplay(LeaderboardEntry score)
    {
        info = new Metadata();
        info = JsonUtility.FromJson<Metadata>(score.Metadata);

        rankText.text = (score.Rank + 1).ToString();
        rankIcon.sprite = GetRankedIcon(score.Tier);
        pointsText.text = score.Score.ToString();        
        nameText.text = info.name;

        glItem.Display(info.goalie, info.goalieStriker);
        mfItem.Display(info.midfielder, info.midfielderStriker);
        fwItem.Display(info.forward, info.forwardStriker);
    }

    [System.Serializable]
    private class Metadata
    {
        public string name;
        public string goalie;
        public string midfielder;
        public string forward;
        public Strikers goalieStriker;
        public Strikers midfielderStriker;
        public Strikers forwardStriker;
    }

    private Sprite GetRankedIcon(string rank)
    {
        return Resources.Load<Sprite>("RankedIcons/" + rank + "_icon");
    }
}
