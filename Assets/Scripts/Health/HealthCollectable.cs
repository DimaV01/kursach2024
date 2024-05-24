using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    [SerializeField] private float healthValue;
    [SerializeField] private AudioClip pickSound;
    private Transform player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBoat" && player.GetComponent<Health>().currentHealth < player.GetComponent<Health>().startingHealth)
        {
            SoundManager.instance.PlaySound(pickSound);
            collision.GetComponent<Health>().AddHealth(healthValue);
            gameObject.SetActive(false);
        }
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PlayerBoat").transform;
    }

}
