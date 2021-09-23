using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

public class CategorySelectedVisual : MonoBehaviour
{

    Dictionary<ItemCategory, GameObject> keyValuePairs = new Dictionary<ItemCategory, GameObject>();

    GameObject current_selected;

    public void Reset()
    {
        keyValuePairs.Clear();
    }
    public void Add(ItemCategory _item_cat,GameObject ui)
    {
        keyValuePairs.Add(_item_cat,ui);
    }



    public void OnSelected(ItemCategory _item_cat,bool _=false)
    {
        if (keyValuePairs.TryGetValue(_item_cat, out GameObject obj))
        {
            if (current_selected)
            {
                current_selected.GetComponent<ProceduralImage>().color = Color.white;
            }
            current_selected = obj;
            current_selected.GetComponent<ProceduralImage>().color = Color.black;
        }
    }

    public void OnSelected(ItemCategory _item_cat)
    {
        OnSelected(_item_cat,false);
    }

}
