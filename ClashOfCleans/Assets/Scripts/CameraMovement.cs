using UnityEngine;
public class CameraMovement : MonoBehaviour
{
    public GameObject player;
    private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        if (player){
            Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}