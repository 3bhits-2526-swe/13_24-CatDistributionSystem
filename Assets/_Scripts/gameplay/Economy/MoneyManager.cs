using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [SerializeField] private int money;

    public int Money => money;

    private void Awake()
    {
        Instance = this;
    }

    public void Add(int amount)
    {
        money += amount;
        Debug.Log($"Earned Money: +{amount}");
    }

    public bool CanAfford(int amount)
    {
        return money >= amount;
    }

    public bool Spend(int amount)
    {
        if (!CanAfford(amount))
            return false;

        money -= amount;
        return true;
    }
}
