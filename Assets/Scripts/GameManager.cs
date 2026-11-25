using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PotionData[] potions;
    public Cauldron cauldron;
    public GameObject recipePanel;
    public TMP_Text recipeText;
    public TMP_Text timerText;
    public TMP_Text levelText;
    public TMP_Text livesText;
    public TMP_Text statusText;

    public float baseRecipeShowTime = 8f;
    public float recipeShowDecreasePerLevel = 0.7f;
    public float minRecipeShowTime = 1.5f;
    public float brewTimeDecreasePerLevel = 0.08f;
    public float minBrewTimeMultiplier = 0.35f;
    public int potionsPerLevel = 3;
    public int startLives = 3;

    int level = 1;
    int madeInLevel;
    int lives;

    void Start()
    {
        lives = startLives;
        StartCoroutine(Run());
    }

    float RecipeShowTime()
    {
        return Mathf.Max(minRecipeShowTime, baseRecipeShowTime - (level - 1) * recipeShowDecreasePerLevel);
    }

    float BrewTime(PotionData potion)
    {
        return potion.baseBrewTime * Mathf.Max(minBrewTimeMultiplier, 1f - (level - 1) * brewTimeDecreasePerLevel);
    }

    void RefreshUI()
    {
        levelText.text = "Level " + level;
        livesText.text = "Can: " + lives;
    }

    IEnumerator Run()
    {
        while (lives > 0)
        {
            RefreshUI();
            PotionData potion = potions[UnityEngine.Random.Range(0, potions.Length)];

            statusText.text = "Tarifi ezberle!";
            recipePanel.SetActive(true);
            recipeText.text = potion.potionName + "\n" + string.Join(" > ", potion.recipe);

            float t = RecipeShowTime();
            while (t > 0f)
            {
                timerText.text = t.ToString("0.0");
                t -= Time.deltaTime;
                yield return null;
            }
            recipePanel.SetActive(false);

            statusText.text = potion.potionName + " yap!";
            bool? result = null;
            Action<bool> handler = r => result = r;
            cauldron.Finished += handler;
            cauldron.Begin(potion, BrewTime(potion));
            while (result == null)
            {
                timerText.text = cauldron.TimeLeft.ToString("0.0");
                yield return null;
            }
            cauldron.Finished -= handler;

            if (result.Value)
            {
                statusText.text = "Başarılı!";
                madeInLevel++;
                if (madeInLevel >= potionsPerLevel)
                {
                    madeInLevel = 0;
                    level++;
                }
            }
            else
            {
                statusText.text = "Başarısız!";
                lives--;
            }

            RefreshUI();
            yield return new WaitForSeconds(1.2f);
        }

        statusText.text = "Oyun bitti - Level " + level;
        timerText.text = "";
    }
}
