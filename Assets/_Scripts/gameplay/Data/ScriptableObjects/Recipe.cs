using UnityEngine;

[CreateAssetMenu(menuName = "CDS/Recipe")]
public class Recipe : ScriptableObject
{
    [SerializeField] private ItemType[] inputs;
    [SerializeField] private int[] inputCounts;

    [SerializeField] private ItemInstance[] outputs;
    [SerializeField] private int[] outputCounts;

    [SerializeField] private float baseProcessTime;

    public ItemType[] Inputs => inputs;
    public int[] InputCounts => inputCounts;
    public ItemInstance[] Outputs => outputs;
    public int[] OutputCounts => outputCounts;
    public float BaseProcessTime => baseProcessTime;
}
