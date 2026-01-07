using UnityEngine;
using System;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Starting Resources")]
    public int startingMoney = 1000;
    public int startingLevel = 0;

    private int currentMoney;
    private int currentExperience;
    private int currentLevel;
    private int experienceToNextLevel = 100;

    public event Action<int> OnMoneyChanged;
    public event Action<int, int> OnExperienceChanged;
    public event Action<int> OnLevelUp;

    private void Awake()
    {
        Instance = this;
        currentMoney = startingMoney;
        currentLevel = startingLevel;
    }

    private void Start()
    {
        OnMoneyChanged?.Invoke(currentMoney);
        OnExperienceChanged?.Invoke(currentExperience, experienceToNextLevel);
    }

    public bool CanAfford(int cost) => currentMoney >= cost;
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public void SpendMoney(int amount)
    {
        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;

        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            LevelUp();
        }

        OnExperienceChanged?.Invoke(currentExperience, experienceToNextLevel);
    }

    private void LevelUp()
    {
        currentLevel++;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

        BuildingManager.Instance?.OnGlobalLevelUp(currentLevel);
        OnLevelUp?.Invoke(currentLevel);

        Debug.Log($"Level Up! Now level {currentLevel}");
    }

    public int GetMoney() => currentMoney;
    public int GetLevel() => currentLevel;
    public int GetExperience() => currentExperience;
    public int GetExperienceToNextLevel() => experienceToNextLevel;
}