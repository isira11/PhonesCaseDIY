﻿/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Utility
{
    using Microsoft.CSharp;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using System.IO;
    using UnityEditor;

    /// <summary>
    /// A class that auto generates a C# class containing the names of all the objects in the database.
    /// </summary>
    public static class DatabaseNamesScriptGenerator
    {
        private static readonly CSharpCodeProvider s_CSharp = new CSharpCodeProvider();

        /// <summary>
        /// Generates a script which contains all the names of the object inside the database.
        /// </summary>
        /// <param name="folderPath">The folder path to save the script to.</param>
        /// <param name="database">The database.</param>
        public static void GenerateDatabaseNamesScript(string folderPath, InventorySystemDatabase database)
        {
            var fullPath = folderPath + "/" + ClassName(database) + ".cs";
            if (database == null) { return; }

            // We no longer create backups for generated scripts, if we decide otherwise use: CreateBackup(fullPath);

            using (TextWriter writer = new StreamWriter(fullPath, false)) {
                writer.Write(GenerateDatabaseNames(database));
                writer.Close();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Create a backup of existing files.
        /// </summary>
        /// <param name="fullPath">The full path to the existing file.</param>
        private static void CreateBackup(string fullPath)
        {
            if (File.Exists(fullPath) == false) { return; }

            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string backupFileNameOnly = fileNameOnly + "Backup";
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);

            string newFullPath = Path.Combine(path, backupFileNameOnly + extension);

            string newFileNameOnly = backupFileNameOnly;
            while (File.Exists(newFullPath)) {
                newFileNameOnly = string.Format("{0}_{1}", backupFileNameOnly, count++);
                newFullPath = Path.Combine(path, newFileNameOnly + extension);
            }

            File.Move(fullPath, newFullPath);

            string text = File.ReadAllText(newFullPath);
            text = text.Replace(fileNameOnly, newFileNameOnly);
            File.WriteAllText(newFullPath, text);
        }

        /// <summary>
        /// The generated script class name.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>The class name.</returns>
        public static string ClassName(InventorySystemDatabase database)
        {
            return VariableStringFormat(database.name, false) + "Names";
        }

        /// <summary>
        /// Generates a string which contains the script formatted version of the names of objects in the database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>All the names in the database.</returns>
        public static string GenerateDatabaseNames(InventorySystemDatabase database)
        {
            string categories = "";
            foreach (var category in database.ItemCategories) {
                categories += "\n" + GetCategoryDataNames(category) + "\n";
            }

            string itemDefinitions = "";
            foreach (var itemDefinition in database.ItemDefinitions) {
                itemDefinitions += GetItemDefinitionName(itemDefinition);
            }

            return "//This File is Auto-Generated by the editor window\n" +
                   $"//Date of creation: {DateTime.Now}\n" +
                   "namespace Opsive.UltimateInventorySystem.DatabaseNames " +
                   $"{{\n public static class {ClassName(database)} \n{{\n //ItemCategories \n{categories} \n//Item Definitions\n{itemDefinitions}\n }}\n}}";
        }

        /// <summary>
        /// Generate ItemCategory static classes, which contains names of its attributes.
        /// </summary>
        /// <param name="category">The ItemCategory.</param>
        /// <returns>The category names.</returns>
        public static string GetCategoryDataNames(ItemCategory category)
        {
            //Example
            //public static class Consumable
            //{
            //    public const string name = "Consumable";//Category Name
            //    public const string healAmount = "healAmount";//Int
            //}
            var attributeDefinitions = GetCategoryAttributeDefinitionNames(category);

            var itemDefinitions = GetCategoryItemDefinitionNames(category);

            return string.Format("public static class {0} \n{{\n public const string name = \"{1}\";//{2} \n\n//Attributes\n{3} \n//Item Definitions\n{4} }}",

                VariableStringFormat(category.name, false), category.name, category.GetType().Name, attributeDefinitions, itemDefinitions);
        }

        /// <summary>
        /// Generate the ItemDefinition names for all the ItemDefinitions in the category provided.
        /// </summary>
        /// <param name="category">The item category.</param>
        /// <returns>The definition names within the category.</returns>
        public static string GetCategoryItemDefinitionNames(ItemCategory category)
        {

            //Example
            //    public const string IronSword = "IronSword";//Item Definition
            var attributeDefinitions = "";
            var itemDefinitionsCount = category.ElementsReadOnly.Count;
            for (int i = 0; i < itemDefinitionsCount; i++) {
                attributeDefinitions += GetItemDefinitionName(category.ElementsReadOnly[i]);
            }

            return attributeDefinitions;
        }

        /// <summary>
        /// Generate the ItemDefinition name.
        /// </summary>
        /// <param name="itemDefinition">The itemDefinition.</param>
        /// <returns>The definition name.</returns>
        public static string GetItemDefinitionName(ItemDefinition itemDefinition)
        {
            if (itemDefinition == null) { return ""; }

            //Example
            //    public const string IronSword = "IronSword";//Item Definition
            return string.Format("public const string {0} = \"{1}\";//{2}\n ",
                    VariableStringFormat(itemDefinition.name, true), itemDefinition.name, itemDefinition.GetType().Name);
        }

        /// <summary>
        /// Generate the Attribute names for the attributes inside the category provided.
        /// </summary>
        /// <param name="category">The item category.</param>
        /// <returns>The definition attribute names.</returns>
        public static string GetCategoryAttributeDefinitionNames(ItemCategory category)
        {

            //Example
            //    public const string healAmount = "healAmount";//Int
            var attributeDefinitions = "";
            var allAttributeCount = category.GetAttributesCount();
            for (var i = 0; i < allAttributeCount; i++) {
                var attribute = category.GetAttributesAt(i);
                if (attribute.GetInheritAttribute() != null) { continue; }

                attributeDefinitions += string.Format("public const string {0} = \"{1}\";//{2}\n ",
                    VariableStringFormat(attribute.Name, true), attribute.Name, attribute.GetValueType().Name);
            }

            return attributeDefinitions;
        }

        /// <summary>
        /// Generates a valid variable name for the names provided.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="startLowerCase">Should it start with a lower case char.</param>
        /// <returns>The formatted name.</returns>
        public static string VariableStringFormat(string name, bool startLowerCase)
        {
            var output = "";
            bool upperCase = false;

            if (char.IsDigit(name[0])) {
                output = "_";
            }

            foreach (char c in name) {
                // Replace anything, but letters and digits, with space
                if (char.IsLetterOrDigit(c)) {
                    if (startLowerCase) {
                        output += char.ToLower(c);
                        startLowerCase = false;
                    } else if (upperCase) {
                        output += char.ToUpper(c);
                        upperCase = false;
                    } else {
                        output += c;
                    }
                } else {
                    upperCase = true;
                }
            }

            if (s_CSharp.IsValidIdentifier(output) == false) {
                output = "@" + output;
            }

            return output;
        }
    }
}

