﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurGuyMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;

    private float horizontalMove = 0f;
    private bool jumpMove = false;
    private bool fistAttack = false;
    private bool kickAttack = false;

    float runSpeed = 40.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //We wanna get the Inputs in here
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed * Time.fixedDeltaTime;
        animator.SetFloat("speed", Mathf.Abs(horizontalMove));

        jumpMove = Input.GetButtonDown("Jump");
        if (jumpMove) {
            print("jump input");
        }
        fistAttack = Input.GetKey(KeyCode.G);
        kickAttack = Input.GetKey(KeyCode.H);
    }

    //And make our player move here
    private void FixedUpdate()
    {
        controller.Move(horizontalMove, jumpMove);
        controller.Attack(fistAttack, kickAttack);
    }
}
