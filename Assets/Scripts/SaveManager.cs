using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Exchange;
using Opsive.UltimateInventorySystem.SaveSystem;
using UnityEngine;
using static PopUpManager;

public class SaveManager : MonoBehaviour
{
    public bool delete;
    public CurrencyOwner currencyOwner;

    public static SaveManager instance;
 

    public static string TOTAL_XP = "TOTAL_XP";


    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {

        try
        {
            if (delete)
            {
                SaveSystemManager.DeleteSave(0);
                SaveSystemManager.Save(0);
            }
            else
            {
                SaveSystemManager.Load(0);
            }
        }
        catch (System.Exception ex)
        {
            SaveSystemManager.Save(0);
        }

        delete = false;
    }


    public void AddXP(int amount)
    {
        int _current_amount = 0;
        if (ES3.KeyExists(TOTAL_XP))
        {
            _current_amount = GetXP();
        }

        _current_amount += amount;

        ES3.Save(TOTAL_XP,_current_amount);

        PopUpData popUpData = new PopUpData();
        popUpData.Description = amount + " XP Gained";
        popUpData.Title = "Congrats";
        ShowPopUp(popUpData);

    }

    public int GetXP()
    {
        if (!ES3.KeyExists(TOTAL_XP))
        {
            AddXP(99999);
        }

        return ES3.Load<int>(TOTAL_XP);
    }

    public int GetCoins()
    {
        return currencyOwner.CurrencyAmount.GetAmountOf(InventorySystemManager.GetCurrency("coins"));
    }

    public void addCoins(int amount)
    {
        currencyOwner.AddCurrency(InventorySystemManager.GetCurrency("coins"), amount);
        SaveSystemManager.Save(0);

        PopUpData popUpData = new PopUpData();

        popUpData.Description = "+" + amount + " COINS";
        popUpData.Title = "Congrats";

        ShowPopUp(popUpData);
    }
}
