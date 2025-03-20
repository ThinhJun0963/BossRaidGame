using System.Collections;
using TMPro;
using UnityEngine;

public class EconomyManager : Singleton<EconomyManager>
{
    private TMP_Text goldText;
    private int currentGold = 0;
    private const string COIN_AMOUNT_TEXT = "Gold Amount Text"; // Ensure the correct GameObject name
    private const string GOLD_KEY = "UserCoins"; // Key for storing gold in PlayerPrefs

    private void Start()
    {
        // Load stored gold amount
        currentGold = PlayerPrefs.GetInt(GOLD_KEY, 0);

        // Find gold text only once
        goldText = GameObject.Find(COIN_AMOUNT_TEXT)?.GetComponent<TMP_Text>();
        UpdateGoldDisplay();
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        PlayerPrefs.SetInt(GOLD_KEY, currentGold); // Save new gold amount
        PlayerPrefs.Save();
        UpdateGoldDisplay();
    }

    public void SubtractGold(int amount)
    {
        currentGold = Mathf.Max(0, currentGold - amount); // Prevent negative values
        PlayerPrefs.SetInt(GOLD_KEY, currentGold);
        PlayerPrefs.Save();
        UpdateGoldDisplay();
    }

    public int GetCurrentGold()
    {
        return currentGold;
    }

    public void UpdateGoldDisplay()
    {
        if (goldText != null)
        {
            goldText.text = currentGold.ToString("D3"); // Always 3 digits (e.g., 001, 100)
        }
        else
        {
            Debug.LogWarning("⚠ Gold text UI not found! Ensure the GameObject name is correct.");
        }
    }
}
