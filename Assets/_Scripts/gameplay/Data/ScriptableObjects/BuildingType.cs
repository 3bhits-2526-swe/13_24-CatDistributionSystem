using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CDS/Building Type")]
public class BuildingType : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public GameObject prefab;
    public Vector2Int size;
    public int cost;
    public int level;
    public Recipe[] recipes;
}
