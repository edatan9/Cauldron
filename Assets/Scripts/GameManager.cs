using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --- Kişi 4 (UI) bu event'lere bağlanabilir ---
    public event Action<int> OnLifeChanged;
    public event Action<int> OnLevelChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnComboChanged;
    public event Action<float> OnTimeChanged;
    public event Action<bool> OnPotionResult;
    public event Action<PotionData> OnPotionStarted;
    public event Action<int> OnGameOver;   // son puanı gönderir

    [Header("Sahne referansları")]
    public PotionData[] potions;
    public Cauldron cauldron;
    public GameObject recipePanel;
    public TMP_Text recipeText;
    public TMP_Text timerText;
    public TMP_Text levelText;
    public TMP_Text livesText;
    public TMP_Text statusText;
    public TMP_Text scoreText;      // opsiyonel
    public TMP_Text comboText;      // opsiyonel
    public GameObject gameOverPanel; // opsiyonel (yoksa basit bir OnGUI paneli çıkar)

    [Header("Zorluk")]
    public float baseRecipeShowTime = 8f;
    public float recipeShowDecreasePerLevel = 0.7f;
    public float minRecipeShowTime = 1.5f;
    public float brewTimeDecreasePerLevel = 0.08f;
    public float minBrewTimeMultiplier = 0.35f;
    public int potionsPerLevel = 3;
    public int startLives = 3;

    [Header("Puan")]
    public int basePoints = 100;
    public float timeBonusPerSecond = 10f;
    public int maxComboMultiplier = 5;

    int level = 1;
    int madeInLevel;
    int lives;
    int score;
    int combo;
    bool isGameOver;
    Action<bool> activeHandler;

    public int Level { get { return level; } }
    public int Lives { get { return lives; } }
    public int Score { get { return score; } }
    public int Combo { get { return combo; } }
    public bool IsGameOver { get { return isGameOver; } }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (potions == null || potions.Length == 0 || cauldron == null)
        {
            Debug.LogError("GameManager: potions listesi veya cauldron atanmamış!", this);
            return;
        }
        StartGame();
    }

    void StartGame()
    {
        level = 1;
        madeInLevel = 0;
        lives = startLives;
        score = 0;
        combo = 0;
        isGameOver = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        RefreshUI();
        StartCoroutine(Run());
    }

    // Game over ekranındaki "Tekrar Oyna" butonu bunu çağırabilir
    public void RestartGame()
    {
        StopAllCoroutines();
        if (activeHandler != null)
        {
            cauldron.Finished -= activeHandler;
            activeHandler = null;
        }
        StartGame();
    }

    float RecipeShowTime()
    {
        return Mathf.Max(minRecipeShowTime, baseRecipeShowTime - (level - 1) * recipeShowDecreasePerLevel);
    }

    float BrewTime(PotionData potion)
    {
        return potion.baseBrewTime * Mathf.Max(minBrewTimeMultiplier, 1f - (level - 1) * brewTimeDecreasePerLevel);
    }

    static void SetText(TMP_Text t, string s)
    {
        if (t != null) t.text = s;
    }

    void RefreshUI()
    {
        SetText(levelText, "Level " + level);
        SetText(livesText, "Can: " + lives);
        SetText(scoreText, "Puan: " + score);
        SetText(comboText, combo > 1 ? "Combo x" + Mathf.Min(combo, maxComboMultiplier) : "");

        if (OnLevelChanged != null) OnLevelChanged(level);
        if (OnLifeChanged != null) OnLifeChanged(lives);
        if (OnScoreChanged != null) OnScoreChanged(score);
        if (OnComboChanged != null) OnComboChanged(combo);
    }

    void ShowTime(float t)
    {
        SetText(timerText, t.ToString("0.0"));
        if (OnTimeChanged != null) OnTimeChanged(t);
    }

    IEnumerator Run()
    {
        while (lives > 0)
        {
            RefreshUI();
            PotionData potion = potions[UnityEngine.Random.Range(0, potions.Length)];
            if (OnPotionStarted != null) OnPotionStarted(potion);

            // 1) Tarifi göster
            SetText(statusText, "Tarifi ezberle!");
            if (recipePanel != null) recipePanel.SetActive(true);
            SetText(recipeText, potion.potionName + "\n" + string.Join(" > ", potion.recipe));

            float t = RecipeShowTime();
            while (t > 0f)
            {
                ShowTime(t);
                t -= Time.deltaTime;
                yield return null;
            }
            if (recipePanel != null) recipePanel.SetActive(false);

            // 2) İksiri yap
            SetText(statusText, potion.potionName + " yap!");
            bool? result = null;
            activeHandler = r => result = r;
            cauldron.Finished += activeHandler;
            cauldron.Begin(potion, BrewTime(potion));
            while (result == null)
            {
                ShowTime(cauldron.TimeLeft);
                yield return null;
            }
            cauldron.Finished -= activeHandler;
            activeHandler = null;

            // 3) Sonuç
            if (result.Value)
            {
                int multiplier = Mathf.Min(1 + combo, maxComboMultiplier);
                int points = Mathf.RoundToInt((basePoints + cauldron.TimeLeft * timeBonusPerSecond) * multiplier);
                score += points;
                combo++;
                madeInLevel++;

                string msg = "Başarılı! +" + points;
                if (multiplier > 1) msg += " (x" + multiplier + ")";

                if (madeInLevel >= potionsPerLevel)
                {
                    madeInLevel = 0;
                    level++;
                    msg += "\nLevel " + level + "!";
                }
                SetText(statusText, msg);
            }
            else
            {
                lives--;
                combo = 0;
                SetText(statusText, "Başarısız!");
            }

            if (OnPotionResult != null) OnPotionResult(result.Value);
            RefreshUI();
            yield return new WaitForSeconds(1.5f);
        }

        // 4) Game over
        isGameOver = true;
        SetText(statusText, "Oyun bitti - Level " + level + " - Puan " + score);
        SetText(timerText, "");
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (OnGameOver != null) OnGameOver(score);
    }

    // Kişi 4 gerçek game over paneli yapana kadar geçici panel
    void OnGUI()
    {
        if (!isGameOver || gameOverPanel != null) return;

        float w = 300f, h = 160f;
        Rect box = new Rect(Screen.width / 2f - w / 2f, Screen.height / 2f - h / 2f, w, h);
        GUI.Box(box, "OYUN BITTI");
        GUI.Label(new Rect(box.x + 20, box.y + 40, w - 40, 30), "Level: " + level + "   Puan: " + score);
        if (GUI.Button(new Rect(box.x + 50, box.y + 90, w - 100, 40), "Tekrar Oyna"))
        {
            RestartGame();
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
