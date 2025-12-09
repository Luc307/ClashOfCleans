using UnityEngine;

public class TrashCollectible : MonoBehaviour
{
    private void Awake()
    {
        // Automatically set the collider as a trigger if it exists
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collected this trash
        if (collision.CompareTag("Player"))
        {
            // Notify the game manager that trash was collected
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.CollectTrash();
            }
            
            // Make the trash disappear
            gameObject.SetActive(false);
        }
    }
}

