using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBehavior2D : MonoBehaviour
{
    
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the enemy is grounded.

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the enemy is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    private float timeBtwAttck;
    public Transform attackPos;
    public float attackRange;
    public LayerMask whatIsEnnemy;
    public float health = 15f;
    public float damage = 10f;
    public float startTimeBtwAttck = 0.3f;
    public float velocity;

    public GameObject player;


    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        if (player != null)
        {
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    m_Grounded = true;
                    if (!wasGrounded)
                        OnLandEvent.Invoke();
                }
            }

            if (health <= 0)
            {
                Die();
            }

            float move = 0f;
            if (Math.Abs(player.transform.position.x - attackPos.position.x) < attackRange)
            {
                Attack();
            }
            else
            {
                move = player.transform.position.x > attackPos.position.x ? 1 : -1;
            }
            Move(move);
        }
    }

    public void Move(float move)
    {
        Vector3 targetVelocity = new Vector2(move * velocity, m_Rigidbody2D.velocity.y);
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if (move > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (move < 0 && m_FacingRight)
        {
            Flip();
        }
    }

    public void Attack()
    {
        if (timeBtwAttck <= 0)
        {
            timeBtwAttck = startTimeBtwAttck;
            Collider2D[] damageableEnemies = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnnemy);
            for (int i = 0; i < damageableEnemies.Length; i++)
            {
                player.GetComponent<CharacterController2D>().Damage(damage);
            }
        }
        else
        {
            timeBtwAttck -= Time.fixedDeltaTime;
        }
    }

    public void Damage(float damage)
    {
        health -= damage;
        print(health);
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
