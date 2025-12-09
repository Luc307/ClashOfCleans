# Leaderboard System - Dokumentation

## 📋 Übersicht aller Änderungen

### 1. **LeaderboardManager.cs** (NEU)
**Zweck:** Zentrale Verwaltung aller Leaderboard-Daten während der Session

**Funktionen:**
- `IsNameTaken(string playerName)` - Prüft, ob ein Name bereits vergeben ist
- `RegisterPlayer(string playerName)` - Registriert einen neuen Spieler (Name muss eindeutig sein)
- `AddScore(string playerName, float time)` - Fügt/aktualisiert die beste Zeit eines Spielers
- `GetPlayerBestTime(string playerName)` - Gibt die beste Zeit eines Spielers zurück
- `GetLeaderboard(int maxEntries)` - Gibt die Top-Scores sortiert zurück
- `Clear()` - Löscht alle Daten (für Tests/Reset)
- `GetPlayerCount()` - Gibt die Anzahl registrierter Spieler zurück

**Speicherung:** 
- Aktuell: In-Memory (Dictionary + List)
- Daten gehen beim Neustart verloren (Session-basiert)
- Keine PlayerPrefs mehr!

---

### 2. **NameInput.cs** (ÜBERARBEITET)
**Änderungen:**
- ✅ **Name-Validierung:** Prüft, ob Name leer ist
- ✅ **Eindeutigkeitsprüfung:** Verwendet `LeaderboardManager.IsNameTaken()`
- ✅ **Fehleranzeige:** Zeigt Fehlermeldungen an, wenn Name bereits vergeben
- ✅ **Automatische Registrierung:** Registriert den Namen im LeaderboardManager
- ✅ **Live-Feedback:** Versteckt Fehler, wenn Spieler tippt

**Neue Felder:**
- `errorText` (TextMeshProUGUI) - Optional, für Fehlermeldungen

**Ablauf:**
1. Spieler gibt Namen ein
2. Klickt auf "Play"
3. System prüft: Ist Name leer? → Fehler
4. System prüft: Ist Name bereits vergeben? → Fehler
5. System registriert Namen → Weiter zu SkinMenu

---

### 3. **Leaderboard.cs** (ÜBERARBEITET)
**Änderungen:**
- ❌ **Entfernt:** PlayerPrefs-Speicherung
- ❌ **Entfernt:** SaveLeaderboard() / LoadLeaderboard()
- ✅ **Neu:** Verwendet `LeaderboardManager` für alle Daten
- ✅ **Session-basiert:** Daten werden nur während der Session gespeichert

**Funktionsweise:**
1. Beim Start: Fügt aktuellen Score hinzu (wenn vorhanden)
2. Zeigt Leaderboard aus LeaderboardManager an
3. Sortiert automatisch nach Zeit (niedrigste = beste)

---

### 4. **GameSetUp.cs** (UNVERÄNDERT)
**Funktion:** 
- Startet Timer beim Spielstart
- Stoppt Timer, wenn alle Müllstücke gesammelt sind
- Lädt Leaderboard-Szene nach Spielende
- Speichert finale Zeit in `PlayerData.finalTime`

---

## 🔄 Datenfluss

```
1. NameInput
   └─> LeaderboardManager.RegisterPlayer("SpielerName")
       └─> Name wird registriert (eindeutig!)

2. GameSetUp (Spiel läuft)
   └─> Timer läuft
   └─> Alle Müllstücke gesammelt
       └─> PlayerData.finalTime = Zeit
       └─> Szene wechselt zu "Leaderboard"

3. Leaderboard (Szene)
   └─> LeaderboardManager.AddScore(PlayerData.playerName, PlayerData.finalTime)
       └─> Zeit wird gespeichert (nur wenn besser als vorherige)
   └─> LeaderboardManager.GetLeaderboard(10)
       └─> Zeigt Top 10 an
```

---

## 🚀 Migration zu Datenbank (WebGL)

### Architektur-Überlegung

Für WebGL-Projekte gibt es **zwei Ansätze**:

#### **Option 1: Client-seitig (LocalStorage)**
- ✅ Einfach zu implementieren
- ✅ Kein Server nötig
- ❌ Daten nur lokal (nicht zwischen Geräten)
- ❌ Kann manipuliert werden

#### **Option 2: Server-seitig (Datenbank) - EMPFOHLEN**
- ✅ Daten persistent und sicher
- ✅ Globales Leaderboard (alle Spieler)
- ✅ Kann nicht manipuliert werden
- ❌ Server nötig

---

## 📡 Schritt-für-Schritt: Datenbank-Integration

