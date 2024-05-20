using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float startingHealth;
    public float currentHealth {get; private set;}
    private float lastHitTime; 
    private Animator animator;
    private bool dead;
    [Header("IFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int flashesAmount;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private bool canGetHit() {return Time.time - lastHitTime >= iFramesDuration;}
    public void AddHealth(float healingValue) {currentHealth = Mathf.Clamp(currentHealth + healingValue, 0, startingHealth);}
    public void TakeDamage(float _damage)
    {
        if (!canGetHit()) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        if (currentHealth > 0 && canGetHit())
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(Shake(0.5f, 0.15f));
            StartCoroutine(Invunerability());
        }
        else
        { 
            if (!dead)
            {
                animator.SetTrigger("Die");
                dead = true;
            }
            
        }
        lastHitTime = Time.time;
    }
    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        Vector3 startPosition = transform.localPosition;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = startPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPosition;
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

