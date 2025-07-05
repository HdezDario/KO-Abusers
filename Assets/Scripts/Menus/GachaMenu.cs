using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Economy.Model;
using Unity.Services.Economy;
using System.Threading.Tasks;

public class GachaMenu : Menu
{
    private const string CREDITS = "CREDITS";
    private PlayerCard[] playerCards;
    [SerializeField] private List<PlayerCard> promoCards;
    public int ammountOfCards;

    [Header("Player Balance")]
    [SerializeField] private PlayerBalance playerBalance;

    [Header("Rates")]
    [SerializeField] private float[] rareRate;
    [SerializeField] private float[] legendaryRate;
    [SerializeField] private float godRate;

    [Header("Gacha")]
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private int gachaCost;
    [SerializeField] private CardDisplay[] gachaPullCards;
    [SerializeField] private GameObject gachaHolder;
    [SerializeField] private TextMeshProUGUI credits;
    [SerializeField] private int cardsOpened;

    [Header("Refunds")]
    [SerializeField] private int refundCommon;
    [SerializeField] private int refundRare;
    [SerializeField] private int refundLegendary;

    [Header("Ranked Player Lists")]
    [SerializeField] private List<string> legendaryCards;
    [SerializeField] private List<string> rareCards;
    [SerializeField] private List<string> commonCards;

    void Start()
    {
        PopulatePlayerLists();
    }

    private void OnEnable()
    {
        InitializeCurrency();
    }

    private async void InitializeCurrency()
    {
        playerBalance = await GetPlayerBalance();

        if (playerBalance.Balance < gachaCost)
            gachaButton.interactable = false;
        else
            gachaButton.interactable = true;

        gachaHolder.SetActive(false);

        credits.text = "Your credits: <color=yellow>" + playerBalance.Balance + "</color>";
    }

    private async Task<PlayerBalance> GetPlayerBalance()
    {
        try
        {
            GetBalancesResult result = await EconomyService.Instance.PlayerBalances.GetBalancesAsync();
            Debug.Log("Got Player currency");
            return result.Balances[0];
        }
        catch
        {
            Debug.Log("Couldnt get currency");
            return default;
        }
    }

    private void PopulatePlayerLists()
    {
        playerCards = Resources.LoadAll<PlayerCard>("PlayerCards");

        foreach(PlayerCard player in playerCards)
        {
            if (!promoCards.Contains(player))
            {
                if (player.rank == Rank.Bronze ||
                player.rank == Rank.Silver ||
                player.rank == Rank.Gold)
                    commonCards.Add(player.playerName);

                else if (player.rank == Rank.Platinum ||
                    player.rank == Rank.Diamond ||
                    player.rank == Rank.Challenger)
                    rareCards.Add(player.playerName);

                else if (player.rank == Rank.Omega ||
                    player.rank == Rank.ProLeague)
                    legendaryCards.Add(player.playerName);
            }
        }
    }

    public async void GachaAPlayer()
    {
        await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync(CREDITS, gachaCost);

        float chance; int index;

        chance = Random.Range(0f, 100f);

        if (chance <= godRate)
        {
            Debug.Log("God Pack");

            for (int i = 0; i < ammountOfCards; i++)
            {
                index = Random.Range(0, legendaryCards.Count);

                if (!SaveData.current.unlockedPlayerCards.Contains(legendaryCards[index]))
                    UnlockPlayerCard(legendaryCards[index]);
                else
                    await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(CREDITS, refundLegendary);

                gachaPullCards[i].cardName = legendaryCards[index];
            }
        }

        else
        {
            Debug.Log("Normal Pack");

            for (int i = 0; i < ammountOfCards; i++)
            {
                chance = Random.Range(0f, 100f);

                if (chance < legendaryRate[i])
                {
                    index = Random.Range(0, legendaryCards.Count);

                    if (!SaveData.current.unlockedPlayerCards.Contains(legendaryCards[index]))
                        UnlockPlayerCard(legendaryCards[index]);
                    else
                        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(CREDITS, refundLegendary);

                    gachaPullCards[i].cardName = legendaryCards[index];
                }

                else if (chance < rareRate[i])
                {
                    index = Random.Range(0, rareCards.Count);

                    if (!SaveData.current.unlockedPlayerCards.Contains(rareCards[index]))
                        UnlockPlayerCard(rareCards[index]);
                    else
                        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(CREDITS, refundRare);

                    gachaPullCards[i].cardName = rareCards[index];
                }

                else
                {
                    index = Random.Range(0, commonCards.Count);

                    if (!SaveData.current.unlockedPlayerCards.Contains(commonCards[index]))
                        UnlockPlayerCard(commonCards[index]);
                    else
                        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(CREDITS, refundCommon);

                    gachaPullCards[i].cardName = commonCards[index];
                }
            }
        }

        cardsOpened = 0;

        foreach(CardDisplay card in gachaPullCards)
        {
            card.KillSequence();
            card.HideCardDetails();
            card.DisplayPlayerCard();
            card.GetComponent<Button>().onClick.AddListener(card.RevealCardDetails);
            card.GetComponent<Button>().onClick.AddListener(CardRevealed);
        }

        closeButton.interactable = false;
        gachaButton.interactable = false;

        gachaHolder.SetActive(true);

        SaveGame();
        playerBalance = await GetPlayerBalance();

        credits.text = "Your credits: <color=yellow>" + playerBalance.Balance + "</color>";
    }

    public async void CardRevealed()
    {
        cardsOpened++;

        if (cardsOpened > 2)
        {
            closeButton.interactable = true;
            playerBalance = await GetPlayerBalance();

            if (playerBalance.Balance > gachaCost)
                gachaButton.interactable = true;
        }
    }
}
