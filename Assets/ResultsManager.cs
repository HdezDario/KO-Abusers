using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Economy;

public class ResultsManager : Menu
{
    public Image rankIcon;
    public Image rewardIcon;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI resultText;

    private Sequence resultsSequence;

    private void OnEnable()
    {
        if (MatchData.current.rank == Rank.Rookie)
        {
            MatchData.current.rank = Rank.ProLeague;
            resultText.text = "You Qualified for EUSL";
        }
        else
        {
            resultText.text = "You Lost";
        }

        rankIcon.sprite = Resources.Load<Sprite>("RankedIcons/" + MatchData.current.rank + "_icon");
        rewardText.text = MatchData.current.reward.ToString();

        resultsSequence = DOTween.Sequence();

        resultsSequence.AppendInterval(1.2f);
        resultsSequence.Append(rankIcon.DOFade(1f, 1f));
        resultsSequence.AppendInterval(1f);
        resultsSequence.Append(rewardText.DOFade(1f, 1f));
        resultsSequence.Join(rewardIcon.DOFade(1f, 1f));

        resultsSequence.SetAutoKill();
    }

    public void BackToMenu()
    {
        //SaveData.current.credits += MatchData.current.reward;
        //SaveGame();

        //await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("CREDITS", MatchData.current.reward);

        SceneManager.LoadScene((int)Scenes.Menu);
    }
}
