using UnityEngine;

[CreateAssetMenu(menuName = "CDS/Building Type")]
public class BuildingType : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2Int size;
    [SerializeField] private int cost;

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public GameObject Prefab => prefab;
    public Vector2Int Size => size;
    public int Cost => cost;
}
