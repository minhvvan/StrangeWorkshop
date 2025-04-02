using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanelController : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private RectTransform requiredIconParent;
    [SerializeField] private GameObject requiredIconPrefab;


    public void SetInitialDescription(CraftRecipeSO recipe)
    {
        iconImage.sprite = recipe.craftRecipeIcon;
        nameText.text = recipe.craftRecipeName;
        descriptionText.text = "";

        RequiredIconCell currentIcon = null;
        HoldableObjectSO currentHoldableObject = null;
        foreach (var require in recipe.inputs)
        {
            if (currentHoldableObject == require)
            {
                if (currentIcon != null) currentIcon.AddCount();
                continue;
            }
            GameObject requiredIcon = Instantiate(requiredIconPrefab, requiredIconParent);
            requiredIcon.GetComponent<RequiredIconCell>().SetInitialize(require);
            currentIcon = requiredIcon.GetComponent<RequiredIconCell>();
            currentHoldableObject = require;
        }
    }

    public void ClearDescription()
    {
        var requiredIcons = requiredIconParent.GetComponentsInChildren<RequiredIconCell>();

        foreach (var icon in requiredIcons)
        {
            Destroy(icon.gameObject);
        }
    }
}
