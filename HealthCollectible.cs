using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public int healthAmount = 1;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.ChangeHealth(healthAmount);
            Destroy(gameObject);
        }
    }
}
