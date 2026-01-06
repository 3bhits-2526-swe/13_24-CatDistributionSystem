using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Cat Distribution/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public BuildingType buildingType;
    public BuildingSize size;
    public int baseCost;
    public Sprite sprite;
    public GameObject prefab;

    [Header("Production")]
    public float baseProductionTime = 1f;
    public int maxLevel = 3;

    [Header("Connections")]
    public int maxInputs = 0;
    public int maxOutputs = 0;
}