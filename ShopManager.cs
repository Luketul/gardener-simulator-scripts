using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TMP_Text CoinsText;
    public GameObject ConfirmPanel;
    public TMP_Text ConfirmText;
    public Button ConfirmYes;
    public Button ConfirmNo;

    public Button[] itemButtons; // Przechowujemy przyciski przedmiotów

    private int coins;
    private int currentProfile;
    private string selectedItem;
    private int selectedItemPrice;
    private Button selectedButton;

    public AudioSource Buy;

    private void Start()
    {
        // Pobieramy aktualny profil gracza
        currentProfile = PlayerPrefs.GetInt("CurrentProfile", 1);

        // £adujemy monety dla tego profilu
        coins = PlayerPrefs.GetInt("Coins_" + currentProfile, 30);
        UpdateCoinsUI();
        ConfirmPanel.SetActive(false);

        // Sprawdzamy, które przedmioty zosta³y kupione dla tego profilu
        UpdateShopButtons();
    }

    public void TryBuyItem(string itemName, int price, Button itemButton)
    {
        Debug.Log($"[Profil {currentProfile}] Próba zakupu: {itemName} za {price} monet.");

        if (coins >= price && !IsItemPurchased(itemName))
        {
            Debug.Log("Warunki spe³nione, otwieram ConfirmPanel.");
            selectedItem = itemName;
            selectedItemPrice = price;
            selectedButton = itemButton; // Zapisujemy referencjê do przycisku
            ConfirmText.text = $"Do you want to buy {itemName} for {price} coins?";
            ConfirmPanel.SetActive(true);

            ConfirmYes.onClick.RemoveAllListeners();
            ConfirmNo.onClick.RemoveAllListeners();

            ConfirmYes.onClick.AddListener(BuyItem);
            ConfirmNo.onClick.AddListener(CancelPurchase);
        }
        else
        {
            Debug.Log("Nie masz wystarczaj¹co monet lub przedmiot zosta³ ju¿ kupiony.");
        }
    }

    private void BuyItem()
    {
        Buy.Play();

        coins -= selectedItemPrice;
        PlayerPrefs.SetInt("Coins_" + currentProfile, coins);

        // Zapisywanie zakupu TYLKO dla aktualnego profilu
        PlayerPrefs.SetInt($"Item_{selectedItem}_{currentProfile}", 1);
        PlayerPrefs.Save();
        UpdateCoinsUI();
        ConfirmPanel.SetActive(false);

        // Dezaktywacja przycisku po zakupie
        if (selectedButton != null)
        {
            selectedButton.interactable = false;
        }
    }

    private void CancelPurchase()
    {
        ConfirmPanel.SetActive(false);
    }

    private void UpdateCoinsUI()
    {
        CoinsText.text = "Coins: " + coins;
    }

    public bool IsItemPurchased(string itemName)
    {
        return PlayerPrefs.GetInt($"Item_{itemName}_{currentProfile}", 0) == 1;
    }

    // Wrapper do przypiêcia w Unity
    public void TryBuyItemWrapper(int itemIndex)
    {
        if (itemButtons == null || itemIndex >= itemButtons.Length)
        {
            Debug.LogWarning("Brak przypisanego przycisku do tego przedmiotu!");
            return;
        }

        switch (itemIndex)
        {
            case 0: TryBuyItem("Trash Bag", 10, itemButtons[0]); break;
            case 1: TryBuyItem("Watering Can", 10, itemButtons[1]); break;
            case 2: TryBuyItem("Sprayer", 30, itemButtons[2]); break;
            case 3: TryBuyItem("Gnome", 20, itemButtons[3]); break;
        }
    }

    // Sprawdzenie, które przedmioty s¹ kupione dla danego profilu i wy³¹czenie ich przycisków
    private void UpdateShopButtons()
    {
        if (itemButtons == null || itemButtons.Length == 0) return;

        string[] itemNames = { "Trash Bag", "Watering Can", "Sprayer", "Gnome" };

        for (int i = 0; i < itemNames.Length; i++)
        {
            if (IsItemPurchased(itemNames[i]))
            {
                itemButtons[i].interactable = false;
            }
            else
            {
                itemButtons[i].interactable = true; // Jeœli nowy profil, aktywujemy
            }
        }
    }
}
