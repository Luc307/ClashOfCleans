using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private GameObject MaskDude;
    [SerializeField] private GameObject NinjaFrog;
    [SerializeField] private GameObject PinkMan;
    [SerializeField] private GameObject SpaceGuy;
    private Dictionary<string, GameObject> skinDict;

    private void Awake()
    {
        skinDict = new Dictionary<string, GameObject>()
        {
            { "MaskDude", MaskDude },
            {"NinjaFrog", NinjaFrog},
            {"PinkMan", PinkMan},
            {"SpaceGuy", SpaceGuy }
        };
        PlayerData.player = skinDict[PlayerData.skinName];
    }
    private void Start()
    {
        Instantiate(PlayerData.player, new Vector3(0, 5, 0), Quaternion.identity, transform);
    }
}