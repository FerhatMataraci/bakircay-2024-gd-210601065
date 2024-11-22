using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private List<DraggableObject> allObjects = new List<DraggableObject>(); 
    public GameObject endScreen;
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI timeText; 

    private float levelStartTime; 
    private bool isLevelFinished = false; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        levelStartTime = Time.time;
    }

    public void RegisterObject(DraggableObject obj)
    {
        if (!allObjects.Contains(obj))
        {
            allObjects.Add(obj);
        }
    }

    public void RemoveObject(DraggableObject obj)
    {
        if (allObjects.Contains(obj))
        {
            allObjects.Remove(obj);
        }

        if (allObjects.Count == 0 && !isLevelFinished)
        {
            EndLevel();
        }
    }

    private void EndLevel()
    {
        isLevelFinished = true;

        float finishTime = Time.time - levelStartTime;
        int finalScore = UIManager.Instance.GetScore();

        endScreen.SetActive(true);
        scoreText.text = "Score: " + finalScore.ToString();
        timeText.text = "Time: " + finishTime.ToString("F2") + " seconds";

        Time.timeScale = 0f;
    }
}
