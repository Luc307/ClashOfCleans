using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Button rightBtn;
    private Button leftBtn;
    private Rigidbody2D rb2d;
    private Animator animator;

    private float moveSpeed = 7.5f;
    private float jumpForce = 750f;
    private float bounceVelocity = 52f; // Neue Variable für konsistente Bounce-Höhe
    private bool grounded = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        grounded = true;

        if (collision.gameObject.CompareTag("Jump")){
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocityX, bounceVelocity);
        }
        else{
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocityX, 0);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {grounded = false;}
    private void OnTriggerStay2D(Collider2D collision) {grounded = true;}


    private void Move(Vector3 direction) {
        animator.SetBool("isRunning", true);

        rb2d.linearVelocity = new Vector2(direction.x * moveSpeed, rb2d.linearVelocity.y);

        if(direction.x > 0){
            transform.localScale = new Vector3(5, 5, 5);
        }
        else if(direction.x < 0){
            transform.localScale = new Vector3(-5, 5, 5);
        }
    }
    private void Awake(){
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        rightBtn = GameObject.Find("Canvas/Right").GetComponent<Button>();
        leftBtn = GameObject.Find("Canvas/Left").GetComponent<Button>();
    }
    private void Start(){
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
        rb2d.linearVelocity = new Vector2(0, rb2d.linearVelocity.y);
        if (rightBtn.GetComponent<HoldButton>().isHeld || Input.GetKey(KeyCode.D))
        {
            Move(Vector3.right);
        }
        else if (leftBtn.GetComponent<HoldButton>().isHeld || Input.GetKey(KeyCode.A))
        {
            Move(Vector3.left);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        if (Input.GetKeyDown(KeyCode.Space)){
            if(grounded){
                rb2d.AddForce(Vector2.up * jumpForce);
            } 
        }

    }

}