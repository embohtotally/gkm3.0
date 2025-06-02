using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject projectilePrefab;    
    public Transform player;               
    public float fireRate = 1f;            
    public float projectileSpeed = 5f;     
    public float shootingRange = 10f;      
    public int health = 100;               
    public float dieAnimationDuration = 1.5f;  

    private float nextFireTime = 0f;       
    private Animator animator;             
    private bool isDead = false;           
    private Vector2 lastDirection;         

    void Start()
    {
        animator = GetComponent<Animator>();  // Mengambil Animator pada GameObject
    }

    void Update()
    {
        if (isDead) return;

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= shootingRange && Time.time >= nextFireTime)
            {
                StartCoroutine(ShootWithAnimation());
                nextFireTime = Time.time + 1f / fireRate; 
            }

            // Tentukan arah terakhir musuh menghadap berdasarkan posisi pemain
            lastDirection = (player.position - transform.position).normalized;
        }
    }

    IEnumerator ShootWithAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");  
        }

        yield return new WaitForSeconds(0.5f); // Waktu tunggu untuk animasi serangan
        FireProjectile();  
    }

    void FireProjectile()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
            else
            {
                Rigidbody rb3D = projectile.GetComponent<Rigidbody>();
                if (rb3D != null)
                {
                    rb3D.velocity = direction * projectileSpeed;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0)
        {
            Die();  // Jika health habis, panggil fungsi Die
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetFloat("Horizontal", lastDirection.x);
            animator.SetFloat("Vertical", lastDirection.y);
            animator.SetTrigger("Die");  // Memicu blend tree animasi kematian
        }

        // Nonaktifkan collider supaya musuh tidak bisa berinteraksi
        Collider2D col2D = GetComponent<Collider2D>();
        if (col2D != null) col2D.enabled = false;

        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(dieAnimationDuration);  // Tunggu sampai animasi selesai
        Destroy(gameObject);  // Hancurkan GameObject
    }
}