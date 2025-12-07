using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private TMP_InputField inputField;

    void Start()
    {   
        playBtn.onClick.AddListener(() =>
        {
            PlayerData.playerName = inputField.text;
            SceneManager.LoadScene("SkinMenu");
        });
    }
}
