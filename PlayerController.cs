using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    public float maxSpeed;
    //**********************************
    public bool isGrounded;
    public LayerMask groundLayer;
    public Transform groundCheck;
    //***********************************
    private int desiredLane = 1;   //0:left 1:middle 2:right
    public float laneDistance = 2.5f;  //The distance between tow lanes
    
    //Jump**
    public float jumpForce;
    public float Gravity = -20; 

    public Animator animator;
    private bool isSliding = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerManager.isGameStarted)
           return;
        //*******************************************jump
        //Increase Speed*******************
        if(forwardSpeed < maxSpeed)
        forwardSpeed += 0.1f * Time.deltaTime;
        //******************************************
       animator.SetBool("isGameStarted", true);
       direction.z = forwardSpeed;

       bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
       animator.SetBool("isGrounded" , isGrounded);
       if(isGrounded)
       {
            
            if (SwipeManager.swipeUp) //กดปุ่ม(Input.GetKeyDown(KeyCode.UpArrow))
                Jump();

            if (SwipeManager.swipeDown && !isSliding)
                StartCoroutine(Slide());
       }
       else
       {
            //animator.SetBool("isGrounded", !isGrounded); 
            direction.y += Gravity * Time.deltaTime;  //1jump     
          //***********************************************************************************jump
       //****************down*************
            if (SwipeManager.swipeDown && !isSliding)
           {
                StartCoroutine(Slide());
                direction.y = -8;
           }
       }
       //Gather the inputs on which lane we should be
       if (SwipeManager.swipeRight)    //กดปุ่ม (Input.GetKeyDown(KeyCode.RightArrow)) 
       {
           desiredLane++;
           if (desiredLane == 3)
               desiredLane = 2;
       }

       if (SwipeManager.swipeLeft)  //กดปุ่ม (Input.GetKeyDown(KeyCode.LeftArrow)) 
       {
           desiredLane--;
           if (desiredLane == -1)
               desiredLane = 0;
       }

       //Calculate where we should be in the future

       Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

       if (desiredLane == 0)
       {
            targetPosition += Vector3.left * laneDistance;
       }
       else if (desiredLane == 2)
       {
            targetPosition += Vector3.right * laneDistance;
       }

       //transform.position = Vector3.Lerp(transform.position, targetPosition, 70 * Time.deltaTime); //เปลี่ยนเลนได้อย่างเดียวแบบสมูท
       //เปลี่ยนเลนแบบสมูทพร้อมกับกระโดดกลางอากาศ
       if (transform.position != targetPosition)
       {
            Vector3 diff = targetPosition - transform.position;
            Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
             
             if (moveDir.sqrMagnitude < diff.magnitude)
                 controller.Move(moveDir);
             else
                 controller.Move(diff);
       } 
           
         //Move Player 
        //if(!PlayerManager.isGameStarted)
           //return;
        controller.Move(direction * Time.deltaTime);
    
    }
  
    
    
    //jump
    private void Jump()
    {
        direction.y = jumpForce;
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
        }
    }
    //slide
    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding" , true);
        controller.center = new Vector3(0,-0.5f,0);
        controller.height = 1;
        yield return new WaitForSeconds(1.3f);

        controller.center = new Vector3(0,0,0);
        controller.height = 2;
        animator.SetBool("isSliding" , false);
        isSliding = false;
    }
}
