using UnityEngine;
using System.Collections;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

[Serializable] public class OnItemClick : UnityEvent<ItemDefinition> { }
public class ShopGridItemFill : MonoBehaviour
{
    [SerializeField] public Transform content;
    [SerializeField] public OnItemClick buy_event;
    [SerializeField] public OnItemClick item_event;

    [SerializeField] public bool shop;

    public void Fill(List<ItemDefinition> _item_defs)
    {
        print("Items: " + _item_defs.Count);
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }


        foreach (var item_def in _item_defs)
        {
 
            GameObject _ui_obj = Instantiate(item_def.GetAttribute<Attribute<GameObject>>("item_ui_shop").GetValue());
            print(_ui_obj.name);

            _ui_obj.GetComponent<RectTransform>().SetParent(content);
            _ui_obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
            _ui_obj.GetComponent<RectTransform>().localScale = Vector3.one;

            _ui_obj.GetComponent<ShopGridItem>().ApplyAttributes(item_def, buy_event, item_event);
 
        }

    }
}
