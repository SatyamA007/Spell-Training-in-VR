using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float movementSpeed = 5;
    [SerializeField] float jumpSpeed = 5;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask layer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right*x+transform.forward*z;
        move *= movementSpeed;

        rb.velocity = new Vector3(move.x,rb.velocity.y+move.y,move.z);

        if(Input.GetKeyDown("space")&&IsGrounded()){
            Jump();
        }
        
    }

    void Jump(){
        rb.velocity = new Vector3(rb.velocity.x,jumpSpeed,rb.velocity.z);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Enemy Head")){
            Destroy(other.transform.parent.gameObject);
            Destroy(other.gameObject);
            Jump();
        }
    }

    bool IsGrounded(){
        return Physics.CheckSphere(groundCheck.position, .1f, layer);
    }
}
