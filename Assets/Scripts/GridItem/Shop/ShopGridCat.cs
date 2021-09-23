using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Opsive.UltimateInventorySystem.Core;
using TMPro;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;

public class ShopGridCat : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI item_name;
    public Button button;



    public void ApplyAttributes(ItemCategory _item_cat, OnShopCatClick _event)
    {

        string name = _item_cat.GetCategoryAttribute<Attribute<string>>("cat_name").GetValue();

        if (item_name)
        {
            item_name.text = name;
        }

        if (button)
        {
            button.onClick.AddListener(() => { _event.Invoke(_item_cat);}); ;
        }


    }



}
