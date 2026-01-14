using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetUp : MonoBehaviour
{
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private GameObject MaskDude;
    [SerializeField] private GameObject NinjaFrog;
    [SerializeField] private GameObject PinkMan;
    [SerializeField] private GameObject SpaceGuy;
    [SerializeField] private int totalTrashCount = 12;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI trashCountText;
    [SerializeField] private Button playAgainBtn;
    
    private Dictionary<string, GameObject> skinDict;
    private int collectedTrashCount = 0;
    private float startTime;
    private float finalTime;
    private float bestTime;
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
        Instantiate(PlayerData.player, new Vector3(0, 5, 0), Quaternion.identity, transform);
    }
    
    private void Start()
    {
        bestTime = PlayerData.GetBestTime();
        finalTime = PlayerData.GetFinalTime();

        StartTimer();
        
        UpdateTrashCountUI();

        playAgainBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("LevelMenu");
        });
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
            finalTime = Time.time - startTime;

            if (finalTime < bestTime || bestTime == 0)
            if (bestTime == 0f || finalTime < bestTime)
            {
                bestTime = finalTime;
            }
            isTimerRunning = false;
            UpdateTimerDisplay(finalTime);
            SceneManager.LoadScene("Leaderboard");

            PlayerData.SetBestTime(bestTime);
            PlayerData.SetFinalTime(finalTime);
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
            Debug.Log("All trash collected! Final time: " + FormatTime(finalTime));
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
        int milliseconds = Mathf.FloorToInt(time * 1000f % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}