using System;
using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    public static event Action<int> OnGemCollect;
    public int worth = 5; //value of this object for progress bar

    public void Collect()
    {
        OnGemCollect.Invoke(worth); //sets up an event that other scripts can subscsribe to
        Destroy(gameObject);
    }

}
