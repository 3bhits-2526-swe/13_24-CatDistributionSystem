using UnityEngine;

[CreateAssetMenu(menuName = "CDS/PackagerRecipe")]
public class PackagerRecipe : ScriptableObject
{
    [SerializeField] private ItemType input;
    [SerializeField] private ItemInstance output;
    [SerializeField] private float processTime;
    [SerializeField] private float valueMultiplier;

    public ItemType Input => input;
    public ItemInstance Output => output;
    public float ProcessTime => processTime;
    public float ValueMultiplier => valueMultiplier;
}
