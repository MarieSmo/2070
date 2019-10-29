using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    private GameObject healthUI;

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
        HealthUI();

        m_Grounded = false;
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
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

        //only control the player if grounded or airControl is turned on
        if (m_Grounded)
        {
            if (jump) {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            } else {
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                // And then smoothing it out and applying it to the character
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

                if (move > 0 && !m_FacingRight) {
                    Flip();
                }
                else if (move < 0 && m_FacingRight) {
                    Flip();
                }
            }
        }

        if (health <= 0)
        {
            Die();
        }

    }

    public void Attack(bool punch, bool kick) {
        if (timeBeforeAttck <= 0)
        {
            timeBeforeAttck = attackRate;
            if (punch)
            {
                Collider2D[] damageableEnemies = Physics2D.OverlapCircleAll(punchPos.position, punchRange, whatIsEnnemy);
                if (damageableEnemies.Length > 0) {
                    damageableEnemies[0].GetComponent<EnemyBehavior2D>().Damage(punchDamage);
                    print("punch");
                }
            }
            else if (kick)
            {
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
        Gizmos.DrawWireSphere(punchPos.position, punchRange);
        Gizmos.DrawWireSphere(kickPos.position, kickRange);
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