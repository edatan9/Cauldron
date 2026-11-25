using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PotionTestSceneBuilder
{
    [MenuItem("PotionGame/Build Test Scene")]
    public static void Build()
    {
        Sprite square = GetSquare();
        PotionData potion = GetPotion();

        GameObject cauldronObj = new GameObject("Cauldron");
        cauldronObj.transform.position = new Vector3(3f, 0f, 0f);
        cauldronObj.transform.localScale = new Vector3(3f, 3f, 1f);
        SpriteRenderer cauldronRenderer = cauldronObj.AddComponent<SpriteRenderer>();
        cauldronRenderer.sprite = square;
        cauldronRenderer.color = new Color(0.3f, 0.2f, 0.45f);
        cauldronObj.AddComponent<BoxCollider2D>();
        Cauldron cauldron = cauldronObj.AddComponent<Cauldron>();
        CauldronDropZone zone = cauldronObj.AddComponent<CauldronDropZone>();
        zone.cauldron = cauldron;
        CauldronTester tester = cauldronObj.AddComponent<CauldronTester>();
        tester.cauldron = cauldron;
        tester.potion = potion;

        CreateIngredient("Mushroom", new Vector3(-4f, 2f, 0f), new Color(0.85f, 0.3f, 0.3f), square);
        CreateIngredient("Frog", new Vector3(-4f, 0f, 0f), new Color(0.3f, 0.8f, 0.35f), square);
        CreateIngredient("MoonDust", new Vector3(-4f, -2f, 0f), new Color(0.4f, 0.6f, 0.95f), square);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Selection.activeGameObject = cauldronObj;
    }

    static void CreateIngredient(string ingredientName, Vector3 position, Color color, Sprite sprite)
    {
        GameObject obj = new GameObject(ingredientName);
        obj.transform.position = position;
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = 1;
        obj.AddComponent<BoxCollider2D>();
        IngredientDrag drag = obj.AddComponent<IngredientDrag>();
        drag.ingredient = ingredientName;
    }

    static Sprite GetSquare()
    {
        string path = "Assets/Sprites/White.png";
        if (!AssetDatabase.IsValidFolder("Assets/Sprites")) AssetDatabase.CreateFolder("Assets", "Sprites");
        if (AssetDatabase.LoadAssetAtPath<Sprite>(path) == null)
        {
            Texture2D texture = new Texture2D(4, 4);
            Color[] pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();
            System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
            AssetDatabase.ImportAsset(path);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 4;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static PotionData GetPotion()
    {
        string path = "Assets/Potions/TestPotion.asset";
        if (!AssetDatabase.IsValidFolder("Assets/Potions")) AssetDatabase.CreateFolder("Assets", "Potions");
        PotionData potion = AssetDatabase.LoadAssetAtPath<PotionData>(path);
        if (potion == null)
        {
            potion = ScriptableObject.CreateInstance<PotionData>();
            potion.potionName = "Test Potion";
            potion.recipe = new string[] { "Mushroom", "Frog", "MoonDust" };
            potion.baseBrewTime = 30f;
            AssetDatabase.CreateAsset(potion, path);
            AssetDatabase.SaveAssets();
        }
        return potion;
    }
}