### Schritt 1: Server-Setup (Backend)

**Empfohlene Technologien:**
- **Backend:** Node.js + Express, PHP, Python (Flask/FastAPI)
- **Datenbank:** MySQL, PostgreSQL, MongoDB
- **API:** REST API (JSON)

**Beispiel: Node.js + Express + MySQL**

```javascript
// server.js
const express = require('express');
const mysql = require('mysql2');
const cors = require('cors');

const app = express();
app.use(cors());
app.use(express.json());

// Datenbank-Verbindung
const db = mysql.createConnection({
  host: 'localhost',
  user: 'root',
  password: 'password',
  database: 'clashofcleans'
});

// API-Endpunkte
app.post('/api/register-player', (req, res) => {
  const { playerName } = req.body;
  // Prüfe ob Name existiert
  // Füge Spieler hinzu
});

app.post('/api/add-score', (req, res) => {
  const { playerName, time } = req.body;
  // Füge/Update Score
});

app.get('/api/leaderboard', (req, res) => {
  // Hole Top 10 Scores
});

app.listen(3000);
```

**Datenbank-Schema:**
```sql
CREATE TABLE players (
  id INT AUTO_INCREMENT PRIMARY KEY,
  player_name VARCHAR(50) UNIQUE NOT NULL,
  best_time FLOAT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

---

### Schritt 2: Unity-Seite anpassen

#### 2.1 Erstelle `LeaderboardAPIClient.cs`

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class LeaderboardAPIClient : MonoBehaviour
{
    private const string API_BASE_URL = "https://deine-website.com/api";
    
    // Callback für asynchrone Operationen
    public delegate void OnSuccessCallback(string response);
    public delegate void OnErrorCallback(string error);
    
    // Prüft ob Name verfügbar ist
    public IEnumerator CheckNameAvailable(string playerName, 
        OnSuccessCallback onSuccess, OnErrorCallback onError)
    {
        string url = $"{API_BASE_URL}/check-name?name={UnityWebRequest.EscapeURL(playerName)}";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    
    // Registriert einen Spieler
    public IEnumerator RegisterPlayer(string playerName,
        OnSuccessCallback onSuccess, OnErrorCallback onError)
    {
        string url = $"{API_BASE_URL}/register-player";
        string jsonData = $"{{\"playerName\":\"{playerName}\"}}";
        
        using (UnityWebRequest request = UnityWebRequest.Post(url, jsonData, "application/json"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    
    // Fügt Score hinzu
    public IEnumerator AddScore(string playerName, float time,
        OnSuccessCallback onSuccess, OnErrorCallback onError)
    {
        string url = $"{API_BASE_URL}/add-score";
        string jsonData = $"{{\"playerName\":\"{playerName}\",\"time\":{time}}}";
        
        using (UnityWebRequest request = UnityWebRequest.Post(url, jsonData, "application/json"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    
    // Holt Leaderboard
    public IEnumerator GetLeaderboard(int maxEntries,
        OnSuccessCallback onSuccess, OnErrorCallback onError)
    {
        string url = $"{API_BASE_URL}/leaderboard?limit={maxEntries}";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}
```

#### 2.2 Erweitere `LeaderboardManager.cs`

**Füge diese Methoden hinzu:**

```csharp
// Füge am Anfang hinzu:
private static LeaderboardAPIClient apiClient;
private static bool useDatabase = false; // Toggle zwischen Local/Server

// Initialisierung (in einem MonoBehaviour)
public static void Initialize(LeaderboardAPIClient client, bool useDB)
{
    apiClient = client;
    useDatabase = useDB;
}

// Asynchrone Version von IsNameTaken
public static IEnumerator IsNameTakenAsync(string playerName, 
    System.Action<bool> callback)
{
    if (useDatabase && apiClient != null)
    {
        bool isTaken = false;
        yield return apiClient.CheckNameAvailable(playerName,
            (response) => {
                // Parse JSON response
                var data = JsonUtility.FromJson<NameCheckResponse>(response);
                isTaken = data.isTaken;
            },
            (error) => {
                Debug.LogError($"API Error: {error}");
                isTaken = false; // Fallback
            });
        callback(isTaken);
    }
    else
    {
        // Fallback zu lokaler Version
        callback(IsNameTaken(playerName));
    }
}

// Asynchrone Version von AddScore
public static IEnumerator AddScoreAsync(string playerName, float time,
    System.Action<bool> callback)
{
    if (useDatabase && apiClient != null)
    {
        bool success = false;
        yield return apiClient.AddScore(playerName, time,
            (response) => {
                success = true;
                // Optional: Lokale Daten aktualisieren
                AddScore(playerName, time); // Fallback
            },
            (error) => {
                Debug.LogError($"API Error: {error}");
                // Fallback zu lokaler Version
                AddScore(playerName, time);
            });
        callback(success);
    }
    else
    {
        AddScore(playerName, time);
        callback(true);
    }
}

// Asynchrone Version von GetLeaderboard
public static IEnumerator GetLeaderboardAsync(int maxEntries,
    System.Action<List<LeaderboardEntry>> callback)
{
    if (useDatabase && apiClient != null)
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
        yield return apiClient.GetLeaderboard(maxEntries,
            (response) => {
                // Parse JSON response
                var data = JsonUtility.FromJson<LeaderboardDataResponse>(response);
                entries = data.entries;
            },
            (error) => {
                Debug.LogError($"API Error: {error}");
                // Fallback zu lokaler Version
                entries = GetLeaderboard(maxEntries);
            });
        callback(entries);
    }
    else
    {
        callback(GetLeaderboard(maxEntries));
    }
}
```

