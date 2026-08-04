using NUnit.Framework;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject LoadCanvas;
    
    private static int currentLevelIndex = 0;
    private int nextLevelIndex = 0;
    private static int totalScenes;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
    }

    void Start()
    {
        totalScenes = SceneManager.sceneCountInBuildSettings; ;
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;

        HoldToLoadNextLevel.OnHoldComplete += LoadNextLevel;
        LoadCanvas.SetActive(false);
    }

    void IncreaseProgressAmount(int amount)
    {

        progressAmount += amount;
        Debug.Log(progressAmount);
        progressSlider.value = progressAmount;
        if (progressAmount >= 100)
        {
            //level complete!
            LoadCanvas.SetActive(true);
            Debug.Log("level complete!");
        }
    }

    void LoadNextLevel()
    {
        LoadCanvas.SetActive(false);

        nextLevelIndex = currentLevelIndex + 1;
        
        if(nextLevelIndex < totalScenes)
        {
            nextLevelIndex = 0;
        }

        SceneManager.LoadScene(nextLevelIndex);

        player.transform.position = new Vector3(0, 0, 0); //default starting pos

        currentLevelIndex = nextLevelIndex;

        //reset progress 
        progressAmount = 0;
        progressSlider.value = 0;
        progressSlider.minValue = 0;
        progressSlider.maxValue = 100;

    }
}