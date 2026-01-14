using TMPro;
using Unity.VisualScripting;
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
        inputField.text = PlayerData.playerName;
        Debug.Log($"Current Player Name: {PlayerData.playerName}");

        playBtn.onClick.AddListener(() =>
        {
            string playerName = inputField.text.Trim();
            
            // Validate name
            if (string.IsNullOrEmpty(playerName))
            {
                ShowError("Please enter a name!");
                return;
            }

            // Register the player name locally
            PlayerData.playerName = playerName.FirstCharacterToUpper();
            HideError();
            SceneManager.LoadScene("LevelMenu");
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
