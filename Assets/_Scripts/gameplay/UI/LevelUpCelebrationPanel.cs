using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpCelebrationPanel : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI rewardText;
    public Button continueButton;
    public GameObject particleEffect;

    [Header("Settings")]
    public float autoHideDelay = 3f;
    public bool pauseGameDuringCelebration = false;

    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(Hide);

        Hide();
    }

    private void Start()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnLevelUp += ShowCelebration;
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnLevelUp -= ShowCelebration;
    }

    public void ShowCelebration(int newLevel)
    {
        if (panel != null)
            panel.SetActive(true);

        if (levelText != null)
            levelText.text = $"LEVEL {newLevel}!";

        if (rewardText != null)
            rewardText.text = GetLevelRewardText(newLevel);

        if (particleEffect != null)
            particleEffect.SetActive(true);

        if (pauseGameDuringCelebration)
            Time.timeScale = 0f;

        if (autoHideDelay > 0)
            Invoke(nameof(Hide), autoHideDelay);
    }

    private string GetLevelRewardText(int level)
    {
        return $"All buildings production speed increased!\nNew features unlocked!";
    }

    private void Hide()
    {
        if (panel != null)
            panel.SetActive(false);

        if (particleEffect != null)
            particleEffect.SetActive(false);

        if (pauseGameDuringCelebration)
            Time.timeScale = 1f;
    }
}