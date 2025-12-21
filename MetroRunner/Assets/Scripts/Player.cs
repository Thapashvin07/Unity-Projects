using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benjathemaker;
using UnityEngine.UI;
using System;
public class Player : MonoBehaviour
{
    Vector2 startPos, endPos, deltaPos;
    [SerializeField]
    Animator animator;
    Vector3 rootPos;
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    float speedFact;
    CharacterController controller;
    Vector3 moveDirection;
    public int currentLane = 1;//1 denotes middle
    public float groundDist = 0.35f;
    public float verticalVelocity;
    public float jumpForce = 5;
    public float gravity = -20f;
    public bool canProcessInput = false;//this can be made true when game start is done
    public GameObject tapGo;
    [SerializeField]
    int player_points, player_health;//resets when game starts
    [SerializeField]
    Text points_txt;
    [SerializeField]
    Image health_img;
    int milestone= 50;
    float fact = 0.2f;
    public int _player_points
    {
        get { return player_points; }
        set
        {
            player_points = value;
            if(player_points >= milestone)
            {
                if(milestone == 50)
                {
                    speedFact += fact;
                    fact = 0.3f;
                }
                else if (milestone >= 200) {
                    fact = 0.4f;
                    speedFact += fact;
                    GameMgr.gameMgr.obstacleProb = 0.45f;
                    GameMgr.gameMgr.collectableProb = 0.55f;
                }
                else if(milestone >= 100) {
                    speedFact += fact;
                    GameMgr.gameMgr.obstacleProb = 0.4f;
                    GameMgr.gameMgr.collectableProb = 0.6f;
                }
                milestone += 50;
                speedFact = Mathf.Clamp(speedFact,1,3);
            }
            points_txt.text = value.ToString();
            //Debug.Log("updated value:" + value);
        }
    }
    public int _player_health
    {
        get { return player_health; }
        set
        {
            player_health = value;
            if(player_health > 100) player_health = 100;
            health_img.fillAmount = player_health/100f;
            //Debug.Log("updated health value:" + value);
        }
    }
    public Animator _animator
    {
        get{return animator;}
    }
    GameObject mainMenu;
    public LayerMask groundLayer;
    [SerializeField]
    Transform groundCheck;
    public bool isGrounded;
    void Start()
    {
        animator = GetComponent<Animator>();
        rootPos = transform.position;
        controller = GetComponent<CharacterController>();
        //Debug.Log("check canvas:" + (GameObject.FindObjectOfType<Canvas>().transform.Find("PointsPanel")==null));
        points_txt = GameObject.FindObjectOfType<Canvas>().transform.Find("PointsPanel").GetComponentInChildren<Text>();
        health_img = GameObject.FindObjectOfType<Canvas>().transform.Find("HealthBar").GetChild(0).GetComponent<Image>();
        _player_health = 100;
        _player_points = 0;
        mainMenu = GameObject.FindObjectOfType<Canvas>().transform.Find("BG").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("verticalVelocity:" + verticalVelocity);
        isGrounded = CheckGrounded();
        if (GameMgr.gameStart)
        {
            if (Input.touchCount > 0 || Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                GameMgr.gameStart = true;
                tapGo.SetActive(false);
                canProcessInput = true;
                if(!animator.enabled) animator.enabled = true;
                animator.SetBool("Run", true);
                animator.SetBool("Idle", false);
            }
        }
        if (player_points < 0)
        {
            player_points = 0;
            //Debug.Log("to be handled if game can get over or only 0");
        }
        if (player_health <= 0)
        {
            //Debug.Log("Game Over!");
            canProcessInput = false;
            animator.SetBool("Die", true);
        }
        if (!canProcessInput)
        {
            if (animator.GetBool("Jump")) animator.SetBool("Jump", false);
            if (animator.GetBool("goLeft")) animator.SetBool("goLeft", false);
            if (animator.GetBool("goRight")) animator.SetBool("goRight", false);
            return;
        }
        Vector3 sideMovement = transform.position;
        var forwardMovement = 5;
        if (animator.GetBool("Jump")) animator.SetBool("Jump", false);
        if (animator.GetBool("goLeft")) animator.SetBool("goLeft", false);
        if (animator.GetBool("goRight")) animator.SetBool("goRight", false);
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startPos = Input.GetTouch(0).position;
                //Debug.Log("startPos:" + startPos);
            }
            // if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                endPos = Input.GetTouch(0).position;
                deltaPos = new Vector3(Mathf.Abs(startPos.x - endPos.x), Mathf.Abs(startPos.y - endPos.y));
                //Debug.Log("endPos:" + endPos);
                //Debug.Log("deltapos check:" + deltaPos);

