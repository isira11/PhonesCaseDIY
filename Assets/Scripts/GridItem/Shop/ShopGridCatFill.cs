using UnityEngine;
using System.Collections;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

[Serializable] public class OnShopCatClick : UnityEvent<ItemCategory> { }
public class ShopGridCatFill : MonoBehaviour
{
    [SerializeField] public Transform content;
    [SerializeField] public OnShopCatClick on_cat_click;
    [SerializeField] public CategorySelectedVisual CategorySelectedVisual;

    public void Fill(List<ItemCategory> _item_cats)
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
        CategorySelectedVisual.Reset();

        foreach (var item_def in _item_cats)
        {
            GameObject _ui_obj = Instantiate(item_def.GetCategoryAttribute<Attribute<GameObject>>("cat_ui_shop").GetValue());
            print(_ui_obj.name);

            _ui_obj.GetComponent<RectTransform>().SetParent(content);
            _ui_obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
            _ui_obj.GetComponent<RectTransform>().localScale = Vector3.one;
            _ui_obj.GetComponent<ShopGridCat>().ApplyAttributes(item_def, on_cat_click);

            CategorySelectedVisual.Add(item_def,_ui_obj);
        }


    }

    public void SelectCat(ItemCategory _cat)
    {
        on_cat_click.Invoke(_cat);
    }

}
