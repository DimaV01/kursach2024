using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set;}
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        
        audioSource = GetComponent<AudioSource>();
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this){Destroy(gameObject);}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
