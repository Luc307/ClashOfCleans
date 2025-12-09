using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages leaderboard data during the session.
/// For WebGL projects, this can later be extended to use a server API.
/// Common approach: REST API with database (MySQL/PostgreSQL) on the server.
/// </summary>
public static class LeaderboardManager
{
    private static Dictionary<string, float> playerScores = new Dictionary<string, float>();
    private static List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

    /// <summary>
    /// Checks if a player name is already taken
    /// </summary>
    public static bool IsNameTaken(string playerName)
    {
        if (string.IsNullOrEmpty(playerName)) return false;
        return playerScores.ContainsKey(playerName);
    }

    /// <summary>
    /// Registers a player name (must be unique)
    /// </summary>
    public static bool RegisterPlayer(string playerName)
    {
        if (string.IsNullOrEmpty(playerName)) return false;
        if (IsNameTaken(playerName)) return false;

        playerScores[playerName] = float.MaxValue; // Initialize with worst possible time
        return true;
    }

    /// <summary>
    /// Adds or updates a player's score (time)
    /// </summary>
    public static void AddScore(string playerName, float time)
    {
        if (string.IsNullOrEmpty(playerName)) return;

        // If player doesn't exist, register them
        if (!playerScores.ContainsKey(playerName))
        {
            RegisterPlayer(playerName);
        }

        // Update score if this time is better (lower)
        if (time < playerScores[playerName])
        {
            playerScores[playerName] = time;
            UpdateLeaderboardEntries();
        }
    }

    /// <summary>
    /// Gets the best time for a player
    /// </summary>
    public static float GetPlayerBestTime(string playerName)
    {
        if (playerScores.ContainsKey(playerName))
        {
            return playerScores[playerName];
        }
        return float.MaxValue;
    }

    /// <summary>
    /// Gets all leaderboard entries sorted by time (best first)
    /// </summary>
    public static List<LeaderboardEntry> GetLeaderboard(int maxEntries = 10)
    {
        return leaderboardEntries.Take(maxEntries).ToList();
    }

    /// <summary>
    /// Updates the sorted leaderboard entries
    /// </summary>
    private static void UpdateLeaderboardEntries()
    {
        leaderboardEntries = playerScores
            .Select(kvp => new LeaderboardEntry(kvp.Key, kvp.Value))
            .OrderBy(entry => entry.time)
            .ToList();
    }

    /// <summary>
    /// Clears all leaderboard data (for testing or reset)
    /// </summary>
    public static void Clear()
    {
        playerScores.Clear();
        leaderboardEntries.Clear();
    }

    /// <summary>
    /// Gets the number of registered players
    /// </summary>
    public static int GetPlayerCount()
    {
        return playerScores.Count;
    }
}

