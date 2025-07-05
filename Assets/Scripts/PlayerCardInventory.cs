using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCardInventory : MonoBehaviour
{
    [Header("Testing")]
    [SerializeField] private string playerName;

    [Header("Display")]
    [SerializeField] private CardDisplay displayCard;
    [SerializeField] private GameObject displayCardHolder;

    [Header("Inventory")]
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject inventoryCard;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private TextMeshProUGUI collectedText;
    
    private void OnEnable()
    {
        PopulateInventory(SaveData.current.unlockedPlayerCards);
    }

    public void PopulateInventory(List<string> playerList)
    {
        PlayerCard[] allPlayerList = Resources.LoadAll<PlayerCard>("PlayerCards");

        if (inventory.transform.childCount > 0)
            for (int i = inventory.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(inventory.transform.GetChild(i).gameObject);
            }

        foreach (string player in playerList)
        {
            CardDisplay playerDisplay = Instantiate(inventoryCard, inventory.transform).GetComponent<CardDisplay>(); // playerCard.GetComponent<CardDisplay>();
            playerDisplay.cardName = player;
            playerDisplay.DisplayPlayerCard();

            Button playerButton = playerDisplay.GetComponent<Button>();
            playerButton.onClick.AddListener(delegate { ShowCard(player); });
        }

        collectedText.text = "You have collected <color=green>" + playerList.Count + "</color> out of " + (allPlayerList.Length /*- 3*/) + " players";
    }

    public void ShowCard(string name)
    {
        displayCard.cardName = name;
        displayCard.DisplayPlayerCard();

        displayCardHolder.SetActive(true);
    }

    public void CloseCard()
    {
        displayCardHolder.SetActive(false);
    }

    [ContextMenu("Add Player")]
    private void AddPlayerToInventory()
    {
        CardDisplay playerDisplay = Instantiate(inventoryCard, inventory.transform).GetComponent<CardDisplay>();
        playerDisplay.cardName = playerName;
        playerDisplay.DisplayPlayerCard();
    }
}
