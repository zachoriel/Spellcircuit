using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float currentHealth;
    [SerializeField] private float maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
        if (currentHealth <= 0f)
        {
            // ToDo: Make this a little more graceful?
            Destroy(gameObject);
        }
    }
}
