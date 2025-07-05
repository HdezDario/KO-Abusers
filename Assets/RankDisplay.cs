using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class RankDisplay : MonoBehaviour
{
    [SerializeField] private Image rankIcon;
    [SerializeField] private Image rankBackground;

    [SerializeField] private Sprite[] rankSprites;
    [SerializeField] private Color32[] rankColors;

    [SerializeField] private float rankIconTime;
    [SerializeField] private float rankBackgroundTime;

    public static UnityEvent RankDisplayFinished;

    private Sequence rankDisplaySequence;

    private void OnEnable()
    {
        if (RankDisplayFinished == null)
            RankDisplayFinished = new UnityEvent();

        rankIcon.sprite = rankSprites[(int)MatchData.current.rank + 1];
        rankBackground.color = rankColors[(int)MatchData.current.rank + 1];

        CreateRankDisplaySequence();

        StartCoroutine(SequenceCoroutine());
    }

    private void CreateRankDisplaySequence()
    {
        rankDisplaySequence = DOTween.Sequence();

        rankDisplaySequence.Append(rankIcon.transform.DOScale(0.2f, rankIconTime));
        rankDisplaySequence.Join(rankIcon.DOFade(0f, rankIconTime));

        rankDisplaySequence.Append(rankBackground.DOColor(rankColors[(int)MatchData.current.rank], rankBackgroundTime));

        rankDisplaySequence.AppendCallback(SwapRankIcon);

        rankDisplaySequence.Append(rankIcon.transform.DOScale(1.15f, rankIconTime));
        rankDisplaySequence.Join(rankIcon.DOFade(1f, rankIconTime));

        rankDisplaySequence.Append(rankIcon.transform.DOScale(1f, rankIconTime * 0.5f));

        rankDisplaySequence.Append(rankIcon.DOFade(0f, rankBackgroundTime));
        rankDisplaySequence.Append(rankBackground.DOFade(0f, rankBackgroundTime));

        rankDisplaySequence.onComplete = RankDisplayFinished.Invoke;
        rankDisplaySequence.SetAutoKill();
        rankDisplaySequence.Pause();
    }

    private IEnumerator SequenceCoroutine()
    {
        yield return new WaitForSeconds(1f);

        rankDisplaySequence.Play();
    }

    private void SwapRankIcon()
    {
        rankIcon.sprite = rankSprites[(int)MatchData.current.rank];
    }
}
