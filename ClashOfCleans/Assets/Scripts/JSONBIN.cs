using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 

public class JsonBinManagers : MonoBehaviour
{
    private static readonly string API_KEY = "$2a$10$igQLnuMoSmveeydQkijDXOCmjRN0ZW0B/w1EoQQFsUV6gNrZPZg66";
    private static readonly string BIN_ID = "693b20e5ae596e708f934e13";
    private static readonly string BASE_URL = "https://api.jsonbin.io/v3/b";

    public delegate void DataLoadedCallback(JObject data);
    public delegate void DataSavedCallback(JObject savedData);


    public void LoadDataFromJsonBin(DataLoadedCallback callback)
    {
        StartCoroutine(LoadDataFromJsonBinCoroutine(callback));
    }
    private IEnumerator LoadDataFromJsonBinCoroutine(DataLoadedCallback callback)
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
                callback?.Invoke(actualData);  // Return the actual data
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

    public void SaveDataToJsonBin(JObject data, DataSavedCallback callback)
    {
        StartCoroutine(SaveDataToJsonBinCoroutine(data, callback));
    }
    private IEnumerator SaveDataToJsonBinCoroutine(JObject data, DataSavedCallback callback)
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

    public bool AddNewUser(JObject data, string userName, int bestTime)
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

    private void Start()
    {
        LoadDataFromJsonBin(OnDataLoaded);
    }

    private void OnDataLoaded(JObject data)
    {
        if (data == null)
        {
            Debug.LogError("Fehler beim Laden der Daten.");
            return;
        }
        Debug.Log(data);
        if (AddNewUser(data, "max", 25))
        {
            SaveDataToJsonBin(data, null);
        }
        Debug.Log(data);
    }
}