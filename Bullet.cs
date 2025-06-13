using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f;
    public int damage = 1;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Örnek:
            // collision.GetComponent<EnemyHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == groundLayer)
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == wallLayer)
        {
            Destroy(gameObject);
        }
    }
}
