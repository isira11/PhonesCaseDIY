using System;
using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class OnCategorySelected : UnityEvent<List<ItemDefinition>> { }

public class ItemFilter : MonoBehaviour
{

    [SerializeField] public OnCategorySelected onCategorySelected;

    public void FillItemsWithCat(ItemCategory category,bool use_player_inventory = false)
    {
        if (!use_player_inventory) { FillItemsWithCat(category); return; }

        InventoryIdentifier _ii;
        InventorySystemManager.InventoryIdentifierRegister.TryGetValue(13579, out _ii);
        Inventory inventory = _ii.Inventory;

        IReadOnlyList<ItemStack> _item_stacks = inventory.MainItemCollection.GetAllItemStacks();


        List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();

        foreach (ItemStack itemStack in _item_stacks)
        {
            if (itemStack.Item.Category.Equals(category))
            {
                itemDefinitions.Add(itemStack.Item.ItemDefinition);
            }
        }

        onCategorySelected.Invoke(itemDefinitions);
    }

    public void FillItemsWithCat(ItemCategory category)
    {
        IReadOnlyCollection<ItemDefinition> _item_defs= InventorySystemManager.ItemDefinitionRegister.GetAll();
   

        List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();

        foreach (ItemDefinition def in _item_defs)
        {
            if (def.Category.Equals(category))
            {
                itemDefinitions.Add(def);
            }
        }

        onCategorySelected.Invoke(itemDefinitions);
    }
}
