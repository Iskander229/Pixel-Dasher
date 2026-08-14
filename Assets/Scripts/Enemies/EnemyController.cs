using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 2;
    protected int currentHealth;
    public int damage = 1;

    protected SpriteRenderer spriteRenderer;
    protected Color ogColor;

    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ogColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        StartCoroutine(FlashWhite());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = ogColor;
    }

    protected virtual void Die()
    {
        Debug.Log("Enemy died");

        foreach (LootItem lootItem in lootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.ItemPrefab);
                Debug.Log($"Enemy dropped: {lootItem.ItemPrefab.name} - item");
            }
            else
            {
                Debug.Log("Enemy dropped No Items");
            }
            break; // only ever evaluates first loot entry, see note below
        }

        Destroy(gameObject);
    }

    protected void InstantiateLoot(GameObject loot)
    {
        if (loot)
        {
            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);
            droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