                if (deltaPos.y > deltaPos.x)
                {
                    // Debug.Log("ground check:"+(CheckGrounded()));
                    if (CheckGrounded() && verticalVelocity < 0)
                    {
                        if (verticalVelocity < 0) verticalVelocity = -1f;
                        if (Mathf.Abs(startPos.y - endPos.y) > 50)
                        {
                            //Debug.Log("Jump");
                            animator.SetBool("Jump", true);
                            verticalVelocity = jumpForce;
                            // rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);//old working
                            // transform.Translate(transform.up * 1.15f);
                            // StartCoroutine(HandleJump());
                            //new one to be handled
                        }
                    }
                    else
                    {
                        if(verticalVelocity > -1)verticalVelocity += gravity * Time.deltaTime;
                    }
                }
                else
                {
                    if (Mathf.Abs(startPos.x - endPos.x) > 20f)
                    {
                        //Debug.Log("movement 1");
                        //perform player move 
                        // if (startPos.x > endPos.x && transform.position.x != -1.5f)
                        if (startPos.x > endPos.x)
                        {
                            //move player left side
                            //Debug.Log("movement left" + ((transform.position.x > 0.1f)));
                            animator.SetBool("goLeft", true);
                            currentLane--;
                            if (currentLane < 0) currentLane = 0;
                            else if (currentLane == 1) controller.Move(new Vector3(currentLane * -1.5f, 0, 0));
                            else controller.Move(new Vector3((currentLane - 1) * 1.5f, 0, 0));
                            // finalPos = new Vector3((transform.position.x > 0.1f) ? 0 : -1.5f, transform.position.y, transform.position.z);
                            // transform.position = finalPos;
                        }
                        // else if (startPos.x < endPos.x && transform.position.x != 1.5f)
                        else if (startPos.x < endPos.x)
                        {
                            //move player right side
                            //Debug.Log("movement right");
                            animator.SetBool("goRight", true);
                            currentLane++;
                            if (currentLane > 2) currentLane = 2;
                            else if (currentLane == 1) controller.Move(new Vector3(currentLane * 1.5f, 0, 0));
                            else controller.Move(new Vector3((currentLane - 1) * 1.5f, 0, 0));
                            // finalPos = new Vector3((transform.position.x == 0f) ? 1.5f : 0f, transform.position.y, transform.position.z);
                            // transform.position = finalPos;
                        }
                    }
                    // else if (Mathf.Abs(startPos.y - endPos.y) > 0.2f)
                    // {
                    //     //Debug.Log("jump panlam mamey");
                    // }
                }
            }
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            animator.SetBool("goLeft", true);
            currentLane--;
            if (currentLane < 0) currentLane = 0;
            else if (currentLane == 1) controller.Move(new Vector3(currentLane * -1.5f, 0, 0));
            else controller.Move(new Vector3((currentLane - 1) * 1.5f, 0, 0));
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            animator.SetBool("goRight", true);
            currentLane++;
            if (currentLane > 2) currentLane = 2;
            else if (currentLane == 1) controller.Move(new Vector3(currentLane * 1.5f, 0, 0));
            else controller.Move(new Vector3((currentLane - 1) * 1.5f, 0, 0));
        }
        else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(CheckGrounded() && verticalVelocity < 0)
            {
                animator.SetBool("Jump", true);
                verticalVelocity = jumpForce;
            }
            else
            {
                if(verticalVelocity > -1)verticalVelocity += gravity * Time.deltaTime;
            }
        }
        // else
        // {
            // if (CheckGrounded())
            // {
            //     if (verticalVelocity < 0) verticalVelocity = -1;
            // }
        else
        {
        if(verticalVelocity > -1)verticalVelocity += gravity * Time.deltaTime;
        if(verticalVelocity < -1) verticalVelocity = -1;
        }
        // }
        float target = (currentLane - 1) * 1.5f;
        //Debug.Log("target:" + target);
        //Debug.Log("verticalvelocity now:" + verticalVelocity);
        // verticalVelocity = Mathf.Clamp(verticalVelocity,-1,jumpForce);
        // var TargetX = Mathf.Lerp(transform.position.x, target, Time.deltaTime*10f);
        // Vector3 finalMove = new Vector3(target - transform.position.x, verticalVelocity, forwardMovement);
        Vector3 finalMove = new Vector3(0, verticalVelocity, forwardMovement);
        //Debug.Log("finalMove:" + finalMove);
        controller.Move(finalMove * Time.deltaTime * speedFact);
    }
    void FixedUpdate()
    {
        // rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speedFact);
        //Debug.Log("is grounded:" + CheckGrounded());
    }
    // void OnCollisionEnter(Collision collision)
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("oncollienter" + hit.gameObject.name);
        if(hit.gameObject.layer == 3)
        {
            canProcessInput = false;
            animator.SetBool("Die", true);
            GameMgr.gameStart = false;
            //Debug.Log("Game Over to be handled!");
        }
    }

    IEnumerator HandleJump()
    {
        yield return new WaitForSeconds(0.5f);
        transform.Translate(transform.up * -1.15f);
        animator.SetBool("Jump", false);
    }

    bool CheckGrounded()
    {
        // if (controller.isGrounded) return true;
        // if (Physics.Raycast(transform.position, Vector3.down, groundDist))
        // {
        //     return true;
        // }
        // return false;
        return Physics.CheckSphere(groundCheck.position,groundDist,groundLayer);
    }

    public void GameOver()
    {
        //Debug.Log("GameOver!");
        canProcessInput = false;
        GameMgr.gameMgr.gameOver.SetActive(true);
        GameMgr.gameMgr.GoToMenu(false);
        animator.SetBool("Run",false);
        animator.SetBool("Die", false);
        GameMgr.gameMgr.finalScore.text = _player_points.ToString();
        GameMgr.gameMgr.SetHighScore(_player_points);
    }

    public void ResetPlayer()
    {
        currentLane = 1;
        if (animator.GetBool("Jump")) animator.SetBool("Jump", false);
        if (animator.GetBool("goLeft")) animator.SetBool("goLeft", false);
        if (animator.GetBool("goRight")) animator.SetBool("goRight", false);
        animator.SetBool("Run",false);
        animator.SetBool("Die", false);
        _player_points = 0;
        _player_health = 100;
        tapGo.SetActive(true);
    }
}
