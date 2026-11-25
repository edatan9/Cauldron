using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CauldronDropZone : MonoBehaviour
{
    public Cauldron cauldron;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Drop(string ingredient)
    {
        cauldron.Add(ingredient);
    }
}
