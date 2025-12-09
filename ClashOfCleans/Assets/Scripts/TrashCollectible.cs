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
            // Notify the game that trash was collected
            Game game = GameObject.Find("ScriptExecuter").GetComponent<Game>();
            if (game != null)
            {
                game.CollectTrash();
            }
            
            // Make the trash disappear
            gameObject.SetActive(false);
        }
    }
}