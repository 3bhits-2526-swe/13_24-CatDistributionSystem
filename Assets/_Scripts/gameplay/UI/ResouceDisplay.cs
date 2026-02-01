using UnityEngine;
using TMPro;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyDisplay;

    private void Start()
    {
        MoneyManager.Instance.MoneyChange += UpdateMoneyDisplay;
        UpdateMoneyDisplay();
    }

    private void UpdateMoneyDisplay() =>
        moneyDisplay.text = MoneyManager.Instance.Money.ToString() + "€";
}
