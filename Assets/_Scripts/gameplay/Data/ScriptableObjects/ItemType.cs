using UnityEngine;

[CreateAssetMenu(menuName = "CDS/Item Type")]
public class ItemType : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int value;

    public string DisplayName => displayName;
    public Sprite Sprite => sprite;
    public int Value => value;
}
