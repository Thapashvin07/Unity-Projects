using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float speed;
    [SerializeField]
    public Vector2 direction;
    Rigidbody2D rb;
    [SerializeField]
    public float health = 100f;
    Vector3 startPos,endPos;
    public Vector2 targetPos;
    public bool isDamageable;//this is for animator purpose when hitting border
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(0.05f, 0.5f));
        transform.position = worldPos;
        targetPos = transform.position;
        animator = this.gameObject.GetComponent<Animator>();
        speed = 1f;
        #if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
        #else
            Debug.unityLogger.logEnabled = false;
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) SpawnItems.spawnItems.ExitGame();
        Debug.Log("up input:"+Input.GetAxisRaw("Vertical"));
        if(health <= 0)
        {
            Debug.Log("Game over!");
            SpawnItems.spawnItems.ShowGameOver();
        }
        else SpawnItems.spawnItems.UpdateHealthBar(health);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, SpawnItems.playerSpeed * Time.deltaTime);
        if(rb==null || animator==null) return;
        animator.SetBool("isDamageable",isDamageable);
        float InputY = 0;
        // InputY = Input.GetAxisRaw("Vertical");
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) InputY = 1;
        else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) InputY = -1;
        direction = new Vector2(0,InputY).normalized;
        Move(direction*speed);
        Debug.Log("dragdist:"+Screen.height*15/100f);
        if(Input.touchCount<=0)
        {
            direction = Vector2.zero;
            return;
        }
        if(Input.GetTouch(0).phase==TouchPhase.Began)
        {
            startPos = Input.GetTouch(0).position;
            Debug.Log("startPos:"+startPos.normalized);
        }
        if(Input.touchCount>0 && Input.GetTouch(0).phase==TouchPhase.Ended)
        {
            endPos = Input.GetTouch(0).position;
            Debug.Log("endPos:"+endPos.normalized);
            if(MathF.Abs(startPos.y-endPos.y)<150f) {
                direction = Vector2.zero;
                Debug.Log("returning");
                return;
            }
            if(endPos.y > startPos.y)
            {
                Debug.Log("I should go up!"+(endPos.y-startPos.y));
                direction = Vector2.up;
                
                // rb.AddForce(direction*speed);
            }
            else if(endPos.y < startPos.y)
            {
                Debug.Log("I should go down!"+(startPos.y-endPos.y));
                direction = Vector2.down;
                // rb.AddForce(direction*speed);
            }
            Debug.Log("direction value:"+direction);
            // rb.velocity = direction*speed;
            Move(direction*speed);
        }
        // Debug.Log("keyboard input is:"+InputY);
    }
    public void Move (Vector2 moveDirection)
    {
        targetPos += moveDirection;
    }
    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("oncollision");
        if(other.gameObject.GetComponent<Fish>())
        {
            health =Mathf.Clamp(health - other.gameObject.GetComponent<Fish>().health,0,100);
            Debug.Log("collided go is:"+other.gameObject.name);
        }
        else if(other.gameObject.name == "Border")
        {
            Debug.Log("Display game over!");
            isDamageable = true;
            health = health - 5f;
        }
        else if(other.gameObject.GetComponent<SpecialPowers>())
        {
            health = Mathf.Clamp(health+other.gameObject.GetComponent<SpecialPowers>().health,0,100);
        }
    }
    private IEnumerator VelocityZero()
    {
        yield return new WaitForSeconds(0.3f);
        direction = Vector2.zero;
    }
}
