using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

public class ItemSelectedVisual : MonoBehaviour
{

    Dictionary<ItemDefinition, GameObject> keyValuePairs = new Dictionary<ItemDefinition, GameObject>();

    GameObject current_selected;

    public void Reset()
    {
        keyValuePairs.Clear();
    }
    public void Add(ItemDefinition _item_def, GameObject ui)
    {
        keyValuePairs.Add(_item_def, ui);
    }

    public void OnSelected(ItemDefinition _item_def)
    {
        if (keyValuePairs.TryGetValue(_item_def, out GameObject obj))
        {
            if (current_selected)
            {
                current_selected.GetComponent<ProceduralImage>().color = Color.white;
            }
            current_selected = obj;
            current_selected.GetComponent<ProceduralImage>().color = Color.red;
        }
    }
}
