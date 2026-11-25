using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IngredientDrag : MonoBehaviour
{
    public string ingredient;
    public float returnSpeed = 25f;
    public int dragSortingBoost = 10;

    Camera cam;
    Vector3 startPosition;
    Vector3 grabOffset;
    SpriteRenderer sprite;
    int baseSortingOrder;
    CauldronDropZone zone;
    bool dragging;
    bool returning;

    void Awake()
    {
        cam = Camera.main;
        startPosition = transform.position;
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) baseSortingOrder = sprite.sortingOrder;

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body == null) body = gameObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        GetComponent<Collider2D>().isTrigger = true;
    }

    Vector3 MouseWorld()
    {
        Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
        p.z = transform.position.z;
        return p;
    }

    void OnMouseDown()
    {
        if (returning) return;
        dragging = true;
        grabOffset = transform.position - MouseWorld();
        if (sprite != null) sprite.sortingOrder = baseSortingOrder + dragSortingBoost;
    }

    void OnMouseDrag()
    {
        if (!dragging) return;
        transform.position = MouseWorld() + grabOffset;
    }

    void OnMouseUp()
    {
        if (!dragging) return;
        dragging = false;
        if (zone != null) zone.Drop(ingredient);
        StartCoroutine(ReturnHome());
    }

    IEnumerator ReturnHome()
    {
        returning = true;
        while ((transform.position - startPosition).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = startPosition;
        if (sprite != null) sprite.sortingOrder = baseSortingOrder;
        returning = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CauldronDropZone found = other.GetComponent<CauldronDropZone>();
        if (found != null) zone = found;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (zone != null && other.GetComponent<CauldronDropZone>() == zone) zone = null;
    }
}
