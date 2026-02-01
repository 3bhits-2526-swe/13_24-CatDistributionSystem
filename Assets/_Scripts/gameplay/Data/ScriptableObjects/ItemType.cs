using UnityEngine;

[CreateAssetMenu(menuName = "CDS/Item Type")]
public class ItemType : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite packagedSprite;
    [SerializeField] private int value = 1;
    [SerializeField] private float valueMultiplier;

    public string DisplayName => displayName;
    public Sprite Sprite => sprite;
    public Sprite PackagedSprite => packagedSprite;
    public int Value => value;
    public float ValueMultiplier => valueMultiplier;
}
