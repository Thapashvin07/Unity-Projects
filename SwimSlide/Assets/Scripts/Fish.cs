using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish: MonoBehaviour
{
    [SerializeField]
    public float health;
    [SerializeField]
    float movingSpeed;
    Rigidbody2D rb;
    [SerializeField]
    GameObject explosion;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if(transform.position.x <= -3f) Destroy(this.gameObject);
        if(rb!=null) rb.velocity = Vector3.left * SpawnItems.obstacleSpeed;
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.GetComponent<PlayerMovement>() != null) 
        {
            Debug.Log("fish destroy!");
            Destroy(this.gameObject);
        }
    }
}
