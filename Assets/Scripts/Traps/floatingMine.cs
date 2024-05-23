using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMine : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float damage = 20f;
    public LayerMask playerLayer; // Убедитесь, что это поле настроено в инспекторе
    public LayerMask mineLayer; // Добавим новый слой для мин
    public LayerMask enemyLayer;
    private bool exploded = false;
    [SerializeField] private AudioClip explosionSound;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D other) {if (other.CompareTag("PlayerBoat") || other.CompareTag("Bullet") || other.CompareTag("EnemyBullet")) {Explode();}}
    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        // Играем анимацию взрыва
        SoundManager.instance.PlaySound(explosionSound);
        animator.SetTrigger("Explode");
        CameraShake.Instance.TriggerShake(0.2f);

        // Находим все объекты в радиусе взрыва
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer | enemyLayer);
        foreach (Collider2D hit in hitColliders)
        {
            // Наносим урон игроку
            Health playerHealth = hit.GetComponent<Health>();
            Enemy enemyHealth = hit.GetComponent<Enemy>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            else if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

        }

        // Проверка на наличие других мин в радиусе взрыва
        Collider2D[] minesInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius, mineLayer);
        foreach (Collider2D mineCollider in minesInRange)
        {
            FloatingMine mine = mineCollider.GetComponent<FloatingMine>();
            if (mine != null && mine != this && !mine.exploded)
            {
                StartCoroutine(DelayedExplode(mine, 0.1f));
            }
        }

        // Убираем объект после взрыва
        StartCoroutine(RemoveMine());
    }

    private IEnumerator DelayedExplode(FloatingMine mine, float delay)
    {
        yield return new WaitForSeconds(delay);
        mine.Explode();
    }

    IEnumerator RemoveMine()
    {
        yield return new WaitForSeconds(1f); // Дожидаемся окончания анимации
        Destroy(gameObject);
    }

    // Метод для визуализации радиуса взрыва в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    
}
