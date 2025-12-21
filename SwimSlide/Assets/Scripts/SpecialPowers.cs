using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPowers : MonoBehaviour
{
    [SerializeField]
    public int health;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x <= -3f) Destroy(this.gameObject);
        if(rb!=null) rb.velocity = Vector3.left * SpawnItems.obstacleSpeed;
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            Debug.Log("power destroy!");
            Destroy(this.gameObject);
        }
    }
}
