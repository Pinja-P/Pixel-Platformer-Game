using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject enemy;
    public GameObject LoadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;

    public GameObject LevelDoneCanvas;

    public ParticleSystem victory;
    public ParticleSystem victory2;

    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoad.OnHoldComplete += LoadNextLevel;
        LoadCanvas.SetActive(false);
        LevelDoneCanvas.SetActive(false);
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        if (progressAmount >= 110)
        {
            //Level complete!
            LoadCanvas.SetActive(true);
            LevelDoneCanvas.SetActive(true);
            Debug.Log("Level Complete");

            victory.Play();
            victory2.Play();
        }
    }

    void LoadNextLevel()
    {
        // Basically one line if statement. If we're on final level then set level back to 0. Otherwise load next level
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadCanvas.SetActive(false);
        LevelDoneCanvas.SetActive(false);

        // Disables old level and enables new level
        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[nextLevelIndex].gameObject.SetActive(true);

        player.transform.position = new Vector3(14, -4, 0);
        enemy.transform.position = new Vector3(13, -4.8f, 0);


        currentLevelIndex = nextLevelIndex;
        progressAmount = 0;
        progressSlider.value = 0;
    }
}
