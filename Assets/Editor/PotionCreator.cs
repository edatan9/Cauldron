using UnityEditor;
using UnityEngine;

public static class PotionCreator
{
    [MenuItem("PotionGame/Create Level 1 Potions")]
    public static void CreateLevelOnePotions()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Potions")) AssetDatabase.CreateFolder("Assets", "Potions");
        Create("HealingPotion", "Healing Potion", new string[] { "Mushroom", "Frog" }, 12f);
        Create("NightVisionPotion", "Night Vision Potion", new string[] { "Frog", "MoonDust" }, 12f);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void Create(string fileName, string potionName, string[] recipe, float brewTime)
    {
        string path = "Assets/Potions/" + fileName + ".asset";
        PotionData potion = AssetDatabase.LoadAssetAtPath<PotionData>(path);
        if (potion == null)
        {
            potion = ScriptableObject.CreateInstance<PotionData>();
            AssetDatabase.CreateAsset(potion, path);
        }
        potion.potionName = potionName;
        potion.recipe = recipe;
        potion.baseBrewTime = brewTime;
        EditorUtility.SetDirty(potion);
    }
}
