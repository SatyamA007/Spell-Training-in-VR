using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject _creaturePreFab;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector3 mouseScreenPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

            if(Physics.Raycast(ray, out RaycastHit hitInfo)){
                SpawnCreatureAtPos(hitInfo.point);
            }
        }
    }

    void SpawnCreatureAtPos(Vector3 spawnPos){
        GameObject creature = Instantiate(_creaturePreFab, spawnPos, Quaternion.identity);
    }
}
