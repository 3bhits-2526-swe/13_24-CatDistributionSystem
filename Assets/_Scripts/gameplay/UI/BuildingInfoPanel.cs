using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoPanel : MonoBehaviour
{
    public static BuildingInfoPanel Instance { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Dropdown recipeDropdown;

    private Image background;

    private BuildingType current;
    private IBuildingInspectable buildingInspectable;

    private void Awake()
    {
        Instance = this;
        root.SetActive(false);
        background = GetComponent<Image>();
        background.enabled = false;
    }

    public void Show(IBuildingInspectable building)
    {
        current = building.Type;
        buildingInspectable = building;
        root.SetActive(true);
        background.enabled = true;

        title.text = building.Type.displayName;
        icon.sprite = building.Type.icon;

        infoText.text =
            $"Level: {building.Level}\n" +
            $"Production Time: {building.ProductionTime}\n" +
            $"Size: {building.Type.size.x}x{building.Type.size.y}\n\n" +
            $"State: {building.StateText}\n\n" +
            $"Recipe:";

        SetupRecipes();
    }

    private void SetupRecipes()
    {
        recipeDropdown.ClearOptions();

        if (current.recipes == null || current.recipes.Length == 0)
        {
            recipeDropdown.gameObject.SetActive(false);
            return;
        }

        recipeDropdown.gameObject.SetActive(true);

        foreach (var recipe in current.recipes)
            recipeDropdown.options.Add(new TMP_Dropdown.OptionData(recipe.displayName));

        recipeDropdown.value = buildingInspectable.ActiveRecipeIndex;
        recipeDropdown.onValueChanged.RemoveAllListeners();
        recipeDropdown.onValueChanged.AddListener(OnRecipeChanged);
    }

    private void OnRecipeChanged(int index)
    {
        buildingInspectable.SetRecipe(index);
    }

    public void Hide()
    {
        root.SetActive(false);
        current = null;
        background.enabled = false;
    }
}
