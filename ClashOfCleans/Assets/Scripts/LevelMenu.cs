using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    [SerializeField] private Button level1Btn;
    [SerializeField] private Button level2Btn;

    void Start()
    {
        level1Btn.onClick.AddListener(() =>
        {
            PlayerData.sceneName = "1.Level";
            SceneManager.LoadScene("SkinMenu");
        });
        level2Btn.onClick.AddListener(() =>
        {
            PlayerData.sceneName = "2.Level";
            SceneManager.LoadScene("SkinMenu");
        });
    }
}
