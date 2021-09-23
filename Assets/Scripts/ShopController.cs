using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using Opsive.UltimateInventorySystem.Exchange.Shops;
using Opsive.UltimateInventorySystem.SaveSystem;
using TMPro;
using UnityEngine;
using static PopUpManager;

public class ShopController : MonoBehaviour
{
    public static ShopController instance;

    public Shop shop;
    public Inventory inventory;
    public CurrencyOwner currency_owner;
    public ShopGridCatFill shopgridCatFill;
    public Transform shop_env;
    public Transform stage;
    public LevelBar levelBar;
    public TextMeshProUGUI coins;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        OnShopOpened();
    }

    public void OnShopOpened()
    {
        ItemCategory shoppable = InventorySystemManager.GetItemCategory("shoppable");
        List<(ItemCategory, int)> list = shoppable.GetAllChildrenWithLevel(false, 1);

        List<ItemCategory> itemcats = new List<ItemCategory>();
        foreach (var item in list)
        {
            itemcats.Add(item.Item1);
        }

        shopgridCatFill.Fill(itemcats);

    }


    public bool IsOwned(ItemDefinition _item_def)
    {
        foreach (ItemStack item_stack in inventory.MainItemCollection.GetAllItemStacks())
        {
            if (item_stack.Item.ItemDefinition.ID.Equals(_item_def.ID))
            {
                return true;
            }

        }

        return false;
    }

    public void OnItemSelected(ItemDefinition itemDefinition)
    {
        foreach (Transform item in stage)
        {
            Destroy(item.gameObject);
        }

        GameObject _obj = Instantiate(itemDefinition.GetAttribute<Attribute<GameObject>>("preview_obj").GetValue());
        _obj.transform.parent = stage;
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.localRotation = Quaternion.identity;

        if(_obj.TryGetComponent(out IPreview preview))
        {
            preview.Preview(itemDefinition);
        }
    }

    public void BuyItem(ItemDefinition item_def)
    {

        ItemInfo iteminfo = new ItemInfo(item_def, 1);

        if (shop.BuyItem(inventory, currency_owner, iteminfo))
        {
            print("success");

            PopUpData popUpData = new PopUpData();
            popUpData.Title = "Purchase Success";
            popUpData.Description = item_def.GetAttribute<Attribute<string>>("item_name").GetValue() + " added to inventory";

            ShowPopUp(popUpData);
            shopgridCatFill.SelectCat(item_def.Category);
            UpdateResources();
            SaveSystemManager.Save(0);
        }
        else
        {
            print("failed");

            PopUpData popUpData = new PopUpData();
            popUpData.Title = "Purchase Failed";
            popUpData.Description = "Not  Enough Coins";
            ShowPopUp(popUpData);
        }



    }

    public void OnVisibilityChanged(float s)
    {
        shop_env.gameObject.SetActive(s != 0);
        if(s == 1)
        {
            
            UpdateResources();
        }
    }

    public void UpdateResources()
    {
        coins.text = SaveManager.instance.GetCoins().ToString();
        levelBar.SetLevel(SaveManager.instance.GetXP());
    }
}
