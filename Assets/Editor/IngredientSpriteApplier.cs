using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class IngredientSpriteApplier
{
    const string Root = "Assets/karsiori/Pixel Potion Pack - Animated/Sprites/";

    [MenuItem("PotionGame/Apply Ingredient Sprites")]
    public static void Apply()
    {
        AssetDatabase.Refresh();
        Apply("Mushroom", Root + "Round Potion/RED/Sprites/Round Potion - RED - 0000.png");
        Apply("Frog", Root + "Large Bottle/GREEN/Sprites/Large Bottle - GREEN - 0000.png");
        Apply("MoonDust", Root + "Glowing Potion/PURPLE/Sprite/Glowing Potion - PURPLE - 0000.png");
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    static void Apply(string objectName, string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning("Bulunamadi: " + path);
            return;
        }
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        GameObject obj = GameObject.Find(objectName);
        if (obj == null || sprite == null)
        {
            Debug.LogWarning("Sahnede obje bulunamadi: " + objectName);
            return;
        }

        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sprite = sprite;
            renderer.color = Color.white;
        }

        obj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        BoxCollider2D box = obj.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = sprite.bounds.size;
            box.offset = sprite.bounds.center;
        }

        EditorUtility.SetDirty(obj);
    }
}
