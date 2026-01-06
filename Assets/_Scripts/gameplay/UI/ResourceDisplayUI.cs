using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplayUI : MonoBehaviour
{
    [Header("Money Display")]
    public TextMeshProUGUI moneyText;
    public string moneyPrefix = "$";

    [Header("Level Display")]
    public TextMeshProUGUI levelText;
    public string levelPrefix = "Level ";

    [Header("XP Display")]
    public Image xpFillImage;
    public TextMeshProUGUI xpText;
    public bool showXPNumbers = true;

    private void Start()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnMoneyChanged += UpdateMoneyDisplay;
            ResourceManager.Instance.OnExperienceChanged += UpdateXPDisplay;
            ResourceManager.Instance.OnLevelUp += UpdateLevelDisplay;

            UpdateMoneyDisplay(ResourceManager.Instance.GetMoney());
            UpdateLevelDisplay(ResourceManager.Instance.GetLevel());
            UpdateXPDisplay(ResourceManager.Instance.GetExperience(),
                          ResourceManager.Instance.GetExperienceToNextLevel());
        }
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnMoneyChanged -= UpdateMoneyDisplay;
            ResourceManager.Instance.OnExperienceChanged -= UpdateXPDisplay;
            ResourceManager.Instance.OnLevelUp -= UpdateLevelDisplay;
        }
    }

    private void UpdateMoneyDisplay(int amount)
    {
        if (moneyText != null)
            moneyText.text = moneyPrefix + amount.ToString();
    }

    private void UpdateLevelDisplay(int level)
    {
        if (levelText != null)
            levelText.text = levelPrefix + level.ToString();
    }

    private void UpdateXPDisplay(int current, int required)
    {
        if (xpFillImage != null)
        {
            float fillAmount = (float)current / required;
            xpFillImage.fillAmount = fillAmount;
        }

        if (xpText != null && showXPNumbers)
        {
            xpText.text = $"{current} / {required}";
        }
    }
}
