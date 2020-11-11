using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float currentHealth, maxHealth;
    
    public void DealDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    public virtual void Kill()
    {
        Destroy(gameObject);
    }
}
