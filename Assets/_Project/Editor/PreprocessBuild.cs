#region
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
#endregion

/// <summary>
/// Performs actions before the build process starts.
/// <para>Clears the statbuffs json, sets the balance to zero, and deletes all playerprefs.</para>
/// </summary>
public class PreprocessBuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }

    public void OnPreprocessBuild(BuildReport report)
    {
        Logger.LogWarning("Preprocessing the build...");

        // clear the statbuffs json
        File.WriteAllText(Application.persistentDataPath + "/statBuffs.json", string.Empty);

        // check if the itemDescriptions.json exists and load it
        if (File.Exists(Application.persistentDataPath + "/itemDescriptions.json"))
        {
            Item.SaveAllDescriptionsToJson();
            Item.LoadAllDescriptionsFromJson();
        }
        else
        {
            throw new FileNotFoundException($"File not found at path: {Application.persistentDataPath}/itemDescriptions.json");
        }

        // set balance to zero
        PlayerPrefs.SetInt("Balance", 0);
        Balance.Coins = 0;

        // delete all playerprefs
        PlayerPrefs.DeleteAll();
    }
}