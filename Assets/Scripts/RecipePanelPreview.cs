using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipePanelPreview : MonoBehaviour
{
    [SerializeField] private PotionData testPotion;
    [SerializeField] private TMP_Text potionTitle;
    [SerializeField] private TMP_Text ingredientList;
    [SerializeField] private Image potionIcon;
    [SerializeField] private float displaySeconds = 5f;

    private IEnumerator Start()
    {
        if (testPotion == null || potionTitle == null || ingredientList == null)
        {
            Debug.LogWarning("Assign Test Potion, Potion Title and Ingredient List.", this);
            yield break;
        }

        potionTitle.text = testPotion.potionName;

        if (potionIcon != null)
        {
            potionIcon.sprite = testPotion.icon;
            potionIcon.preserveAspect = true;
            potionIcon.enabled = testPotion.icon != null;
        }

        StringBuilder recipeText = new StringBuilder();
        string[] ingredients = testPotion.recipe;

        if (ingredients != null)
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                recipeText.AppendLine($"{i + 1}. {ingredients[i]}");
            }
        }

        ingredientList.text = recipeText.ToString().TrimEnd();

        yield return new WaitForSeconds(displaySeconds);
        gameObject.SetActive(false);
    }
}