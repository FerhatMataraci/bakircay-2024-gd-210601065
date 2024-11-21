using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance; // Singleton yapısı

    private List<DraggableObject> allObjects = new List<DraggableObject>(); // Tüm eşleşebilen nesneler
    public GameObject endScreen; // Bölüm sonu ekranı
    public TextMeshProUGUI scoreText; // Puan göstergesi
    public TextMeshProUGUI timeText; // Zaman göstergesi

    private float levelStartTime; // Bölümün başlangıç zamanı
    private bool isLevelFinished = false; // Bölüm bitti mi kontrolü

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        levelStartTime = Time.time; // Bölüm başlangıç zamanı kaydediliyor
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

        // Nesne kalmadıysa bölümü bitir
        if (allObjects.Count == 0 && !isLevelFinished)
        {
            EndLevel();
        }
    }

    private void EndLevel()
    {
        isLevelFinished = true;

        // Skor ve süreyi hesapla
        float finishTime = Time.time - levelStartTime;
        int finalScore = UIManager.Instance.GetScore(); // Skoru UIManager'dan alıyoruz

        // Bölüm sonu ekranını güncelle ve göster
        endScreen.SetActive(true);
        scoreText.text = "Score: " + finalScore.ToString();
        timeText.text = "Time: " + finishTime.ToString("F2") + " seconds";

        // Oyunu durdur
        Time.timeScale = 0f; // Oyun duruyor
    }
}
