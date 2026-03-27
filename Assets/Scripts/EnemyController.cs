using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int damage = 1;
    [SerializeField] private int scoreOnKill = 25;
    [SerializeField] private int health = 1;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private GameObject destroyEffectPrefab;

    private Transform playerTransform;
    private float lastAttackTime;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (playerTransform == null) return;

        Vector3 direction = (playerTransform.position - transform.position);
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance > attackRange)
        {
            direction.Normalize();
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else if (Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        lastAttackTime = Time.time;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(damage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
        }
    }

    public void OnHit()
    {
        health--;
        if (health <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreOnKill);
            }

            if (destroyEffectPrefab != null)
            {
                Instantiate(destroyEffectPrefab, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}
