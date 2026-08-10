
using System;
using UnityEngine;

public class HealthItem : MonoBehaviour, IItem
{
    public int healAmount = 1; //this item gives +1 health
    public static event Action<int> OnHealthCollect;

    private bool collected;
    public void Collect() 
    {
        if (collected)
            return;

        collected = true;

        OnHealthCollect.Invoke(healAmount); //when item is collected invoke player's "Heal" method.
        Destroy(gameObject);
        //Debug.Log("MORE HEALTH COLLECTED!");
    }
}
