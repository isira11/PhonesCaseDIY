using UnityEngine;
using System.Collections;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;

public class DeskGridCat : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI item_name;
    public Button button;



    public void ApplyAttributes(ItemCategory _item_cat, OnDeskCatClick _event)
    {


        string name = _item_cat.GetCategoryAttribute<Attribute<string>>("cat_name").GetValue();

        if (item_name)
        {
            item_name.text = name;
        }

        if (button)
        {
            button.onClick.AddListener(() => { _event.Invoke(_item_cat, true); }); ;
        }
    }
}
