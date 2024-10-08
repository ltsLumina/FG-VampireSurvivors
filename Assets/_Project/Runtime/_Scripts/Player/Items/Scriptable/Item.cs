#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion

public abstract partial class Item : ScriptableObject
{
    /// <summary>
    ///     Contains the name, description and icon of the item.
    /// </summary>
    [Header("Item Details")]
    public Details details;
    
    public List<WeaponLevels> weaponLevels;
    public List<PassiveLevels> passiveLevels;
    WeaponLevels levels;

    public string Name
    {
        get => details.name;
        private set => details.name = value;
    }

    public string Description => details.description;

    public Sprite Icon => details.icon;
    
    #region Utility | OnValidate
    void OnValidate()
    {
        Name = name;

        #region WeaponLevels
        // Set the name of the structs' "name" variable to the index +1
        for (int i = 0; i < weaponLevels.Count; i++)
        {
            WeaponLevels levels = weaponLevels[i];
            levels.name     = "Level " + (i + 1);
            weaponLevels[i] = levels;
        }

        // Set the level value to the index +1
        for (int i = 0; i < weaponLevels.Count; i++)
        {
            WeaponLevels levels = weaponLevels[i];
            levels.level    = i + 1;
            weaponLevels[i] = levels;
        }
        #endregion

        #region PassiveLevels
        // Set the name of the structs' "name" variable to the index +1
        for (int i = 0; i < passiveLevels.Count; i++)
        {
            PassiveLevels levels = passiveLevels[i];
            levels.name     = "Level " + (i + 1);
            passiveLevels[i] = levels;
        }

        // Set the level value to the index +1
        for (int i = 0; i < passiveLevels.Count; i++)
        {
            PassiveLevels levels = passiveLevels[i];
            levels.level    = i + 1;
            passiveLevels[i] = levels;
        }
        #endregion

        // Set the name of the item to the name of the class
        details.name = string.Concat(GetType().Name.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

        // throw error if the level is out of bounds
        OutOfBounds();

        return;
        void OutOfBounds()
        {
            // bug: The list is empty for 1 frame when recompiling so I just don't throw an error if the list is empty
            if (weaponLevels.Count == 0 || weaponLevels == null) return;

            for (int i = 0; i < weaponLevels.Count; i++)
            {
                WeaponLevels weaponLevels = this.weaponLevels[i];
                if (weaponLevels.level != i + 1) Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + weaponLevels.level);
            }
        }
    }
    #endregion

    public string GetItemLevelDescription(Item item)
    {
        LoadAllDescriptionsFromJson();

        if (LevelInvalid(out int _))
            // If the item isn't in the inventory, the level will be invalid. Therefore, return the description of the first level.
            return item is WeaponItem ? weaponLevels[0].description : passiveLevels[0].description;

        // return the description of the next level
        if (item.GetItemLevel() == 8) return "Max Level Reached. [This should never be displayed in regular gameplay, only in the editor.]";
        return item is WeaponItem ? weaponLevels[item.GetItemLevel()].description : passiveLevels[item.GetItemLevel()].description;
    }

    /// <summary>
    /// Uses the item. (Basic attack loop)
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// Plays the card and its associated effects.
    /// </summary>
    public abstract void Play();

    #region Utility | Create method
    /// <summary>
    ///     "Creates" and returns a random item from the list of potential items.
    /// </summary>
    /// <returns></returns>
    public static Item Create()
    {
        var potentialItems = new List<Item>(Resources.LoadAll<Item>("Items"));
        // Remove any items that are already in the inventory and are at level 8 (max level)
        //foreach (Item i in Inventory.Items.Where(i => potentialItems.Contains(i) && i.GetItemLevel() == 8)) { potentialItems.Remove(i); }
        //TODO: this will crash unity

        // return a random item from the list of potential items
        Item item = potentialItems[Random.Range(0, potentialItems.Count)];
        
        return item;
    }
    #endregion

    /// <summary>
    /// Ensures the level is within the bounds of the list.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    protected bool LevelInvalid(out int level)
    {
        level = this.GetItemLevel();

        if (level == -1) return true;

        switch (this)
        {
            case WeaponItem when level  >= 1 && level <= weaponLevels.Count:
            case PassiveItem when level >= 1 && level <= passiveLevels.Count:
                return false;

            default:
                Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
                return true;
        }
    }

    [Serializable]
    public struct Details
    {
        public string name;
        [Multiline]
        public string description;
        public Sprite icon;
    }

    [Serializable]
    public struct WeaponLevels
    {
        public enum StatTypes
        {
            Damage,
            Cooldown,
            Zone, // How much of the screen the item covers
        }

        [Header("Item Stats")]
        [HideInInspector, UsedImplicitly] // The name field is used to rename the "Element X" in the inspector to match the item level
        public string name;

        [HideInInspector]
        public int level;

        /// <summary>
        ///     The description of the item's stats at the current level. E.g. LvL 1: +10% damage, LvL 2: Knocks-back enemies, etc.
        ///     <para>Used by the Level-Up Canvas' Buttons to display the stats of the item at the current level.</para>
        /// </summary>
        public string description;
        public BaseStats baseStats;
        public ItemSpecificStats itemSpecificStats;
    }
    
