﻿/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Managers
{
    using Opsive.Shared.Editor.UIElements;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.AttributeSystem;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Crafting;
    using Opsive.UltimateInventorySystem.Editor.Styles;
    using Opsive.UltimateInventorySystem.Editor.Utility;
    using Opsive.UltimateInventorySystem.Editor.VisualElements;
    using Opsive.UltimateInventorySystem.Editor.VisualElements.ControlTypes;
    using Opsive.UltimateInventorySystem.Editor.VisualElements.ViewNames;
    using Opsive.UltimateInventorySystem.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// The item definition manager.
    /// </summary>
    [OrderedEditorItem("Item Definitions", 3)]
    [RequireDatabase]
    [System.Serializable]
    public class ItemDefinitionManager : InventorySystemObjectBaseManager<ItemDefinition>
    {
        protected ItemCategoryField m_ItemCategoryField;
        protected ItemDefinitionField m_ParentField;
        protected UnityObjectFieldWithPreview m_IconField;
        protected TabToolbar m_AttributeCollectionsTabToolbar;
        protected AttributeVisualElement m_AttributeVisualElement;
        protected ReorderableList m_AttributesReorderableList;
        protected TabToolbar m_RelationshipsTabToolbar;
        protected ReorderableList m_RelationshipsReorderableList;

        /// <summary>
        /// Adds the visual elements to the ManagerContentContainer visual element. 
        /// </summary>
        public override void BuildVisualElements()
        {
            var list = GetList();
            if (list != null) {
                for (int i = 0; i < list.Count; i++) { list[i].Initialize(false); }
            }

            base.BuildVisualElements();

            m_ItemCategoryField = new ItemCategoryField(
                "Item Category",
                m_MainManagerWindow.Database,
                new (string, Action<ItemCategory>)[]
                {
                    ("Change for family (default)",c => { SetNewCategory(SelectedObject, c, Relation.Family); }),
                    ("Change for this only",c =>{ SetNewCategory(SelectedObject, c, Relation.None); }),
                    ("Change for parents",c =>{ SetNewCategory(SelectedObject, c, Relation.Parents); }),
                    ("Change for children",c =>{ SetNewCategory(SelectedObject, c, Relation.Children); }),
                },
                (cat) => cat.IsAbstract == false && cat != SelectedObject?.Category);

            m_ItemCategoryField.OnClose += () =>
            {
                Refresh();
            };
            m_ItemCategoryField.tooltip = "The category for the item definition. Defines its attributes";
            m_ContentPanel.Add(m_ItemCategoryField);

            m_ParentField = new ItemDefinitionField(
                "Parent",
                m_MainManagerWindow.Database,
                new (string, Action<ItemDefinition>)[]
                {
                    ("Set Parent",newParent => { ItemDefinitionEditorUtility.SetItemDefinitionParent(SelectedObject, newParent); })
                },
                (def) => SelectedObject?.SetParentCondition(def) ?? false);

            m_ParentField.OnClose += () =>
            {
                Refresh();
            };
            m_ParentField.tooltip =
                "The item definition may have a parent. The attributes are then inherited from the parent instead of the category";
            m_ContentPanel.Add(m_ParentField);

            m_IconField = new UnityObjectFieldWithPreview();
            m_IconField.tooltip = "The icon used in the custom editors, displayed next to the object name.";
            m_IconField.label = "Editor Icon";
            m_IconField.objectType = typeof(Sprite);
            m_IconField.RegisterValueChangedCallback(evt =>
            {
                ItemDefinitionEditorUtility.SetIcon(SelectedObject, evt.newValue as Sprite);
                Refresh();
            });
            m_ContentPanel.Add(m_IconField);

            var attributesBox = new VisualElement();
            attributesBox.name = "box";
            attributesBox.AddToClassList(ManagerStyles.BoxBackground);

            m_AttributeCollectionsTabToolbar = new TabToolbar(new string[]
            {
                "Item Definition Attributes",
                "Default Item Attributes",
            }, 0, ShowAttributeTab);
            attributesBox.Add(m_AttributeCollectionsTabToolbar);

            m_AttributeVisualElement = new AttributeVisualElement(m_MainManagerWindow.Database);
            m_AttributeVisualElement.AddToClassList(ManagerStyles.s_AttributeView_Margin);
            m_AttributesReorderableList = new ReorderableList(null, (VisualElement parent, int index) =>
            {
                parent.Add(new AttributeViewNameAndValue());
            }, (VisualElement parent, int index) =>
            {
                var attributeViewNameAndValue = parent.ElementAt(0) as AttributeViewNameAndValue;
                var attribute = m_AttributesReorderableList.ItemsSource[index] as AttributeBase;
                attributeViewNameAndValue.Refresh(attribute);
            }, null, (int index) =>
            {
                m_AttributeVisualElement.BindAttribute(m_AttributesReorderableList.ItemsSource[index] as AttributeBase);
            }, null, null,
            (i1, i2) =>
            {
                ItemDefinitionEditorUtility.SetItemDefinitionDirty(SelectedObject, true);
                Refresh();
            });
            m_AttributeVisualElement.OnValueChanged += (attribute) => m_AttributesReorderableList.Refresh();
            m_AttributeVisualElement.OnAttributeReplaced += (attribute) =>
            {
                ShowAttributeTab(m_AttributeCollectionsTabToolbar.Selected);
                m_AttributesReorderableList.SelectedIndex = m_AttributesReorderableList.ItemsSource.Count - 1;
            };
            attributesBox.Add(m_AttributesReorderableList);
            attributesBox.Add(m_AttributeVisualElement);
            m_ContentPanel.Add(attributesBox);

            var relationshipBox = new VisualElement();
            relationshipBox.name = "box";
            relationshipBox.AddToClassList(ManagerStyles.BoxBackground);

            m_RelationshipsTabToolbar = new TabToolbar(new string[]
            {
                "Item Categories",
                "Ancestors",
                "Descendants",
                "Direct Recipes",
                "Inherited Recipes"
            }, 0, ShowRelationshipTab);
            relationshipBox.Add(m_RelationshipsTabToolbar);

            m_RelationshipsReorderableList = new ReorderableList(null, (VisualElement parent, int index) =>
            {
                //parent.Add(new ViewName());
            }, (VisualElement parent, int index) =>
            {
                var viewName = parent.childCount == 0 ? null : parent.ElementAt(0) as ViewName;
                var obj = m_RelationshipsReorderableList.ItemsSource[index];
                if (obj == null) {
                    Debug.LogError("A Relation Object is null.");
                    return;
                }

                if (viewName == null || viewName.GetType().GetProperty("m_Object")?.PropertyType != obj.GetType()) {
                    viewName = ManagerUtility.CreateViewNameFor(obj);
                    parent.Clear();
                    parent.Add(viewName);
                }

                viewName.Refresh(obj);
            }, null, null, null, null, null);
            relationshipBox.Add(m_RelationshipsReorderableList);
            m_ContentPanel.Add(relationshipBox);
        }

        /// <summary>
        /// Set the ItemDefinition Category and serialize.
        /// </summary>
        /// <param name="itemDefinition">The ItemDefinition.</param>
        /// <param name="newCategory">The new ItemCategory.</param>
        /// <param name="relation">The relations that should be affected.</param>
        protected void SetNewCategory(ItemDefinition itemDefinition, ItemCategory itemCategory, Relation relation)
        {
            ItemDefinitionEditorUtility.SetItemDefinitionCategory(itemDefinition, itemCategory, relation);
            MainManagerWindow.NavigateTo(SelectedObject);
        }

        /// <summary>
        /// Shows the attributes that correspond to the tab at the specified index.
        /// </summary>
        /// <param name="index">The attribute tab index.</param>
        private void ShowAttributeTab(int index)
        {
            m_AttributeVisualElement.ClearBinding();
            m_AttributesReorderableList.Refresh(GetAttributeCollection(index).Attributes);

            var attributeIndex = m_AttributesReorderableList.SelectedIndex;
            if (attributeIndex >= 0 && attributeIndex < m_AttributesReorderableList.ItemsSource.Count) {
                m_AttributeVisualElement.BindAttribute(m_AttributesReorderableList.ItemsSource[attributeIndex] as AttributeBase);
            }
        }

        /// <summary>
        /// Returns the AttributeCollection at the specified index.
        /// </summary>
        /// <param name="index">The index to retrieve the AttributeCollection at.</param>
        /// <returns>The AttributeCollection at the specified index.</returns>
        private AttributeCollection GetAttributeCollection(int index)
        {
            var itemDefinition = SelectedObject;
            if (index == 0) { // Item Definition Attributes.
                return itemDefinition.ItemDefinitionAttributeCollection;
            } else { // Default Item Attributes.
                return itemDefinition.DefaultItem.ItemAttributeCollection;
            }
        }

        /// <summary>
        /// Shows the attributes that correspond to the tab at the specified index.
        /// </summary>
        /// <param name="index">The attribute tab index.</param>
        private void ShowRelationshipTab(int index)
        {
            var itemDefinition = SelectedObject;

            var itemCategories = new ItemCategory[0];
            var itemDefinitions = new ItemDefinition[0];
            IList list;
            switch (index) {
                case 0: list = ListUtility.CreateArrayCopy(itemDefinition.Category.GetAllParents(ref itemCategories, true), itemCategories); break; // Item Categories
                case 1: list = ListUtility.CreateArrayCopy(itemDefinition.GetAllParents(ref itemDefinitions, false), itemDefinitions); break; //Ancestors
                case 2: list = ListUtility.CreateArrayCopy(itemDefinition.GetAllChildren(ref itemDefinitions, false), itemDefinitions); break; //Descendants
                case 3: list = InventoryDatabaseUtility.DirectItemDefinitionRecipes(m_MainManagerWindow.Database.CraftingRecipes, itemDefinition); break; // Direct Recipes
                case 4: list = InventoryDatabaseUtility.InheritedItemDefinitionRecipes(m_MainManagerWindow.Database.CraftingRecipes, itemDefinition); break; // Inherit Recipes
                default: list = null; break;
            }

            m_RelationshipsReorderableList.Refresh(list);
        }

        /// <summary>
        /// Returns true if the definition name is unique.
        /// </summary>
        /// <param name="name">The possible name of the definition.</param>
        /// <returns>True if the definition name is unique.</returns>
        public override bool IsObjectNameValidAndUnique(string name)
        {
            for (int i = 0; i < m_MainManagerWindow.Database.ItemDefinitions.Length; ++i) {
                if (m_MainManagerWindow.Database.ItemDefinitions[i].name == name) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Refreshes the content for the current database.
        /// </summary>
        public override void Refresh()
        {
            var itemDefinitions = m_MainManagerWindow.Database?.ItemDefinitions;
            if (itemDefinitions == null) {
                return;
            }
            if (m_SelectedIndex == -1) {
                m_SelectedIndex = 0;
            }
            OnSelected(m_SelectedIndex);
        }

        /// <summary>
        /// Returns the list that the ReorderableList should use.
        /// </summary>
        /// <returns>The list that the ReorderableList should use.</returns>
        public override IList<ItemDefinition> GetList()
        {
            return m_MainManagerWindow.Database?.ItemDefinitions;
        }

        /// <summary>
        /// Returns a list of options for sorting.
        /// </summary>
        /// <returns>The list of sort options.</returns>
        public override IList<SortOption> GetSortOptions()
        {
            return ItemDefinitionEditorUtility.SortOptions();
        }

        /// <summary>
        /// Build the contextual menu when right-clicking a definition.
        /// </summary>
        /// <param name="evt">The event context.</param>
        void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Duplicate", DuplicateObject, DropdownMenuAction.AlwaysEnabled, evt.target);
            evt.menu.AppendAction("Create Recipe", CreateRecipeForItemDefinition, DropdownMenuAction.AlwaysEnabled, evt.target);
        }

        /// <summary>
        /// Create a recipe that has the item definition as output.
        /// </summary>
        /// <param name="action">The drop down menu action.</param>
        void CreateRecipeForItemDefinition(DropdownMenuAction action)
        {
            var definitionViewName = action.userData as ItemDefinitionViewName;
            var definition = definitionViewName?.ItemDefinition;

            if (definition == null) { return; }

            var validName = AssetDatabaseUtility.FindValidName(definition.name, m_MainManagerWindow.Database.CraftingRecipes);

            var newRecipe = CraftingRecipeEditorUtility.AddCraftingRecipe(validName,
                DatabaseValidator.UncategorizedCraftingCategory,
                m_MainManagerWindow.Database,
                m_MainManagerWindow.GetDatabaseDirectory());

            newRecipe.SetOutput(new CraftingOutput(new ItemAmounts(new ItemAmount[] { (1, Item.Create(definition)) })));
            newRecipe.m_EditorIcon = definition.m_EditorIcon;

            CraftingRecipeEditorUtility.SetCraftingRecipeDirty(newRecipe, true);

            MainManagerWindow.NavigateTo(newRecipe);
        }

        /// <summary>
        /// Duplicate the item definition using the right click.
        /// </summary>
        /// <param name="action">The drop down menu action.</param>
        void DuplicateObject(DropdownMenuAction action)
        {
            var definitionViewName = action.userData as ItemDefinitionViewName;
            var definitionToDuplicate = definitionViewName?.ItemDefinition;

            var newItemDefinition = ItemDefinitionEditorUtility.DuplicateItemDefinition(definitionToDuplicate,
                m_MainManagerWindow.Database,
                m_MainManagerWindow.GetDatabaseDirectory());

            Refresh();
            Select(newItemDefinition);
        }

        /// <summary>
        /// Creates the new ReorderableList item.
        /// </summary>
        /// <param name="parent">The parent ReorderableList item.</param>
        /// <param name="index">The index of the item.</param>
        public override void MakeListItem(VisualElement parent, int index)
        {
            var itemDefinitionViewName = new ItemDefinitionViewName();
            itemDefinitionViewName.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            parent.Add(itemDefinitionViewName);
        }

        /// <summary>
        /// Binds the ReorderableList item to the specified index.
        /// </summary>
        /// <param name="parent">The ReorderableList item that is being bound.</param>
        /// <param name="index">The index of the item.</param>
        public override void BindListItem(VisualElement parent, int index)
        {
            var itemDefinitionViewName = parent.ElementAt(0) as ItemDefinitionViewName;
            if (index < 0 || index >= m_ListPanel.SearchableList.ItemList.Count) { return; }
            var itemDefinition = m_ListPanel.SearchableList.ItemList[index];
            itemDefinitionViewName.Refresh(itemDefinition);
        }

        /// <summary>
        /// Update the visual elements to reflect the specified category.
        /// </summary>
        /// <param name="itemDefinition">The definition that is being displayed.</param>
        protected override void UpdateElements(ItemDefinition itemDefinition)
        {
            if (itemDefinition == null) { return; }
            itemDefinition.Initialize(false);

            m_Name.SetValueWithoutNotify(itemDefinition.name);
            m_ItemCategoryField.Refresh(itemDefinition.Category);
            m_ParentField.Refresh(itemDefinition.Parent);
            m_IconField.SetValueWithoutNotify(itemDefinition.m_EditorIcon);

            if (m_AttributeCollectionsTabToolbar.Selected < 0) { m_AttributeCollectionsTabToolbar.Selected = 0; }
            ShowAttributeTab(m_AttributeCollectionsTabToolbar.Selected);
            if (m_RelationshipsTabToolbar.Selected < 0) { m_RelationshipsTabToolbar.Selected = 0; }
            ShowRelationshipTab(m_RelationshipsTabToolbar.Selected);
        }

        /// <summary>
        /// The add button has been pressed.
        /// </summary>
        public override ItemDefinition OnAdd(string name)
        {
            return ItemDefinitionEditorUtility.AddItemDefinition(name,
                DatabaseValidator.UncategorizedItemCategory,
                m_MainManagerWindow.Database,
                m_MainManagerWindow.GetDatabaseDirectory());
        }

        /// <summary>
        /// The remove button has been pressed.
        /// </summary>
        /// <param name="index">The index of the selected object.</param>
        public override void OnRemove(int index)
        {
            // No more work needs to be performed if the object is empty.
            if (SelectedIndexOutOfRange) { return; }

            string warningMessage;
            if (SelectedObject.Children.Count == 0) {
                warningMessage = $"Are you sure you want to remove the Item Definition '{SelectedObject}'? This action cannot be undone.";
            } else if (SelectedObject.Children.Count == 1) {
                warningMessage = $"Are you sure you want to remove the Item Definition '{SelectedObject}' with 1 child? This action cannot be undone.";
            } else {
                warningMessage = $"Are you sure you want to remove the Item Definition '{SelectedObject}' with {SelectedObject.Children.Count} children? " +
                                  "This action cannot be undone.";
            }

            if (EditorUtility.DisplayDialog("Remove Item Definition?", warningMessage, "Yes", "Cancel")) {
                ItemDefinitionEditorUtility.RemoveItemDefinition(SelectedObject, m_MainManagerWindow.Database);
                OnSelected(-1);
            }
        }

        /// <summary>
        /// Search filter for the ItemDefinition list
        /// </summary>
        /// <param name="list">The list to search.</param>
        /// <param name="searchValue">The search string.</param>
        /// <returns>A new filtered list.</returns>
        public override IList<ItemDefinition> GetSearchFilter(IList<ItemDefinition> list, string searchValue)
        {
            return ItemDefinitionEditorUtility.SearchFilter(list, searchValue);
        }
    }
}