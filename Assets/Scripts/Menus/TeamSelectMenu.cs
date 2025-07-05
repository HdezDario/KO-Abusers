using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectMenu : Menu
{
    [SerializeField] private CardDisplay goalieCard;
    [SerializeField] private CardDisplay midfielderCard;
    [SerializeField] private CardDisplay forwardCard;

    private Button goalieButton;
    private Button midfielderButton;
    private Button forwardButton;

    [Header("Inventory")]
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject inventoryHolder;
    [SerializeField] private CardDisplay inventoryCard;

    private void OnEnable()
    {
        goalieButton = goalieCard.GetComponent<Button>();
        midfielderButton = midfielderCard.GetComponent<Button>();
        forwardButton = forwardCard.GetComponent<Button>();

        goalieButton.onClick.AddListener(delegate { PopulateTeamSelectInventory(SaveData.current.unlockedGoalies, Role.Goalie); });
        midfielderButton.onClick.AddListener(delegate { PopulateTeamSelectInventory(SaveData.current.unlockedMidfielders, Role.Midfielder); });
        forwardButton.onClick.AddListener(delegate { PopulateTeamSelectInventory(SaveData.current.unlockedForwards, Role.Forward); });

        LoadDisplayCards();
    }

    private void PopulateTeamSelectInventory(List<string> playerList, Role role)
    {
        if (inventory.transform.childCount > 0)
            for (int i = inventory.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(inventory.transform.GetChild(i).gameObject);
            }

        foreach(string player in playerList)
        {
            CardDisplay playerDisplay = Instantiate(inventoryCard, inventory.transform).GetComponent<CardDisplay>(); // playerCard.GetComponent<CardDisplay>();
            playerDisplay.cardName = player;
            playerDisplay.DisplayPlayerCard();

            Button playerButton = playerDisplay.GetComponent<Button>();
            playerButton.onClick.AddListener(delegate { SetTeamMember(player, role); });
        }

        inventoryHolder.SetActive(true);
    }

    private void LoadDisplayCards()
    {
        goalieCard.cardName = SaveData.current.goalie;
        midfielderCard.cardName = SaveData.current.midfielder;
        forwardCard.cardName = SaveData.current.forward;

        goalieCard.DisplayPlayerCard();
        midfielderCard.DisplayPlayerCard();
        forwardCard.DisplayPlayerCard();
    }

    public override void SetTeamMember(string name, Role role)
    {
        base.SetTeamMember(name, role);

        LoadDisplayCards();
        CloseInventoryHolder();
    }

    public void CloseInventoryHolder()
    {
        inventoryHolder.SetActive(false);
    }
}
