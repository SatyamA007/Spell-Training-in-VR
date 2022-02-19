using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] GameObject[] waypoints;
    int currentWaypoint=0;
        // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position,waypoints[currentWaypoint].transform.position)<0.1f){
            currentWaypoint++;
            currentWaypoint = currentWaypoint >=waypoints.Length?0:currentWaypoint;            
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].transform.position, 1f*Time.deltaTime);
    }
}
