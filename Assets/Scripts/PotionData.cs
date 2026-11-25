using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "PotionGame/Potion")]
public class PotionData : ScriptableObject
{
    public string potionName;
    public Sprite icon;
    public string[] recipe;
    public float baseBrewTime = 10f;
}
