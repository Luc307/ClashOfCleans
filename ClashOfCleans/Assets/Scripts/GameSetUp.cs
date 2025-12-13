using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private GameObject MaskDude;
    [SerializeField] private GameObject NinjaFrog;
    [SerializeField] private GameObject PinkMan;
    [SerializeField] private GameObject SpaceGuy;
    [SerializeField] private int totalTrashCount = 12;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI trashCountText;
    
    private Dictionary<string, GameObject> skinDict;
    private int collectedTrashCount = 0;
    private float startTime;
    private bool isTimerRunning = false;

    private void Awake()
    {
        skinDict = new Dictionary<string, GameObject>()
        {
            { "MaskDude", MaskDude },
            {"NinjaFrog", NinjaFrog},
            {"PinkMan", PinkMan},
            {"SpaceGuy", SpaceGuy }
        };
        PlayerData.player = skinDict[PlayerData.skinName];
    }
    
    private void Start()
    {
        Instantiate(PlayerData.player, new Vector3(0, 5, 0), Quaternion.identity, transform);
        
        // Start the stopwatch when the game scene loads
        StartTimer();
        
        // Update UI
        UpdateTrashCountUI();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            // Update timer display
            float elapsedTime = Time.time - startTime;
            UpdateTimerDisplay(elapsedTime);
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        if (isTimerRunning)
        {
            PlayerData.finalTime = Time.time - startTime;
            if(PlayerData.bestTime == 0f || PlayerData.finalTime < PlayerData.bestTime)
            {
                PlayerData.bestTime = PlayerData.finalTime;
            }
            isTimerRunning = false;
            UpdateTimerDisplay(PlayerData.finalTime);
            SceneManager.LoadScene("Leaderboard");
        }
    }

    public void CollectTrash()
    {
        collectedTrashCount++;
        UpdateTrashCountUI();
        
        // Check if all trash is collected
        if (collectedTrashCount >= totalTrashCount)
        {
            StopTimer();
            Debug.Log("All trash collected! Final time: " + FormatTime(PlayerData.finalTime));
        }
    }

    private void UpdateTrashCountUI()
    {
        if (trashCountText != null)
        {
            trashCountText.text = $"Trash: {collectedTrashCount}/{totalTrashCount}";
        }
    }

    private void UpdateTimerDisplay(float time)
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + FormatTime(time);
        }
    }

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}