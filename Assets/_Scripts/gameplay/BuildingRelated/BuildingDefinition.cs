using UnityEngine;

[CreateAssetMenu(menuName = "Factory/Building Definition")]
public class BuildingDefinition : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2Int size;
    [SerializeField] private int cost;

    public GameObject Prefab => prefab;
    public Vector2Int Size => size;
    public int Cost => cost;
}
