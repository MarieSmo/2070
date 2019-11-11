using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    private Transform m_Transform;
    private GameObject healthUI;
    public Animator animator;

    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;
    public float health;

    public LayerMask whatIsEnnemy;
    private float timeBeforeAttck;
    public float attackRate;
    public Transform punchPos;
    public Transform kickPos;
    public float punchRange;
    public float kickRange;
    public float punchDamage;
    public float kickDamage;


    private void Awake() {
        healthUI = GameObject.Find("HealthNumber");
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        attackRate = 0.7f;
        timeBeforeAttck = 0f;

        punchDamage = 12f;
        kickDamage = 7f;
        kickRange = 2 * punchRange;
        health = 100f;
}

    private void FixedUpdate()
    {
        if (health <= 0) {
            Die();
        }
        HealthUI();

        m_Grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                break;
            }
        }

        if (timeBeforeAttck > 0) {
            timeBeforeAttck -= Time.fixedDeltaTime;
        }
    }

    public void Move(float move, bool jump) {
        if (m_Grounded && jump)
        {
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }

        m_Rigidbody2D.velocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
        if (move > 0 && !m_FacingRight) {
            Flip();
        }
        else if (move < 0 && m_FacingRight) {
            Flip();
        }
    }

    public void Attack(bool punch, bool kick) {
        if (timeBeforeAttck <= 0)
        {
            timeBeforeAttck = attackRate;
            if (punch) {
                animator.SetBool("punch", true);
                Collider2D[] damageableEnemies = Physics2D.OverlapCircleAll(punchPos.position, punchRange, whatIsEnnemy);
                if (damageableEnemies.Length > 0) {
                    damageableEnemies[0].GetComponent<EnemyBehavior2D>().Damage(punchDamage);
                    print("punch");
                }
            }
            else if (kick)
            {
                animator.SetBool("kick", true);
                Collider2D[] damageableEnemies = Physics2D.OverlapCircleAll(kickPos.position, kickRange, whatIsEnnemy);
                if (damageableEnemies.Length > 0) {
                    damageableEnemies[0].GetComponent<EnemyBehavior2D>().Damage(kickDamage);
                    print("kick");
                }
            }
        }
    }

    //Displays some visual infos, for example a circle showing the range
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(punchPos.position, punchRange);
        //Gizmos.DrawWireSphere(kickPos.position, kickRange);
    }

    public void Damage(float damage)
    {
        health -= damage;
        HealthUI();
    }

    // Modify health UI
    void HealthUI() {
        if (health >= 0) healthUI.GetComponent<Text>().text = health.ToString();
        else healthUI.GetComponent<Text>().text = "YOU DIED";
    }

    private void Die() {
        Destroy(gameObject);
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}