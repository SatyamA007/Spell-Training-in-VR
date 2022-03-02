using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public float range = 100f;
    public Camera fpsCam;
    public ParticleSystem wandFlash;
    public GameObject impactEffect;
    public GameObject[] monster;
    RaycastHit hit;
    int idx=0;
    void Update()
    {

    }

    public void Shoot(int i){
        wandFlash.Play();
        idx = i;
        if(idx>=monster.Length) idx = 0;
        Debug.Log("Called with spell number "+ i);
        
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)){
            Debug.Log(hit.transform.name);
            Invoke("SpawnMonster", 2);
        }
        GameObject impactPS = Instantiate(impactEffect, hit.point,Quaternion.LookRotation(hit.normal));
        Destroy(impactPS, 2);
    }

    public void SpawnMonster(){
        if(monster.Length!=0){
            GameObject monsterGO = Instantiate(monster[idx], hit.point,Quaternion.identity);
        }
    }

}