    [Serializable]
    public struct PassiveLevels
    {
        [Header("Item Stats")]
        [HideInInspector, UsedImplicitly] // The name field is used to rename the "Element X" in the inspector to match the item level
        public string name;

        [HideInInspector]
        public int level;

        /// <summary>
        ///     The description of the item's stats at the current level. E.g. LvL 1: +10% damage, LvL 2: Knocks-back enemies, etc.
        ///     <para>Used by the Level-Up Canvas' Buttons to display the stats of the item at the current level.</para>
        /// </summary>
        public string description;
        public CharacterStats.Stats effectType;
    }

#if UNITY_EDITOR
    public void SetAllScriptableObjects(BaseStats[] allBaseStats, ItemSpecificStats[] allItemSpecificStats)
    {
        foreach (BaseStats baseStat in allBaseStats)
        {
            (string itemName, string type, int levelNumber) = ParseStatObjectName(baseStat.name);

            if (type != "Base" || itemName != Name) continue;

            for (int i = 0; i < weaponLevels.Count; i++)
            {
                if (weaponLevels[i].level != levelNumber) continue;
                WeaponLevels weaponLevel = weaponLevels[i];
                weaponLevel.baseStats = baseStat;
                weaponLevels[i]       = weaponLevel;
            }
        }

        foreach (ItemSpecificStats specificStat in allItemSpecificStats)
        {
            (string itemName, string type, int levelNumber) = ParseStatObjectName(specificStat.name);

            if (type != "Specific" || itemName != Name) continue;

            for (int i = 0; i < weaponLevels.Count; i++)
            {
                if (weaponLevels[i].level != levelNumber) continue;
                WeaponLevels weaponLevel = weaponLevels[i];
                weaponLevel.itemSpecificStats = specificStat;
                weaponLevels[i]               = weaponLevel;
            }
        }
    }

    static (string itemName, string type, int level) ParseStatObjectName(string objectName)
    {
        // Define a regex pattern to match the item name, type, and level in the name
        var pattern = @"(.+)\s(Base|Specific)\sLvL\s(\d+)";
        var match   = Regex.Match(objectName, pattern);

        if (match.Success)
        {
            var itemName = match.Groups[1].Value;
            var type     = match.Groups[2].Value;
            var level    = int.Parse(match.Groups[3].Value);
            return (itemName, type, level);
        }

        throw new ArgumentException("Invalid object name format: " + objectName);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Item), true)]
public class ItemEditor : Editor
{
    SerializedProperty details;
    SerializedProperty passiveLevels;
    SerializedProperty weaponLevels;
    
    // Passive item specific:
    // - Effect type (enum)
    // - Effect value (float)
    
    SerializedProperty passiveType;
    SerializedProperty passiveEffect;

    void OnEnable()
    {
        details       = serializedObject.FindProperty("details");
        weaponLevels  = serializedObject.FindProperty("weaponLevels");
        passiveLevels = serializedObject.FindProperty("passiveLevels");
        
        passiveType = serializedObject.FindProperty("effectType");
        passiveEffect = serializedObject.FindProperty("effect");
    }

    public override void OnInspectorGUI()
    {
        var item = target as Item;
        
        serializedObject.Update();

        // Check the type of the target object and conditionally display fields
        EditorGUILayout.PropertyField(details, true);
        if (target is WeaponItem) EditorGUILayout.PropertyField(weaponLevels, true);
        if (target is PassiveItem) EditorGUILayout.PropertyField(passiveLevels, true);

        if (target is PassiveItem)
        {
            EditorGUILayout.PropertyField(passiveType);
            EditorGUILayout.PropertyField(passiveEffect);
        }

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(20);
        
        if (item is WeaponItem)
        {
            using var scope = new EditorGUILayout.HorizontalScope();

            GUILayout.FlexibleSpace();

            var content = new GUIContent("Set Scriptable Objects", "Set the BaseStats and ItemSpecificStats for each level of this item.");
            
            if (GUILayout.Button(content, GUILayout.Height(25), GUILayout.Width(250)))
            {
                if (item)
                {
                    var allBaseStats         = Resources.LoadAll<BaseStats>("Items");
                    var allItemSpecificStats = Resources.LoadAll<ItemSpecificStats>("Items");
                    item.SetAllScriptableObjects(allBaseStats, allItemSpecificStats);
                }
            }

            GUILayout.FlexibleSpace();
        }

        using (new GUILayout.HorizontalScope("box"))
        {
            var saveContent = new GUIContent("Save Level Descriptions", "Save the Level Descriptions to a JSON file.");
            var loadContent = new GUIContent("Load Level Descriptions", "Load the Level Descriptions from a JSON file.");
            
            if (GUILayout.Button(saveContent)) Item.SaveAllDescriptionsToJson();
            if (GUILayout.Button(loadContent)) Item.LoadAllDescriptionsFromJson();
        }
    }
}
#endif
