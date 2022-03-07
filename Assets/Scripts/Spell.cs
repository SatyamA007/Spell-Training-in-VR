using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Spell : MonoBehaviour
{
    // Range of Raycast
    public float range = 100f;
    
    // Device through which the player views the world
    public Camera fpsCam;
    
    // Particle system is a component that simulates fluid entities such as 
    // liquids, clouds and flames by generating and animating large number
    // of small 2D images in the scene. 
    public ParticleSystem wandFlash;

    // Game object defined to produce effect of impact  
    // In our case ImpactEffect is a Impact Flash 
    public GameObject impactEffect;

    // Game object defined to produce effect of impact  
    // In our case ImpactEffect is a Impact Flash 
    public GameObject destroyEffect;

    // List of Monster game objects to be summoned by wand actions
    public GameObject[] monsterList;

    // Monster index to iterate list of monsters
    int monsterIndex=0;

    // delay in rendering actions
    public float delay = 1.3f;

    // tolerance limit
    public float tolerance = 2.5f;

    // Structure used to get information back from a Raycast
    RaycastHit hit;

    // Method to be invoked when wand is fired
    public void Shoot(int numberDetected)
    {
        // Play the wand flash particle system in the beginning
        wandFlash.Play();
        
        // Assigning monster index to the number detected via OpenCV
        monsterIndex = numberDetected;

        // Default monster index set to 1
        if( monsterIndex >= monsterList.Length) 
        {
            monsterIndex = 1;
        } 
        
        Debug.Log("Invoked wand with spell number: " + numberDetected);

        // Fire a raycast from the wand in the direction of camera with the range defined
        bool rayIntersection = Physics.Raycast( fpsCam.transform.position, fpsCam.transform.forward, out hit, range);

        Debug.Log("Raycast hit location: " + hit.transform.name);
        Debug.Log("Raycast hit point: " + hit.point);

        if( CheckAndDestroyMonster(hit.point) )
        {
            Debug.Log("Successfully destroyed monster, put animations here.");
        }
        else {
            // SpawnMonster(hit, delay);
            Invoke("SpawnMonster", delay);

            // Generate a game object to simulate the generation of monster 
            GameObject impactPS = Instantiate( impactEffect, hit.point, Quaternion.LookRotation(hit.normal) );
            
            // Destroy the game object after a specific delay
            Destroy(impactPS, delay);
        }

        // Disable the picture-in-picture mode once a new monster is summoned
        RawImage image = GameObject.FindGameObjectWithTag("PIP").GetComponent<RawImage>();
        image.enabled = false;
    }

    // Find the closest monster to the RaycastHit point
    public bool CheckAndDestroyMonster(Vector3 rayCastHitPoint) 
    {
        GameObject[] instantiatedMonsterList = (GameObject[])GameObject.FindGameObjectsWithTag("monster");

        Debug.Log("Total monsters summoned in scene: " + instantiatedMonsterList.Length);

        if(instantiatedMonsterList.Length > 0) 
        {
            GameObject bestTarget = null;
            float closestMonsterDistance = GetClosestMonsterDistance(rayCastHitPoint, instantiatedMonsterList, out bestTarget);

            if(closestMonsterDistance < tolerance)
            {
                // Generate a game object to simulate the generation of monster 
                GameObject impactPS = Instantiate( destroyEffect, bestTarget.transform.position, Quaternion.Euler(0, -hit.transform.rotation.y, 0) );
                
                // Destroy the game object after a specific delay
                Destroy(impactPS, delay);

                Destroy(bestTarget, delay);
                Debug.Log("Monster " + bestTarget.transform.name + " destroyed !");

                return true;
            }
            else {
                Debug.Log("Closest monster farther than tolerance, separation distance : " + closestMonsterDistance);
            }
        }

        return false;
    }

    public float GetClosestMonsterDistance(Vector3 rayHitPosition, GameObject[] instantiatedMonsterList, out GameObject bestTarget)
    {
        bestTarget = null;
        float closestDistance = Mathf.Infinity;
        Debug.Log("GetClosestMonsterDistance ...");

        if(instantiatedMonsterList.Length > 0) 
        {
            foreach (GameObject potentialTarget in instantiatedMonsterList)
            {   
                Debug.Log("Monster position: " + potentialTarget.transform.position);
                Debug.Log("Raycast hit point: " + rayHitPosition);

                Vector3 directionToTarget = potentialTarget.transform.position - rayHitPosition;
    
                float distanceToTarget = directionToTarget.magnitude;
        
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    bestTarget = potentialTarget;
                }
            }

            if(null != bestTarget)
            {
                Debug.Log("Target to be destroyed: " + bestTarget);
            }
        }
        
        return closestDistance;
    }

    // Method to summon a new monster
    public void SpawnMonster()
    {
        // WaitForSeconds(delay);

        // summon a new moster from the monster list
        if( monsterList.Length != 0 )
        {
            GameObject monsterGO = Instantiate( monsterList[monsterIndex], hit.point, Quaternion.Euler(0, -hit.transform.rotation.y, 0) );
            monsterGO.tag = "monster";
            monsterGO.transform.LookAt(gameObject.transform);
        }

        // // Disable the picture-in-picture mode once a new monster is summoned
        // RawImage image = GameObject.FindGameObjectWithTag("PIP").GetComponent<RawImage>();
        // image.enabled = false;
    }
}