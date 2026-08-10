using System;
using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    public static event Action<int> OnGemCollect;
    public int worth = 25; //value of this object for progress bar

    private bool collected;

    public void Collect()
    {
        if (collected)
            return;

        collected = true;
        //Debug.Log("GEM COLLECTED !");
        OnGemCollect?.Invoke(worth); //sets up an event that other scripts can subscsribe to
        Destroy(gameObject);
    }

}
