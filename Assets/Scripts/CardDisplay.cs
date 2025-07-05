using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CardDisplay : MonoBehaviour
{
    public string cardName;

    [Header("Display")]
    public Image m_picture;
    public Image m_card;
    public Image m_roleIcon;
    public Image m_rankIcon;
    
    public TextMeshProUGUI m_name;
    public Image[] m_characters;
    public Image[] m_statIcons;
    public TextMeshProUGUI m_healthStat;
    public TextMeshProUGUI m_powerStat;
    public TextMeshProUGUI m_speedStat;
    public TextMeshProUGUI m_actionStat;

    [Header("Time")]
    public Sequence sequence;
    public float revealTime;

    [Header("Colors")]
    public Color32 colorBlack;
    public Color32 colorWhite;
    public Color32 colorHidden;

    public void DisplayPlayerCard()
    {
        PlayerCard card = Resources.Load<PlayerCard>("PlayerCards/" + cardName);

        m_name.text = card.playerName;
        m_picture.sprite = card.picture;

        m_card.sprite = card.GetRankedCard(card.rank);
        m_roleIcon.sprite = card.GetRoleIcon(card.role);
        m_rankIcon.sprite = card.GetRankedIcon(card.rank);

        for (int i = 0; i < card.roster.Length; i++)
        {
            m_characters[i].gameObject.SetActive(true);
            m_characters[i].sprite = card.GetStrikerIcon(card.roster[i]);
        }

        if (card.roster.Length < 5)
        {
            for (int i = card.roster.Length; i < m_characters.Length; i++)
            {
                m_characters[i].gameObject.SetActive(false);
            }
        }

        m_healthStat.text = card.health.ToString();
        m_powerStat.text = card.power.ToString();
        m_speedStat.text = card.speed.ToString();
        m_actionStat.text = card.actionMetter.ToString();
    }

    [ContextMenu("Hide Card Details")]
    public void HideCardDetails()
    {
        m_picture.color = colorBlack;
        m_card.color = colorBlack;
        m_roleIcon.color = colorBlack;
        m_rankIcon.color = colorBlack;

        foreach(Image striker in m_characters)
        {
            striker.color = colorHidden;
        }
        
        foreach(Image icon in m_statIcons)
        {
            icon.color = colorHidden;
        }
    }

    [ContextMenu("Reveal Card Details")]
    public void RevealCardDetails()
    {
        this.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();

        sequence = DOTween.Sequence();

        sequence.Join(m_picture.DOColor(colorWhite, revealTime));
        sequence.Join(m_card.DOColor(colorWhite, revealTime));
        sequence.Join(m_roleIcon.DOColor(colorWhite, revealTime));
        sequence.Join(m_rankIcon.DOColor(colorWhite, revealTime));

        //m_picture.DOColor(colorWhite, revealTime);
        //m_card.DOColor(colorWhite, revealTime);
        //m_roleIcon.DOColor(colorWhite, revealTime);
        //m_rankIcon.DOColor(colorWhite, revealTime);

        foreach (Image striker in m_characters)
        {
            sequence.Join(striker.DOColor(colorWhite, revealTime));
        }

        foreach (Image icon in m_statIcons)
        {
            sequence.Join(icon.DOColor(colorWhite, revealTime));
        }

        sequence.Play();
    }

    public void KillSequence()
    {
        if (sequence != null)
            sequence.Kill();
    }
}
