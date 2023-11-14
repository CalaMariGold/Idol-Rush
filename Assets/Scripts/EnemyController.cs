using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        Vector2 direction = (player.position - transform.position).normalized; // Calculate direction towards the player
        rb.velocity = direction * speed; // Move enemy towards the player
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("hit");
            int damage = 10;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

}
