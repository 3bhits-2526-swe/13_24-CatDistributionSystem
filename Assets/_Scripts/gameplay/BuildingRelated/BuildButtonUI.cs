using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildButtonUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private Button button;

    private BuildingType type;

    public void Bind(BuildingType buildingType)
    {
        type = buildingType;
        icon.sprite = buildingType.icon;
        label.text = buildingType.displayName;
        cost.text = buildingType.cost.ToString();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        PlacementState.Instance.BeginPlacement(type);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { PlacementState.Instance.BeginPlacement(type); }
    }

}
