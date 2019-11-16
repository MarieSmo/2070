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

    private float timeBeforeAttck;
    public float attackRate;
    public Transform attackPos;
    public float attackRange;
    public LayerMask whatIsEnnemy;
    public float health = 15f;
    public float damage = 10f;
    public float velocity;
	
	[Range(0, 1f)] public float meat_spawn_prob;
	public float meat_value;
	[Range(0, 1f)] public float flight_prob_100;
	[Range(0, 1f)] public float flight_prob_50;
	[Range(0, 1f)] public float flight_prob_20;
	[Range(0, 1f)] public float flight_prob_10;
	private float current_flight_prob;
	private bool flying_away;

    public GameObject player;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
		current_flight_prob = flight_prob_100;
		flying_away = false;
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
                }
            }

            if (health <= 0 || (flying_away && !gameObject.GetComponent<Renderer>().isVisible))
            {
                Die();
            }

            float move = 0f;
			if (flying_away) {
				move = player.transform.position.x > attackPos.position.x ? -1 : 1;
				Move(move);
			} else {
				Fly_away();
				if (Math.Abs(player.transform.position.x - attackPos.position.x) < attackRange)
				{
					Attack();
				}
				else
				{
					move = player.transform.position.x > attackPos.position.x ? 1 : -1;
					Move(move);
				}
			}
        }

        if (timeBeforeAttck > 0) {
            timeBeforeAttck -= Time.fixedDeltaTime;
        }
    }

    //Displays some visual infos, for example a circle showing the range
    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(attackPos.position, attackRange);
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
        if (timeBeforeAttck <= 0)
        {
            timeBeforeAttck = attackRate;
            Collider2D[] damageableEnemies = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnnemy);
            for (int i = 0; i < damageableEnemies.Length; i++)
            {
                player.GetComponent<CharacterController2D>().Damage(damage);
            }
        }
    }
	
	public void Fly_away() {
		if (new System.Random().NextDouble() <= current_flight_prob) {
			velocity *= 2.5f;
			flying_away = true;
		}
	}

    public void Damage(float damage)
    {
        health -= damage;
		if (health <= 10) {
			current_flight_prob = flight_prob_10;
		} else if (health <= 20) {
			current_flight_prob = flight_prob_20;
		} else if (health <= 50) {
			current_flight_prob = flight_prob_50;
		}
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
		if (new System.Random().NextDouble() <= meat_spawn_prob) {
			player.GetComponent<CharacterController2D>().Heal(meat_value);
		}
		
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
