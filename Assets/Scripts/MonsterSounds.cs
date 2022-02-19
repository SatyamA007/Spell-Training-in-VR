using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSounds : MonoBehaviour
{
    public AudioSource entry;
    public AudioSource roar;
    public AudioSource grunt;
    void Start()
    {
        roar.PlayDelayed(2);
        entry.Play();
    }

    private void OnTriggerStay(Collider other) {
                entry.Play();

    }
}
