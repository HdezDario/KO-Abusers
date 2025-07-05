using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardPlayerItemDisplay : MonoBehaviour
{
    [SerializeField] private Image playerIcon;
    [SerializeField] private Image rankIcon;
    [SerializeField] private Image strikerIcon;
    [SerializeField] private TextMeshProUGUI nameText;

    public void Display(string playerName, Strikers striker)
    {
        PlayerCard card = Resources.Load<PlayerCard>("PlayerCards/" + playerName);

        nameText.text = playerName;
        playerIcon.sprite = card.picture;
        rankIcon.sprite = GetRankedIcon(card.rank);
        strikerIcon.sprite = GetStrikerIcon(striker);
    }

    private Sprite GetRankedIcon(Rank rank)
    {
        return Resources.Load<Sprite>("RankedIcons/" + rank + "_icon");
    }

    private Sprite GetStrikerIcon(Strikers striker)
    {
        return Resources.Load<Sprite>("CharacterIcons/" + striker + "_icon");
    }
}
