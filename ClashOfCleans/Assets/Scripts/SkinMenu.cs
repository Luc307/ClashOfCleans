using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button returnBtn;
    [SerializeField] private Button playBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button prevBtn;
    [SerializeField] private GameObject skinsGameObject;

    [SerializeField] private Image MaskDude;
    [SerializeField] private Image NinjaFrog;
    [SerializeField] private Image PinkMan;
    [SerializeField] private Image VirtualGuy;
    private int currentIndex = 0;
    private string[] skins = { "MaskDude", "NinjaFrog", "PinkMan", "SpaceGuy" };

    private Vector3 transformVelocity = Vector3.zero;
    private float smoothTime = 0.15f;
    private Vector3 targetPosition;

    private void Awake()
    {
        targetPosition = skinsGameObject.transform.position;
        PlayerData.skinName = skins[currentIndex];
    }
    private void Start()
    {
        nextBtn.onClick.AddListener(() =>
        {
            if (currentIndex < 3)
            {
                targetPosition -= new Vector3(350, 0, 0);
                currentIndex++;
                PlayerData.skinName = skins[currentIndex];
            }
        });
        prevBtn.onClick.AddListener(() =>
        {
            if (currentIndex > 0)
            {
                targetPosition += new Vector3(350, 0, 0);
                currentIndex--;
                PlayerData.skinName = skins[currentIndex];
            }
        });

        playBtn.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(PlayerData.sceneName);
        });
        returnBtn.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelMenu");
        });
    }

    private void Update()
    {
        skinsGameObject.transform.position = Vector3.SmoothDamp(
            skinsGameObject.transform.position,
            targetPosition,
            ref transformVelocity,
            smoothTime
        );
    }
}