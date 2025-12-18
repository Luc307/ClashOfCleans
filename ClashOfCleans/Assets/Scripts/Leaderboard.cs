using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI leaderboardText;
    [SerializeField] private Button skinMenuBtn;

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt(time * 1000f % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    private float ParseTime(string timeString)
    {
        if (string.IsNullOrEmpty(timeString)) return float.MaxValue;
        string[] parts = timeString.Split(':');
        if (parts.Length != 2) return float.MaxValue;
        string[] subParts = parts[1].Split('.');
        if (subParts.Length != 2) return float.MaxValue;
        int minutes = int.Parse(parts[0]);
        int seconds = int.Parse(subParts[0]);
        int milliseconds = int.Parse(subParts[1]);
        return minutes * 60f + seconds + milliseconds / 1000f;
    }

    private void UpdateLeaderboardUI(JObject data)
    {
        if (data == null)
        {
            leaderboardText.text = "Fehler beim Laden der Bestenliste.";
            return;
        }

        JArray usersArray = data["users"] as JArray;
        if (usersArray == null)
        {
            leaderboardText.text = "Keine Benutzerdaten gefunden.";
            return;
        }

        // Sortiere die Benutzer nach Bestzeit (aufsteigend)
        var sortedUsers = usersArray.OrderBy(user => ParseTime((string)user["bestTime"])).Take(10);

        StringBuilder leaderboardBuilder = new StringBuilder();
        leaderboardBuilder.AppendLine("Bestenliste");
        leaderboardBuilder.AppendLine("-----------");
        int rank = 1;
        foreach (var user in sortedUsers)
        {
            string name = (string)user["name"];
            string bestTime = (string)user["bestTime"];
            leaderboardBuilder.AppendLine($"{rank}. {name} - {bestTime}");
            rank++;
        }

        leaderboardText.text = leaderboardBuilder.ToString();
    }
    private void OnDataLoaded(JObject data)
    {
        if (data == null) return;

        //check if player already exists
        bool userExists = false;
        foreach (var user in data["users"])
        {
            if ((string)user["name"] == PlayerData.playerName)
            {
                userExists = true;
                break;
            }
        }
        if(userExists)
        {
            foreach (var user in data["users"])
            {
                if ((string)user["name"] == PlayerData.playerName)
                {
                    user["bestTime"] = FormatTime(PlayerData.bestTime);
                    break;
                }
            }
        }
        else
        {
            JsonBinHelper.AddNewUser(data, PlayerData.playerName, FormatTime(PlayerData.bestTime)); 
        }
                
        JsonBinHelper.SaveDataToJsonBin(data, UpdateLeaderboardUI);
    }

    private void Awake()
    {
        JsonBinHelper.SetCoroutineRunner(this);
    }
    private void Start()
    {
        JsonBinHelper.LoadDataFromJsonBin(OnDataLoaded);

        if (skinMenuBtn != null)
        {
            skinMenuBtn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("SkinMenu");
            });
        }
    }
}

public static class JsonBinHelper
{
    private static readonly string API_KEY = "$2a$10$igQLnuMoSmveeydQkijDXOCmjRN0ZW0B/w1EoQQFsUV6gNrZPZg66";
    private static readonly string BIN_ID = "693b20e5ae596e708f934e13";
    private static readonly string BASE_URL = "https://api.jsonbin.io/v3/b";

    public delegate void DataLoadedCallback(JObject data);
    public delegate void DataSavedCallback(JObject savedData);

    // Statische Referenz zu einem MonoBehaviour für Coroutinen (muss einmal gesetzt werden, z.B. in Awake eines anderen Skripts)
    private static MonoBehaviour coroutineRunner;

    public static void SetCoroutineRunner(MonoBehaviour runner)
    {
        coroutineRunner = runner;
    }

    public static void LoadDataFromJsonBin(DataLoadedCallback callback)
    {
        if (coroutineRunner == null)
        {
            Debug.LogError("CoroutineRunner nicht gesetzt!");
            return;
        }
        coroutineRunner.StartCoroutine(LoadDataFromJsonBinCoroutine(callback));
    }

    private static IEnumerator LoadDataFromJsonBinCoroutine(DataLoadedCallback callback)
    {
        string url = $"{BASE_URL}/{BIN_ID}/latest";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("X-Master-Key", API_KEY);

        yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (www.result == UnityWebRequest.Result.Success)
#else
        if (!www.isNetworkError && !www.isHttpError)
#endif
        {
            string jsonResponse = www.downloadHandler.text;
            try
            {
                var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);
                var actualData = jsonObject["record"] as JObject;
                callback?.Invoke(actualData);
            }
            catch (System.Exception parseEx)
            {
                Debug.LogWarning($"Hinweis: JSON konnte nicht geparst werden: {parseEx.Message}");
                Debug.LogWarning("Aber die rohen Daten wurden erfolgreich geladen!");
                callback?.Invoke(null);
            }
        }
        else
        {
            Debug.LogError($"✗ Fehler! Status Code: {www.responseCode}");
            Debug.LogError($"Fehler Details: {www.downloadHandler.text}");
            callback?.Invoke(null);
        }
    }

    public static void SaveDataToJsonBin(JObject data, DataSavedCallback callback)
    {
        if (coroutineRunner == null)
        {
            Debug.LogError("CoroutineRunner nicht gesetzt!");
            return;
        }
        coroutineRunner.StartCoroutine(SaveDataToJsonBinCoroutine(data, callback));
    }

    private static IEnumerator SaveDataToJsonBinCoroutine(JObject data, DataSavedCallback callback)
    {
        string url = $"{BASE_URL}/{BIN_ID}";
        string jsonData = JsonConvert.SerializeObject(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest www = new UnityWebRequest(url, "PUT");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("X-Master-Key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (www.result == UnityWebRequest.Result.Success)
#else
        if (!www.isNetworkError && !www.isHttpError)
#endif
        {
            string responseText = www.downloadHandler.text;
            try
            {
                var jsonObject = JsonConvert.DeserializeObject<JObject>(responseText);
                var actualData = jsonObject["record"] as JObject;
                Debug.Log("✓ Daten wurden gespeichert!");
                callback?.Invoke(actualData);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Warnung: Gespeicherte Daten konnten nicht ausgelesen werden: {ex.Message}");
                callback?.Invoke(null);
            }
        }
        else
        {
            Debug.LogError($"✗ Fehler beim Speichern! Status Code: {www.responseCode}");
            Debug.LogError($"Fehler Details: {www.downloadHandler.text}");
            callback?.Invoke(null);
        }
    }

    public static bool AddNewUser(JObject data, string userName, string bestTime)
    {
        try
        {
            if (data == null)
            {
                Debug.LogError("Fehler: Daten konnten nicht gecastet werden");
                return false;
            }

            JArray usersArray = data["users"] as JArray;
            if (usersArray == null)
            {
                Debug.LogError("Fehler: users Array nicht gefunden");
                return false;
            }

            JObject newUser = new JObject
            {
                ["name"] = userName,
                ["bestTime"] = bestTime
            };

            usersArray.Add(newUser);

            Debug.Log($"✓ User '{userName}' wurde hinzugefügt!");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"✗ Fehler beim Hinzufügen: {ex.Message}");
            return false;
        }
    }
}