#### 2.3 Passe `NameInput.cs` an

```csharp
// Ersetze die RegisterPlayer-Logik:
private IEnumerator RegisterPlayerAsync(string playerName)
{
    bool nameAvailable = false;
    
    // Prüfe Name über API
    yield return LeaderboardManager.IsNameTakenAsync(playerName, 
        (isTaken) => {
            nameAvailable = !isTaken;
        });
    
    if (!nameAvailable)
    {
        ShowError("This name is already taken!");
        yield break;
    }
    
    // Registriere Spieler
    bool success = false;
    yield return LeaderboardManager.RegisterPlayerAsync(playerName,
        (result) => {
            success = result;
        });
    
    if (success)
    {
        PlayerData.playerName = playerName;
        SceneManager.LoadScene("SkinMenu");
    }
    else
    {
        ShowError("Could not register name. Please try again.");
    }
}
```

#### 2.4 Passe `Leaderboard.cs` an

```csharp
private IEnumerator LoadLeaderboardAsync()
{
    List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    
    // Hole Leaderboard von Server
    yield return LeaderboardManager.GetLeaderboardAsync(maxEntries,
        (result) => {
            entries = result;
        });
    
    // Zeige Leaderboard an
    DisplayLeaderboard(entries);
}

// Füge Score hinzu (asynchron)
private IEnumerator AddScoreAsync(string playerName, float time)
{
    yield return LeaderboardManager.AddScoreAsync(playerName, time,
        (success) => {
            if (success)
            {
                StartCoroutine(LoadLeaderboardAsync());
            }
        });
}
```

---

## 🔧 Konfiguration

### Unity-Seite aktivieren:

1. **Erstelle ein GameObject** in der ersten Szene (z.B. "APIManager")
2. **Füge `LeaderboardAPIClient` Script hinzu**
3. **Erstelle ein Initialisierungs-Script:**

```csharp
public class GameInitializer : MonoBehaviour
{
    [SerializeField] private LeaderboardAPIClient apiClient;
    [SerializeField] private bool useDatabase = false; // Toggle hier!
    
    void Start()
    {
        LeaderboardManager.Initialize(apiClient, useDatabase);
    }
}
```

4. **Setze `useDatabase = true`** wenn Server bereit ist

---

## 📝 Zusammenfassung

### Aktueller Stand (Session-basiert):
- ✅ Namen werden nur während Session gespeichert
- ✅ Eindeutige Namen werden erzwungen
- ✅ Beste Zeiten werden gespeichert
- ✅ Daten gehen beim Neustart verloren

### Nach Datenbank-Migration:
- ✅ Namen werden persistent gespeichert
- ✅ Globales Leaderboard (alle Spieler weltweit)
- ✅ Daten bleiben erhalten
- ✅ Manipulation nicht möglich

### Migration-Schritte:
1. Server mit REST API erstellen
2. `LeaderboardAPIClient.cs` erstellen
3. `LeaderboardManager` erweitern (Async-Methoden)
4. `NameInput.cs` und `Leaderboard.cs` anpassen
5. Toggle `useDatabase = true` setzen

---

## 🎯 Empfehlung für WebGL

**Für Produktion:**
- Verwende **Server-seitige Datenbank** (MySQL/PostgreSQL)
- REST API für Kommunikation
- HTTPS für Sicherheit
- Rate Limiting gegen Spam

**Für Entwicklung:**
- Aktuelle Session-basierte Lösung ist perfekt
- Einfach zu testen
- Kein Server nötig

