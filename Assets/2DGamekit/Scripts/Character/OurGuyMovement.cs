using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurGuyMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;

    private float horizontalMove;
    private float jumpMove;
    private bool fistAttack;
    private bool kickAttack;
    float runSpeed;
    
    void Awake()
    {
        horizontalMove = 0f;
        jumpMove = 0f;
        fistAttack = false;
        kickAttack = false;
        runSpeed = 40.0f;
}

    // Update is called once per frame
    //We wanna get the Inputs in here
    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal") * runSpeed * Time.fixedDeltaTime;
        animator.SetFloat("speed", Mathf.Abs(horizontalMove));

        jumpMove = Input.GetAxis("Jump");
        fistAttack = Input.GetButtonDown("Punch");
        if (fistAttack) {
            print("fist_input");
        }
        kickAttack = Input.GetButtonDown("Kick");
        if (kickAttack) { print("kick_inut"); }
    }

    //And make our player move here
    private void FixedUpdate()
    {
        bool jump = jumpMove > 0 ? true : false;

        controller.Move(horizontalMove, jump);
        controller.Attack(fistAttack, kickAttack);
    }
}
