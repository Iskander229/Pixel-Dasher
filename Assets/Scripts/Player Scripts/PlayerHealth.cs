using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int damageFromStomp = 1;
    private int currentHealth;

    public UI_Health healthUI; //get health container script which is UI for health

    private SpriteRenderer spriteRenderer; //to change color if needed
    private Color ogColor;


    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth); // set UI health max too

        spriteRenderer = GetComponent<SpriteRenderer>(); // GET PLAYERS RENDERER TO CHANGE ITS COLOR WHEN GETTING DAMAGE
        ogColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy)
        {
            if (IsPlayerAboveEnemy(enemy))
            {
                enemy.TakeDamage(damageFromStomp); // damage to Enemy now
                return; // Don't take damage, player is stomping the enemy
            }

            TakeDamage(enemy.damage);
        }
    }

    //Check if player is above the enemy
    private bool IsPlayerAboveEnemy(EnemyController enemy)
    {
        // Get positions
        Vector3 playerPos = transform.position;
        Vector3 enemyPos = enemy.transform.position;

        // Check if player is above the enemy (player's y is higher than enemy's y)
        // Adjust the threshold based on your game
        float threshold = 0.5f;
        return playerPos.y > enemyPos.y + threshold;
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth); //update UI health too

        
        //flash red
        StartCoroutine(FlashRed());

        if(currentHealth <= 0)
        {
            //player dead 
            Debug.Log($"Player DEAD! Health: {currentHealth}");
        }
        else
        {
            Debug.Log($"Player hit! Health: {currentHealth}");
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = ogColor;
    }
}
