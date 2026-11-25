using UnityEngine;

public class CauldronTester : MonoBehaviour
{
    public Cauldron cauldron;
    public PotionData potion;
    public float time = 30f;

    void Start()
    {
        cauldron.Finished += OnFinished;
        cauldron.Begin(potion, time);
        Debug.Log("Tarif: " + string.Join(" > ", potion.recipe));
    }

    void OnFinished(bool success)
    {
        Debug.Log(success ? "BASARILI" : "BASARISIZ");
    }

    void OnDestroy()
    {
        cauldron.Finished -= OnFinished;
    }
}
