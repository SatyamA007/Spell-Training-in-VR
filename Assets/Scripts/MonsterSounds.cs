using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSounds : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip voice;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other) {
       if(other.gameObject.CompareTag("Player")){
           audioSource.PlayOneShot(voice, 0.2f);
       }
   }
}
