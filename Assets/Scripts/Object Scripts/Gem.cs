using System;
using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    public static event Action<int> OnGemCollect;
    public int worth = 25; //value of this object for progress bar

    public void Collect()
    {
        Debug.Log("GEM COLLECTED !");
        OnGemCollect?.Invoke(worth); //sets up an event that other scripts can subscsribe to
        gameObject.SetActive(false);
    }

}
