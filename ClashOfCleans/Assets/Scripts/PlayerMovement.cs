using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveDir = Vector2.zero;

    public void SetMoveDirection(Vector2 dir) => moveDir = dir;
    public void StopMove() => moveDir = Vector2.zero;

    private void FixedUpdate()
    {
        if (rb2d != null)
            rb2d.linearVelocity = moveDir * moveSpeed;
    }
}

