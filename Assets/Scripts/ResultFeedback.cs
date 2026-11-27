using System.Collections;
using UnityEngine;

// Cadıya ve kazana ayrı ayrı eklenebilir.
// İksir sonucuna göre: renk flaşı + "pop" animasyonu + (varsa) sprite değişimi.
public class ResultFeedback : MonoBehaviour
{
    [Header("Hedef (boşsa kendi SpriteRenderer'ı)")]
    public SpriteRenderer target;

    [Header("Opsiyonel sprite'lar")]
    public Sprite successSprite;
    public Sprite failSprite;

    [Header("Efekt")]
    public Color successColor = new Color(0.4f, 1f, 0.4f);
    public Color failColor = new Color(1f, 0.35f, 0.35f);
    public float popScale = 1.25f;
    public float duration = 0.5f;

    GameManager gm;
    Vector3 baseScale;
    Color baseColor;
    Sprite baseSprite;
    Coroutine routine;

    void Awake()
    {
        if (target == null) target = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        if (target != null)
        {
            baseColor = target.color;
            baseSprite = target.sprite;
        }
    }

    void Start()
    {
        gm = GameManager.Instance;
        if (gm != null) gm.OnPotionResult += Play;
    }

    void OnDestroy()
    {
        if (gm != null) gm.OnPotionResult -= Play;
    }

    void Play(bool success)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Effect(success));
    }

    IEnumerator Effect(bool success)
    {
        Color flash = success ? successColor : failColor;
        Sprite s = success ? successSprite : failSprite;

        if (target != null)
        {
            target.color = flash;
            if (s != null) target.sprite = s;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            transform.localScale = baseScale * (1f + (popScale - 1f) * Mathf.Sin(k * Mathf.PI));
            if (target != null) target.color = Color.Lerp(flash, baseColor, k);
            yield return null;
        }

        transform.localScale = baseScale;
        if (target != null)
        {
            target.color = baseColor;
            target.sprite = baseSprite;
        }
        routine = null;
    }
}
