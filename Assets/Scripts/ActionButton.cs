using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Button button;

    [Header("Information")]
    [SerializeField] private Library library;
    [SerializeField] private object[] actionInformation;
    [SerializeField] private int characterIndex;
    [SerializeField] private ActionType actionType;
    [SerializeField] private Skill actionSkill;

    [Header("Display")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private Image colorBanner;
    [SerializeField] private Image bannerBorder;
    [SerializeField] private Image characterIcon;
    
    public UnityAction action;
    public GameObject descriptionBanner;

    private void Start()
    {
        library = new Library();
        library = JsonUtility.FromJson<Library>(Resources.Load<TextAsset>("Skills/SkillsInformation").text);

        //object[] twewe = { "Marcos" , 2};
        //string str = string.Format("Hola {0}, tengo {1} manzanas", twewe);

        //Debug.Log(str);
    }

    public void AssignStrikerAction(Strikers character, UnityAction action, Skill skill, ActionType type, object[] information)
    {
        button.onClick.RemoveAllListeners();

        actionInformation = new object[information.Length];
        information.CopyTo(actionInformation, 0);

        characterIndex = (int)character;
        actionType = type;
        actionSkill = skill;

        nameText.color = library.characterInfo[characterIndex].color;
        bannerBorder.color = library.characterInfo[characterIndex].color;
        colorBanner.color = library.characterInfo[characterIndex].color;
        characterIcon.sprite = Resources.Load<Sprite>("CharacterIcons/" + character + "_icon");

        button.interactable = skill.currentCooldown <= 0 ? true : false;
        
        if (button.interactable)
        {
            button.onClick.AddListener(action);
        }
    }

    public void UnassignAction()
    {
        button.onClick.RemoveAllListeners();
        button.interactable = false;
    }

    private void PopulateSkillInfo(int index, ActionType type, Skill skill)
    { 
        switch (type)
        {
            case (ActionType.Evade):
                nameText.text           = library.evade.name;
                explanationText.text    = library.evade.explanation;
                break;
            case (ActionType.Primary):
                nameText.text           = library.characterInfo[index].primary.name;
                explanationText.text    = string.Format(library.characterInfo[index].primary.explanation, actionInformation);
                break;
            case (ActionType.Secondary):
                nameText.text           = library.characterInfo[index].secondary.name;
                explanationText.text    = string.Format(library.characterInfo[index].secondary.explanation, actionInformation);
                break;
            case (ActionType.Ultimate):
                nameText.text           = library.characterInfo[index].ultimate.name;
                explanationText.text    = string.Format(library.characterInfo[index].ultimate.explanation, actionInformation);
                break;
            case (ActionType.Strike):
                nameText.text           = library.characterInfo[index].strike.name;
                explanationText.text    = string.Format(library.characterInfo[index].strike.explanation, actionInformation);
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

        if (skill.endsTurn)
            explanationText.text += "\n· Ends the turn";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            PopulateSkillInfo(characterIndex, actionType, actionSkill);
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
