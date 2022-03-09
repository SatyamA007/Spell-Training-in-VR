using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Spell : MonoBehaviour
{
    // Range of Raycast
    public float range = 20f;
    
    // Device through which the player views the world
    public Camera fpsCam;
    
    // Particle system is a component that simulates fluid entities such as 
    // liquids, clouds and flames by generating and animating large number
    // of small 2D images in the scene. 
    public ParticleSystem wandFlash;

    // Game object defined to produce effect of impact  
    // In our case ImpactEffect is an Impact Flash 
    // which is essentially a Particle System
    public GameObject impactEffect;

    // Game object defined to produce effect of impact  
    // In our case DestroyEffect is a Destroy Particle System 
    public GameObject destroyEffect;

    // List of Monster game objects to be summoned by wand actions
    // This list is initialized before the start of the game
    // Monsters are added to the list by drag & drop from the Prefabs
    public GameObject[] monsterList;

    // Monster index to iterate list of monsters
    int monsterIndex=0;

    // delay in rendering actions
    public float delay = 1.3f;

    // tolerance limit
    // tolerance defines closeness to a game object
    // In our case the closeness is compared to the position 
    // of summoned monsters
    public float tolerance = 1f;

    // Structure used to get information back from a Raycast
    // Defined globally so as to provide easy access to all methods
    RaycastHit hit;

    // Game Object to provide reference to closest monster to 
    // a Raycast hit point
    GameObject bestTarget = null;

    // Vector defining the scaling action on a monster
    public Vector3 scaleChange = new Vector3(0.2f, 0.2f, 0.2f);

    // Vector defining the position change action on a monster
    public Vector3 positionChange = new Vector3(1f, 0, 1f);

    // Method to be invoked when wand is fired
    public void Shoot(int numberDetected)
    {
        // Play the wand flash particle system in the beginning
        wandFlash.Play();
        
        // Assigning monster index to the number detected via OpenCV
        monsterIndex = numberDetected;

        // Error handling
        // Default monster index set to 1
        if( monsterIndex >= monsterList.Length || monsterIndex < 0) 
        {
            monsterIndex = 1;
        } 
        
        Debug.Log("Invoking wand with spell number: " + monsterIndex);

        // Fire a raycast from the wand in the direction of camera with the range defined
        bool rayIntersection = Physics.Raycast( fpsCam.transform.position, fpsCam.transform.forward, out hit, range);

        // Retrieve point information where the Raycast hit
        Debug.Log("Raycast hit point: " + hit.point);

        // Check if the Raycast Hit point is close to an existing monster
        if( CheckIfMonsterHit(hit.point) )
        {
            // Check the number detected and take an 
            // action to manipulate monster based on it

            // Transform position of game object
            if(monsterIndex < 2) {
                Debug.Log("Position before " + bestTarget.transform.position);
                bestTarget.transform.position += positionChange;             
                Debug.Log("Position after " + bestTarget.transform.position);

                Debug.Log("Changed position of " + bestTarget.transform.name + " monster");
            }
            // Destory monster game object
            else if(monsterIndex < 5) {
                // Destroy the game object after a specific delay
                Destroy(bestTarget, delay);

                Debug.Log("Monster " + bestTarget.transform.name + " destroyed");
            }
            // Scale monster game object
            else if(monsterIndex < 8) {
                bestTarget.transform.localScale += scaleChange;             

                Debug.Log("Scale " + bestTarget.transform.name + " monster");
            }
            else {
                Destroy(bestTarget, 0);
                
                bestTarget = Instantiate( monsterList[(monsterIndex+1)%monsterList.Length], hit.point, Quaternion.Euler(0, -hit.transform.rotation.y, 0) );
                bestTarget.transform.LookAt(gameObject.transform);

                Debug.Log("Transform " + bestTarget.transform.name + " monster into something else");
            }

            // Generate a Particle System game object to simulate manipulation action 
            GameObject impactPS = Instantiate( destroyEffect, hit.point, Quaternion.LookRotation(hit.normal) );
            
            // Destroy the game object after a specific delay
            Destroy(impactPS, delay);
        }
        // If the Raycast Hit point is far away from all instantiated monsters 
        else {
            // Spawn a new monster creature
            Invoke("SpawnMonster", delay);

            // Generate a Particle System game object to simulate the generation of monster 
            GameObject impactPS = Instantiate( impactEffect, hit.point, Quaternion.LookRotation(hit.normal) );
            
            // Destroy the game object after a specific delay
            Destroy(impactPS, delay);
        }

        // Disable the picture-in-picture mode once a new monster is summoned
        RawImage image = GameObject.FindGameObjectWithTag("PIP").GetComponent<RawImage>();
        image.enabled = false;
    }

    // Check if Raycast point hit an existing spawned monster in the scene
    public bool CheckIfMonsterHit(Vector3 rayCastHitPoint) 
    {
        // Get the list of all instantiated monsters/creatures in the game
        // find all GameObjects with tag 'monster'
        GameObject[] instantiatedMonsterList = (GameObject[])GameObject.FindGameObjectsWithTag("monster");

        Debug.Log("Total monsters summoned in scene so far: " + instantiatedMonsterList.Length);

        if(instantiatedMonsterList.Length > 0) 
        {
            // Find the distance of the closest monster to the Raycast hit point
            float closestMonsterDistance = GetClosestMonsterDistance(rayCastHitPoint, instantiatedMonsterList, out bestTarget);

            if(closestMonsterDistance < tolerance)
            {
                Debug.Log("Raycast hit monster: " + bestTarget.transform.name);
                return true;
            }
            else {
                Debug.Log("Closest monster farther than tolerance, separation distance : " + closestMonsterDistance);
                return false;
            }
        }

        Debug.Log("No monster instantiated so far, hence not checking any distance");
        return false;
    }


    // Find the closest monster to the RaycastHit point
    public bool CheckAndDestroyMonster(Vector3 rayCastHitPoint) 
    {
        // Get the list of all instantiated monsters/creatures in the game
        // find all GameObjects with tag 'monster'
        GameObject[] instantiatedMonsterList = (GameObject[])GameObject.FindGameObjectsWithTag("monster");

        Debug.Log("Total monsters summoned in scene so far: " + instantiatedMonsterList.Length);

        if(instantiatedMonsterList.Length > 0) 
        {
            
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

    // Find the monster that is closest to the Hit Raycast
    public float GetClosestMonsterDistance(Vector3 rayHitPosition, GameObject[] instantiatedMonsterList, out GameObject bestTarget)
    {
        bestTarget = null;
        float closestDistance = Mathf.Infinity;
        Debug.Log("Finding the closest monster to Raycast Hot point ...");

        if(instantiatedMonsterList.Length > 0) 
        {
            foreach (GameObject potentialTarget in instantiatedMonsterList)
            {   
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
                Debug.Log("Closest monster name: " + bestTarget.transform.name);
            }
        }
        
        return closestDistance;
    }

    // Method to summon a new monster
    public void SpawnMonster()
    {
        // summon a new moster from the monster list
        if( monsterList.Length != 0 )
        {
            // 
            GameObject monsterGO = Instantiate( monsterList[monsterIndex], hit.point, Quaternion.Euler(0, -hit.transform.rotation.y, 0) );
            
            // tag the newly instantiated createure as 'monster'
            monsterGO.tag = "monster";
            monsterGO.transform.LookAt(gameObject.transform);

            Debug.Log("Summoned monster: " + monsterGO.transform.name);
        }
    }
}