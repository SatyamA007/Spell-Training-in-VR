using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    bool dead = false;
   private void OnCollisionEnter(Collision other) {
       if(other.gameObject.CompareTag("Enemy")){

           Die();
       }
   }

    private void Update() {
        if(transform.position.y<-1){
            Die();
        }
    }
   void Die(){
       if(dead) return;
       dead = true;
       GetComponent<Rigidbody>().isKinematic = true;
       GetComponent<MeshRenderer>().enabled = false;
       GetComponent<PlayerMovement>().enabled = false;
       Invoke("ReloadLevel",1.3f);
   }

   void ReloadLevel(){
       SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }
}
