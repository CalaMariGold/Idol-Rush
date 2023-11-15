using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    [SerializeField]
    private int currentHealth;
    public Image healthBar;
    public Transform healthBarTransform;

    public int CurrentHealth { get { return currentHealth; } }
    public int MaxHealth { get { return maxHealth; } }


    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        healthBarTransform.position = transform.position + new Vector3(0, 1.3f, 0);

    }

    public void TakeDamage(int damage)
    {
        Debug.Log("took " + damage + " damage");
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died");
        gameObject.SetActive(false);
        healthBar.fillAmount = 0;
    }
}
