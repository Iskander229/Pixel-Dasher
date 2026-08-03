using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    void Start()
    {
        progressSlider.value = 0;
        progressAmount = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;
        if (progressAmount >= 100)
        {
            //level complete!
            Debug.Log("level complete!");
        }
    }

    void Update()
    {
        
    }
}
