using UnityEngine;
using System.Collections;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;

[Serializable] public class OnDeskCatClick : UnityEvent<ItemCategory,bool> { }
public class DeskGridCatFill : MonoBehaviour
{
    [SerializeField] public Transform content;
    [SerializeField] public OnDeskCatClick on_cat_click;
    [SerializeField] public CategorySelectedVisual categorySelectedVisual;

    public void Fill(List<ItemCategory> _item_cats)
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }

        categorySelectedVisual.Reset();

        foreach (var item_def in _item_cats)
        {
            GameObject _ui_obj = Instantiate(item_def.GetCategoryAttribute<Attribute<GameObject>>("cat_ui").GetValue());
            print(_ui_obj.name);

            _ui_obj.GetComponent<RectTransform>().SetParent(content);
            _ui_obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
            _ui_obj.GetComponent<RectTransform>().localScale = Vector3.one;
            _ui_obj.GetComponent<DeskGridCat>().ApplyAttributes(item_def, on_cat_click);

            categorySelectedVisual.Add(item_def, _ui_obj);
        }

        if (_item_cats.Count > 0)
        {
            on_cat_click.Invoke(_item_cats[0], true);
        }

    }
}
