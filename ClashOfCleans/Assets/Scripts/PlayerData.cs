using UnityEngine;

public static class PlayerData
{
    public static string playerName { get; set; } = "";
    public static string skinName { get; set; } = "MaskDude";
    public static GameObject player { get; set; } = null;
    public static float finalTime { get; set; } = 0f;
    public static float bestTime { get; set; } = 0f;
    public static float finalTime2 { get; set; } = 0f;
    public static float bestTime2 { get; set; } = 0f;
    public static float finalTime1 { get; set; } = 0f;
    public static float bestTime1 { get; set; } = 0f;
    public static string sceneName { get; set; } = "1.Level";

    public static float GetFinalTime()
    {
        if(sceneName == "1.Level")
        {
            return finalTime1;
        }
        else
        {
            return finalTime2;
        }
    }
    public static float GetBestTime()
    {
        if(sceneName == "1.Level")
        {
            return bestTime1;
        }
        else
        {
            return bestTime2;
        }
    }
    public static void SetBestTime(float time)
    {
        if(sceneName == "1.Level")
        {
            bestTime1 = time;
        }
        else
        {
            bestTime2 = time;
        }
    }
    public static void SetFinalTime(float time)
    {
        if(sceneName == "1.Level")
        {
            finalTime1 = time;
        }
        else
        {
            finalTime2 = time;
        }
    }
}
