using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public LayerMask interactionLayer;

    //type
    public Dictionary<string, int> parts = new Dictionary<string, int>();

    //sprite
    public SpriteRenderer[] partsSpriteRenderer;

    public Sprite[] EmoticonSpr;
    public Sprite[] headSpr;
    public Sprite[] bodySpr;
    public Sprite[] leftHandSpr;
    public Sprite[] rightHandSpr;
    public Sprite[] leftLegSpr;
    public Sprite[] rightLegSpr;

    //move
    public float movePower;
    int moveDirection; // 1 = up, 2 = left, 3 = down, 4 = right

    //mod
    public string mod;
    public float delay;

    //interaction
    public BoxCollider2D interactionColider;


    Rigidbody2D rigid;

    Animator animator;

    Vector3 movement;

    private void Awake()
    {
        parts.Add("Emoticon", 0);
        parts.Add("Head", 0);
        parts.Add("Body", 0);
        parts.Add("LHand", 0);
        parts.Add("RHand", 0);
        parts.Add("LLeg", 0);
        parts.Add("RLeg", 0);
    }

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        init();
        SpriteCtrl();
        AniCtrl();
        Attack();
        Interaction();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void init()
    {
        if (delay > 0)
        {
            delay -= 1 * Time.deltaTime;
        }
        else
        {
            mod = null;
            animator.SetBool("hand", false);
            animator.SetBool("weapon", false);
            delay = 0;
        }
    }

    void Attack()
    {
        if(Input.GetKeyDown(KeyCode.Z) && mod != "Attack" && mod != "Interaction")
        {
            delay = 1;
            mod = "Attack";
            if (parts["LHand"] == 0)
            {
                animator.SetBool("hand", true);
            }
            else
            {
                animator.SetBool("weapon", true);
            }
        }
    }

    void Interaction()
    {
        Collider2D colTrigger = Physics2D.OverlapCircle(transform.position, 5, interactionLayer);

        if (colTrigger != null)
        {
            //Emoticon
            if(mod != "Attack" && mod != "Interaction")
            {
                partsSpriteRenderer[0].gameObject.SetActive(true);

                if (colTrigger.tag == "NPC")
                {
                    parts["Emoticon"] = 2;
                }
                else if (colTrigger.name == "Club")
                {
                    parts["Emoticon"] = 1;
                }
            }
            else
            {
                partsSpriteRenderer[0].gameObject.SetActive(false);
            }


            //Start Interaction
            if (Input.GetKeyDown(KeyCode.Space) && mod != "Attack" && mod != "Interaction")
            {
                delay = 1;
                mod = "Interaction";

                if (colTrigger.name == "Club")
                {
                    parts["LHand"] = 1;
                    colTrigger.gameObject.SetActive(false);
                }
                else if (colTrigger.tag == "NPC")
                {
                    Debug.Log("상호작용");
                }
            }
        }
        else
        {
            partsSpriteRenderer[0].gameObject.SetActive(false);
        }
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

        //moveDirection
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection = 1;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection = 2;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection = 3;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection = 4;
        }
    }

    void SpriteCtrl()
    {
        partsSpriteRenderer[0].sprite = EmoticonSpr[parts["Emoticon"]];
        partsSpriteRenderer[1].sprite = headSpr[parts["Head"]];
        partsSpriteRenderer[2].sprite = bodySpr[parts["Body"]];
        partsSpriteRenderer[3].sprite = leftHandSpr[parts["LHand"]];
        partsSpriteRenderer[4].sprite = rightHandSpr[parts["RHand"]];
        partsSpriteRenderer[5].sprite = leftLegSpr[parts["LLeg"]];
        partsSpriteRenderer[6].sprite = rightLegSpr[parts["RLeg"]];
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5);
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision != null && mod != "Attack" && mod != "Interaction")
    //    {
    //        Debug.Log("충돌중");
    //        if (collision.tag == "NPC")
    //        {
    //            if (Input.GetKeyDown(KeyCode.Space))
    //            {
    //                Debug.Log("interaction with NPC");
    //            }
    //        }
    //    }
    //}
}
