using UnityEngine;

public class TrashCollectible : MonoBehaviour
{
    private bool isCollected = false; // Neue Flag, um Mehrfach-Sammlung zu verhindern

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
        // Check if the player collected this trash and es noch nicht gesammelt wurde
        if (!isCollected && collision.CompareTag("Player"))
        {
            isCollected = true; // Markiere als gesammelt
            
            // Notify the game that trash was collected
            GameSetUp game = GameObject.Find("ScriptExecuter").GetComponent<GameSetUp>();
            if (game != null)
            {
                game.CollectTrash();
            }
            
            Destroy(gameObject);
        }
    }
}