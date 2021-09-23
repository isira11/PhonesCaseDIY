using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;

public class DeskGridItem : MonoBehaviour
{
    public Image item_icon;
    public TextMeshProUGUI item_name;
    public Button item_button;



    public void ApplyAttributes(ItemDefinition _item_def,OnItemClick _item_event)
    {

        Color color = _item_def.GetAttribute<Attribute<Color>>("item_color").GetValue();
        Sprite icon = _item_def.GetAttribute<Attribute<Sprite>>("item_icon").GetValue();
        string name = _item_def.GetAttribute<Attribute<string>>("item_name").GetValue();

        if (item_name)
        {
            item_name.text = name;
        }

        if (item_icon)
        {
            item_icon.sprite = icon;
            item_icon.color = color;
        }

        if (item_button)
        {
            item_button.onClick.AddListener(() => { _item_event.Invoke(_item_def); }); ;
        }

    }
}
