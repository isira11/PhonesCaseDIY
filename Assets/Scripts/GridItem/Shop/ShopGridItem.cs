using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Exchange;

public class ShopGridItem : MonoBehaviour
{
    public Image item_icon;
    public TextMeshProUGUI item_name;
    public TextMeshProUGUI item_level;
    public TextMeshProUGUI item_price;
    public Button item_button;
    public Button buy_button;
    public Transform not_owned;
    public Transform owned;
    public Transform locked;



    public void ApplyAttributes(ItemDefinition _item_def, OnItemClick _buy_event, OnItemClick _item_event)
    {

        Sprite icon = _item_def.GetAttribute<Attribute<Sprite>>("item_icon").GetValue();
        string name = _item_def.GetAttribute<Attribute<string>>("item_name").GetValue();
        int level = _item_def.GetAttribute<Attribute<int>>("item_level").GetValue();
        int price = _item_def.GetAttribute<Attribute<CurrencyAmounts>>("BuyPrice").GetValue()[0].Amount;
        Color color = _item_def.GetAttribute<Attribute<Color>>("item_color").GetValue();

        //name
        if (item_name)
        {
            item_name.text = name;
        }
        if (item_price)
        {
            item_price.text = price.ToString()+"C";
        }

        if(icon)
        {
            item_icon.sprite = icon ;

            item_icon.color = color;

        }
        bool _isLocked = false;
        //level
        if (item_level)
        {
            item_level.text = level.ToString();


            if (SaveManager.instance.GetXP() / 1000 >= level)
            {
                _isLocked = false;
            }
            else
            {
                _isLocked = true;
            }


        }

        //info button
        if (item_button)
        {
            item_button.onClick.AddListener(() => { _item_event.Invoke(_item_def); }); ;
        }

        //buy button
        if (!ShopController.instance.IsOwned(_item_def))
        {
            if (buy_button)
            {
                buy_button.onClick.AddListener(() => { _buy_event.Invoke(_item_def); }); ;
            }
            not_owned.gameObject.SetActive(!_isLocked);
            locked.gameObject.SetActive(_isLocked);
            owned.gameObject.SetActive(false);
        }
        else
        {
            not_owned.gameObject.SetActive(false);
            locked.gameObject.SetActive(false);
            owned.gameObject.SetActive(true);
        }


    }
}
