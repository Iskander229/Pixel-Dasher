using UnityEngine;

[System.Serializable]
public class LootItem 
{
    public GameObject ItemPrefab;
    [Range(0, 100)] public float dropChance;
}
