using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class project : MonoBehaviour
{
    [SerializeField] private float damage = 10f; // Урон, наносимый снарядом
    [SerializeField] private LayerMask damageableLayers; // Слои, по которым снаряд может наносить урон
    [SerializeField] private float speed = 20;
    private bool hit;
    private CircleCollider2D circleCollider2D;
    private Animator animator;
    private float lifetime;

    private void Awake(){
        animator = GetComponent<Animator>();
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Update(){
        if (hit) return;
        Vector3 movingSpeed = speed * Time.deltaTime * direction; // Это уже Vector3
        transform.position += movingSpeed; // Корректное прибавление Vector3 к Vector3
        lifetime += Time.deltaTime;
        if (lifetime > 5) Deactivate();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit) return;

        // Проверка на попадание по мине
        if (collision.CompareTag("Floating Mine"))
        {
            collision.GetComponent<FloatingMine>().Explode();
            Deactivate();
            return;
        }

        // Проверка на попадание по объекту, которому можно нанести урон
        if ((damageableLayers.value & (1 << collision.gameObject.layer)) > 0)
        {
            Health health = collision.GetComponent<Health>();
            Enemy enemyHealth = collision.GetComponent<Enemy>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            else if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            
        }
        hit = true;
        circleCollider2D.enabled = false;
        animator.SetTrigger("explode");
    }

    private Vector3 direction;
    public void setDirection(Vector3 _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        circleCollider2D.enabled = true;

        // Установка угла поворота так, чтобы "верх" снаряда был направлен к игроку
        transform.up = -direction;  // Изменяем ориентацию с "правого" на "верхний" край
    }


    private void Deactivate(){
        gameObject.SetActive(false);
    }
}
