using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Attacking")]
    [SerializeField] public float closeDamage = 2f;
    [SerializeField] public float closeAttackRange = 2f;
    [SerializeField] public float farAttackRange = 5f;
    [SerializeField] public float detectionRange = 10f;
    [SerializeField ]private float attackCooldown = 0.2f;
    [SerializeField] private AudioClip farAttackSound;
    [SerializeField] private AudioClip closeAttackSound;
    [Header("Speed & Health")]
    [SerializeField] public float moveSpeed = 2f;
    [SerializeField] public float maxHealth = 1f; // Максимальное здоровье
    [Header("IFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int flashesAmount;
    [Header("Path")]
    [SerializeField] private float pathfindingInterval = 0.1f; // Интервал проверки пути
    [Header("Shooting")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private GameObject[] projectiles;
    
    private SpriteRenderer spriteRenderer;
    private float currentHealth;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public LayerMask mineLayer; // Добавляем слой мин
    public GameObject projectilePrefab;
    private Transform player;
    private Vector3 currentDirection;
    private float pathfindingTimer;
    private bool isDead = false;
    private float lastAttackTime;
    private Animator animator;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PlayerBoat").transform;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth; // Устанавливаем текущее здоровье при создании объекта
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentDirection = Vector3.zero;
        pathfindingTimer = pathfindingInterval; // Устанавливаем таймер
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool mineInRange = CheckForMineInRange();

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= closeAttackRange)
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    CloseAttack();
                }
            }
            else if (distanceToPlayer <= farAttackRange)
            {
                if (Time.time - lastAttackTime >= attackCooldown && !mineInRange)
                {
                    FarAttack();
                }
            }
            else
            {
                if (pathfindingTimer <= 0)
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                    FollowPlayer(direction);
                    pathfindingTimer = pathfindingInterval; // Сброс таймера
                }
                else
                {
                    pathfindingTimer -= Time.deltaTime;
                }
                MoveInDirection(currentDirection);
            }
        }
    }

    private void CloseAttack()
    {
        SoundManager.instance.PlaySound(closeAttackSound);
        lastAttackTime = Time.time;
        animator.SetTrigger("CloseAttack");
        // Нанесение урона игроку, если он вблизи
        if (Vector3.Distance(transform.position, player.position) <= closeAttackRange) {CloseAttackDamaging();}
    }

    private void CloseAttackDamaging()
    {
        player.GetComponent<Health>().TakeDamage(closeDamage);
    }

    private bool IsPathClearForShooting(Vector3 targetPosition)
    {
        Vector3 shootingDirection = (targetPosition - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, shootingDirection, farAttackRange, obstacleLayer);
        return hit.collider == null;
    }

    private void FarAttack()
    {
        Vector3 targetPosition = player.position;
        if (IsPathClearForShooting(targetPosition))
        {
            SoundManager.instance.PlaySound(farAttackSound);
            lastAttackTime = Time.time;
            animator.SetTrigger("FarAttack");
            ShootAtTarget(targetPosition);
        }
        else
        {
            ApproachPlayer();
        }
    }

    private void ApproachPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        if (IsPathClear(direction))
        {
            MoveInDirection(direction);
        }
        else
        {
            Vector3 newDirection = FindClearPath(direction);
            if (newDirection != Vector3.zero)
            {
                MoveInDirection(newDirection);
            }
        }
    }

    private bool CheckForMineInRange()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, farAttackRange, mineLayer);
        foreach (var hitCollider in hitColliders)
        {
            FloatingMine mine = hitCollider.GetComponent<FloatingMine>();
            if (mine != null)
            {
                float distanceToPlayer = Vector3.Distance(player.position, mine.transform.position);
                float distanceToEnemy = Vector3.Distance(transform.position, mine.transform.position);
                // Проверяем, находится ли игрок в радиусе взрыва и враг вне радиуса взрыва
                if (distanceToPlayer <= mine.explosionRadius && distanceToEnemy + 1 > mine.explosionRadius && Time.time - lastAttackTime >= attackCooldown)
                {
                    FarAttackAtMine(mine);
                    return true;
                }
            }
        }
        return false;
    }

    private void ShootAtTarget(Vector3 targetPosition)
    {
        GameObject projectile = projectiles[FindProjectile()];
        projectile.transform.position = shootingPoint.position;
        projectile.SetActive(true);
        Vector3 shootingDirection = (targetPosition - transform.position).normalized;
        projectile.GetComponent<project>().setDirection(shootingDirection);
    }

    private int FindProjectile()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            if (!projectiles[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    private void FarAttackAtMine(FloatingMine mine)
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("FarAttack");
        ShootAtTarget(mine.transform.position);
    }

    private void FollowPlayer(Vector3 direction)
    {
        // Проверка наличия мин на пути
        if (IsPathClear(direction))
        {
            currentDirection = direction;
        }
        else
        {
            // Если путь не свободен, ищем обходной путь
            Vector3 newDirection = FindClearPath(direction);
            if (newDirection != Vector3.zero)
            {
                currentDirection = newDirection;
            }
        }
    }

    private bool IsPathClear(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, closeAttackRange, obstacleLayer);
        return hit.collider == null;
    }

    private Vector3 FindClearPath(Vector3 originalDirection)
    {
        Vector3[] directions = {
            Quaternion.Euler(0, 0, 45) * originalDirection,
            Quaternion.Euler(0, 0, -45) * originalDirection,
            Quaternion.Euler(0, 0, 90) * originalDirection,
            Quaternion.Euler(0, 0, -90) * originalDirection,
            Quaternion.Euler(0, 0, 135) * originalDirection,
            Quaternion.Euler(0, 0, -135) * originalDirection,
            Quaternion.Euler(0, 0, 180) * originalDirection
        };

        foreach (var direction in directions)
        {
            if (IsPathClear(direction))
            {
                return direction;
            }
        }

        // Если все направления заблокированы, возвращаем вектор нуля
        return Vector3.zero;
    }

    private void MoveInDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.up = direction;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        if (currentHealth > 0)
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(Invunerability());
        }
        else
        { 
            if (!isDead)
            {
                animator.SetTrigger("Die");
                isDead = true;
                Destroy(gameObject, 1f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Floating Mine"))
        {
            other.GetComponent<FloatingMine>().Explode();
        }
    }

    private IEnumerator Invunerability()
    {
        for (int i = 0; i < flashesAmount; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(iFramesDuration/(flashesAmount*2));
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(iFramesDuration/(flashesAmount*2));
        }
    }
}
