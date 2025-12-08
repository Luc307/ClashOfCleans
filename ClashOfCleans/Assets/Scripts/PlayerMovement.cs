using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Button rightBtn;
    private Button leftBtn;
    private Rigidbody2D rb2d;
    private Animator animator;
    private float moveSpeed = 17.5f;
    private float jumpForce = 750f;
    private bool grounded = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        grounded = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        grounded = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        grounded = true;
    }

    private void Move(Vector3 direction)
    {
        animator.SetBool("isRunning", true);
        rb2d.MovePosition(transform.position + direction * Time.deltaTime * moveSpeed);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        rightBtn = GameObject.Find("Canvas/Right").GetComponent<Button>();
        leftBtn = GameObject.Find("Canvas/Left").GetComponent<Button>();
        GameObject.Find("Canvas/Up").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (grounded)
            {
                rb2d.AddForce(Vector2.up * jumpForce);
            }
        });
    }
    private void Update()
    {
        if (rightBtn.GetComponent<HoldButton>().isHeld)
        {
            Move(Vector3.right);
        }
        else if (leftBtn.GetComponent<HoldButton>().isHeld)
        {
            Move(Vector3.left);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }
}