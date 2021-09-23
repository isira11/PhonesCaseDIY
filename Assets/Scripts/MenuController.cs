using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.SaveSystem;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public ItemFilter itemFilter;
    public Transform stage;
    public Transform menu_env;
    public TextMeshProUGUI coins;
    public LevelBar levelBar;


    void Awake()
    {
        Advertising.UnityAdsClient.Init();

    }

    private void Start()
    {
        Advertising.ShowBannerAd(BannerAdPosition.Bottom);
    }

    public void UpdatePhones()
    {
        itemFilter.FillItemsWithCat(InventorySystemManager.GetItemCategory("item_phones"), true);
    }

    public void OnPhoneSelected(ItemDefinition item_def)
    {
        foreach (Transform item in stage)
        {
            Destroy(item.gameObject);
        }

        GameObject _phone_obj = Instantiate(item_def.GetAttribute<Attribute<GameObject>>("preview_obj").GetValue());
        _phone_obj.transform.parent = stage;
        _phone_obj.transform.localPosition = Vector3.zero;
        _phone_obj.transform.localRotation = Quaternion.identity;

        SelectedPhone.selected_phone = _phone_obj;
        SelectedPhone.itemDefinition = item_def;
    }


    public void OnVisibilityChanged(float s)
    {
        menu_env.gameObject.SetActive(s!=0);

        if (s==1)
        {
            SaveManager.instance.Init();

            coins.text = SaveManager.instance.GetCoins().ToString();
            levelBar.SetLevel(SaveManager.instance.GetXP());
            UpdatePhones();

            Advertising.ShowBannerAd(BannerAdPosition.Bottom);
        }
    }

}
