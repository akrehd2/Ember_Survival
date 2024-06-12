using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public float movePower;
    public string mod;
    public float delay;

    public BoxCollider2D interactionColider;


    Rigidbody2D rigid;

    Animator animator;

    Vector3 movement;

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if(delay > 0) 
        {
            delay -= 1 * Time.deltaTime;
        }
        else
        {
            mod = null;
            animator.SetBool("attack", false);
            delay = 0;
        }

        AniCtrl();
        Attack();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement.Normalize();

        if (mod != "Attack" && mod != "Interaction")
        {
            rigid.velocity = movement * movePower;
        }
        else
        {
            rigid.velocity = Vector2.zero;
        }
    }

    void Attack()
    {
        if(Input.GetKeyDown(KeyCode.Z) && mod != "Interaction")
        {
            mod = "Attack";
            animator.SetBool("attack", true);
            delay = 1;
        }
    }

    void AniCtrl()
    {
        //move
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow))
            && mod != "Attack" && mod != "Interaction")
        {
            animator.SetBool("move", true);
        }
        else
        {
            animator.SetBool("move", false);
        }

        //attack
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null && mod != "Attack")
        {
            Debug.Log("충돌중");
            if (collision.tag == "NPC")
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("interaction with NPC");
                }
            }
        }
    }
}
