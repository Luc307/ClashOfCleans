using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI errorText;

    void Start()
    {   
        playBtn.onClick.AddListener(() =>
        {
            string playerName = inputField.text.Trim();
            
            // Validate name
            if (string.IsNullOrEmpty(playerName))
            {
                ShowError("Please enter a name!");
                return;
            }

            // Check if name is already taken
            if (LeaderboardManager.IsNameTaken(playerName))
            {
                ShowError("This name is already taken!");
                return;
            }

            // Register the player name
            if (LeaderboardManager.RegisterPlayer(playerName))
            {
                PlayerData.playerName = playerName;
                HideError();
                SceneManager.LoadScene("SkinMenu");
            }
            else
            {
                ShowError("Could not register name. Please try again.");
            }
        });

        // Also check on input change (optional - gives immediate feedback)
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnInputChanged);
        }
    }

    private void OnInputChanged(string value)
    {
        // Hide error when user starts typing
        if (!string.IsNullOrEmpty(value))
        {
            HideError();
        }
    }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"NameInput Error: {message}");
        }
    }

    private void HideError()
    {
        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }
    }
}
