using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LogDisplay : MonoBehaviour
{
    public Log logInfo;
    public LogHolder logHolder;
    private Sequence displaySequence;

    [Header("Visuals")]
    public Image icon;
    public Image background;
    public TextMeshProUGUI message;

    private void OnEnable()
    {
        CreateSequence();
    }

    private void OnDisable()
    {
        displaySequence.Kill();
    }

    public void DisplayLog(Color32 logColor)
    {
        icon.sprite = Resources.Load<Sprite>("CharacterIcons/" + logInfo.striker + "_icon");
        background.color = logColor;
        message.text = logInfo.message;

        displaySequence.Play();
    }

    private void CreateSequence()
    {
        displaySequence = DOTween.Sequence();

        displaySequence.Append(this.transform.DOLocalMoveX(0, 1f));
        displaySequence.AppendInterval(0.75f);
        displaySequence.AppendCallback(LogFinishing);
        displaySequence.AppendInterval(0.25f);
        displaySequence.Append(this.transform.DOLocalMoveY(150, 1f));
        displaySequence.Join(icon.DOFade(0f, 0.35f));
        displaySequence.Join(background.DOFade(0f, 0.35f));
        displaySequence.Join(message.DOFade(0f, 0.35f));

        displaySequence.onComplete = SequenceComplete;
        displaySequence.SetAutoKill();
        displaySequence.Pause();
    }

    private void LogFinishing()
    {
        logHolder.LogFinished();
    }

    private void SequenceComplete()
    {
        displaySequence.Kill();
        Destroy(this.gameObject);
    }
}
