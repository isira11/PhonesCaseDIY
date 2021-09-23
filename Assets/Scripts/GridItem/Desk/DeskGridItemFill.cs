using UnityEngine;
using System.Collections;
using Opsive.UltimateInventorySystem.Core;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;

public class DeskGridItemFill : MonoBehaviour
{
    public Transform content;
    public OnItemClick item_event;
    public ItemSelectedVisual itemSelectedVisual;

    public void Fill(List<ItemDefinition> _item_defs)
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }

        itemSelectedVisual.Reset();

        foreach (var item_def in _item_defs)
        {

            GameObject _ui_obj = Instantiate(item_def.GetAttribute<Attribute<GameObject>>("item_ui").GetValue());

            _ui_obj.GetComponent<RectTransform>().SetParent(content);
            _ui_obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
            _ui_obj.GetComponent<RectTransform>().localScale = Vector3.one;

            _ui_obj.GetComponent<DeskGridItem>().ApplyAttributes(item_def, item_event);

            itemSelectedVisual.Add(item_def,_ui_obj);
        }

        if (_item_defs.Count > 0)
        {
            item_event.Invoke(_item_defs[0]);
        }
    }
}
