using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float time;

    public LeaderboardEntry(string name, float time)
    {
        this.playerName = name;
        this.time = time;
    }
}

/// <summary>
/// Leaderboard display component.
/// Uses LeaderboardManager for data storage (session-based, not persistent).
/// For WebGL: Can be extended to fetch data from server API instead.
/// </summary>
public class Leaderboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private int maxEntries = 10;
    [SerializeField] private string menuSceneName = "Menu";

    private void Start()
    {
        // Add current score if available
        if (PlayerData.finalTime > 0)
        {
            LeaderboardManager.AddScore(PlayerData.playerName, PlayerData.finalTime);
            PlayerData.finalTime = 0f; // Reset after adding
        }
        
        UpdateLeaderboardDisplay();
    }

    private void UpdateLeaderboardDisplay()
    {
        if (leaderboardText == null) return;

        List<LeaderboardEntry> entries = LeaderboardManager.GetLeaderboard(maxEntries);

        if (entries == null || entries.Count == 0)
        {
            leaderboardText.text = "No scores yet!";
            return;
        }

        string displayText = "LEADERBOARD\n\n";
        
        for (int i = 0; i < entries.Count; i++)
        {
            LeaderboardEntry entry = entries[i];
            string timeString = FormatTime(entry.time);
            displayText += $"{i + 1}. {entry.playerName} - {timeString}\n";
        }

        leaderboardText.text = displayText;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void ClearLeaderboard()
    {
        LeaderboardManager.Clear();
        UpdateLeaderboardDisplay();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("SkinMenu");
    }
}

