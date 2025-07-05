using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;

public class MultiplayerActionButton : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Button button;

    [Header("Information")]
    [SerializeField] private Library library;
    //[SerializeField] private object[] actionInformation;
    [SerializeField] private int characterIndex;
    [SerializeField] private ActionType actionType;
    [SerializeField] private bool skillEndsTurn;

    [Header("Display")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private Image colorBanner;
    [SerializeField] private Image bannerBorder;
    [SerializeField] private Image characterIcon;
    
    public GameObject descriptionBanner;

    [SerializeField] private MultiplayerMatchManager mm;

    private void Start()
    {
        library = new Library();
        library = JsonUtility.FromJson<Library>(Resources.Load<TextAsset>("Skills/SkillsInformation").text);
    }

    [ClientRpc]
    public void AssignStrikerActionClientRpc(Strikers character, ActionType type, bool endsTurn)
    {
        characterIndex = (int)character;
        actionType = type;
        skillEndsTurn = endsTurn;

        nameText.color = library.characterInfo[characterIndex].color;
        bannerBorder.color = library.characterInfo[characterIndex].color;
        colorBanner.color = library.characterInfo[characterIndex].color;
        characterIcon.sprite = Resources.Load<Sprite>("CharacterIcons/" + character + "_icon");
    }

    public void EnableActionButton()
    {
        button.interactable = true;
    }

    public void DisableActionButton()
    {
        button.interactable = false;
    }

    private void PopulateSkillInfo(int index, ActionType type, bool endsTurn)
    {
        object[] info;

        switch (type)
        {
            case (ActionType.Evade):
                nameText.text           = library.evade.name;
                explanationText.text    = library.evade.explanation;
                break;
            case (ActionType.Primary):

                info = new object[mm.primaryInfo.Count];
                for (int i = 0; i < info.Length; i++)
                    info[i] = mm.primaryInfo[i];

                nameText.text           = library.characterInfo[index].primary.name;
                explanationText.text    = string.Format(library.characterInfo[index].primary.explanation, info);
                break;
            case (ActionType.Secondary):
                
                info = new object[mm.secondaryInfo.Count];
                for (int i = 0; i < info.Length; i++)
                    info[i] = mm.secondaryInfo[i];

                nameText.text           = library.characterInfo[index].secondary.name;
                explanationText.text    = string.Format(library.characterInfo[index].secondary.explanation, info);
                break;
            case (ActionType.Ultimate):

                info = new object[mm.ultimateInfo.Count];
                for (int i = 0; i < info.Length; i++)
                    info[i] = mm.ultimateInfo[i];

                nameText.text           = library.characterInfo[index].ultimate.name;
                explanationText.text    = string.Format(library.characterInfo[index].ultimate.explanation, info);
                break;
            case (ActionType.Strike):

                info = new object[mm.strikeInfo.Count];
                for (int i = 0; i < info.Length; i++)
                    info[i] = mm.strikeInfo[i];

                nameText.text           = library.characterInfo[index].strike.name;
                explanationText.text    = string.Format(library.characterInfo[index].strike.explanation, info);
                break;
            case (ActionType.MoveLeft):
                nameText.text           = library.moveLeft.name;
                explanationText.text    = library.moveLeft.explanation;
                break;
            case (ActionType.MoveRight):
                nameText.text           = library.moveRight.name;
                explanationText.text    = library.moveRight.explanation;
                break;
        }

        if (endsTurn)
            explanationText.text += "\n· Ends the turn";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            PopulateSkillInfo(characterIndex, actionType, skillEndsTurn);
            descriptionBanner.SetActive(true);
        }
        else
            descriptionBanner.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionBanner.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        descriptionBanner.SetActive(false);
    }

    [System.Serializable]
    private class Library
    {
        public SkillExplanation evade;
        public SkillExplanation moveLeft;
        public SkillExplanation moveRight;
        public CharacterInfo[] characterInfo;
    }

    [System.Serializable]
    private class CharacterInfo
    {
        public string name;
        public Color32 color;
        public SkillExplanation primary;
        public SkillExplanation secondary;
        public SkillExplanation ultimate;
        public SkillExplanation strike;
    }

    [System.Serializable]
    private class SkillExplanation
    {
        public string name;
        public string explanation;
    }
}
