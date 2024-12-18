using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class NpcCtrl : MonoBehaviour
{
    public CharacterType characterType;
    public NpcType npcType;
    public CombatPersonalityType combatType;
    public GameObject LightObj;

    [Header("Main Stat")]
    //main
    public int level;

    public int aggroRadius;
    public int range;

    [Space()]
    public int HP;
    public int MaxHP;
    public int ST;
    public int MaxST;

    [Header("Additional Stat")]
    //consider with power
    public int strikeDamage;
    public int defense;
    public int movePower;
    public int startMovePower;

    [Space()]
    //consider with dexterity
    public int deterrent;
    public float dodge;
    public float critical;

    [Space()]
    //consider with intellect
    public int magicDamage;

    //type
    public Dictionary<string, int> parts = new Dictionary<string, int>();

    [Header("Sprite")]
    public SpriteRenderer[] partsSpriteRenderer;

    public Sprite[] EmoticonSpr;
    public Sprite[] headSpr;
    public Sprite[] bodySpr;
    public Sprite[] leftHandSpr;
    public Sprite[] rightHandSpr;
    public Sprite[] leftLegSpr;
    public Sprite[] rightLegSpr;
    public Sprite[] directionSpr;

    [Header("Pattern")]
    public int Direction = 2; // 0 = up, 1 = left, 2 = down, 3 = right
    float combatTime = 0;
    float chickenTime = 0;
    public int movePattern;
    public float moveTime;

    [Header("Attack")]
    public GameObject attackPrefab;

    public Sprite[] attackSpr;

    [Header("Mod")]
    public GameObject particleEffect;
    public int pattern; // 0 = Normal, 1 = Combat
    public string mod;
    public float delay;
    public bool isDead;
    public bool isTired;
    public bool isScared;
    public bool isWitness;

    //base
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
        parts.Add("Dir", 0);
    }

    void Start()
    {
        WhatIsCharacter();

        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();

        startMovePower = movePower;

        movePattern = UnityEngine.Random.Range(0, 5);
        moveTime = UnityEngine.Random.Range(1.0f, 5.0f);
    }

    private void Update()
    {
        init();
        StatCtrl();

        if (!isDead)
        {
            SpriteCtrl();

            if (!isTired)
            {
                AniCtrl();
                Combat();
            }
            else
            {
                rigid.velocity = Vector2.zero;
            }
        }
        else
        {
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            rigid.velocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && !isTired)
        {
            Move();
        }
    }

    void WhatIsCharacter()
    {
        if (characterType == CharacterType.Cherra)
        {
            parts["Head"] = 0;
            parts["Body"] = 0;
            parts["LHand"] = 0;
            parts["RHand"] = 0;
            parts["LLeg"] = 0;
            parts["RLeg"] = 0;
        }
        else if (characterType == CharacterType.Steve)
        {
            parts["Head"] = 1;
            parts["Body"] = 1;
            parts["LHand"] = 1;
            parts["RHand"] = 1;
            parts["LLeg"] = 1;
            parts["RLeg"] = 1;
        }
        else if (characterType == CharacterType.Liam)
        {
            parts["Head"] = 2;
            parts["Body"] = 2;
            parts["LHand"] = 2;
            parts["RHand"] = 2;
            parts["LLeg"] = 2;
            parts["RLeg"] = 2;
        }
        else if (characterType == CharacterType.Jade)
        {
            parts["Head"] = 3;
            parts["Body"] = 3;
            parts["LHand"] = 3;
            parts["RHand"] = 3;
            parts["LLeg"] = 3;
            parts["RLeg"] = 3;
        }
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

            for (int i = 0; i < partsSpriteRenderer.Length; i++)
            {
                partsSpriteRenderer[i].material.color = Color.white;
            }

            delay = 0;
        }

        //Ctrl Light
        LightObj.SetActive(npcType == NpcType.Party);
    }

    void StatCtrl()
    {
        movePower = startMovePower;

        movePower = (int)(movePower * (ST / (float)MaxST));

        if (movePower <= startMovePower / 2)
        {
            //땀 파티클 활성화
            particleEffect.transform.GetChild(0).gameObject.SetActive(true);    //LeftSweat:왼쪽 땀
            particleEffect.transform.GetChild(1).gameObject.SetActive(true);    //RightSweat:왼쪽 땀
            movePower = startMovePower / 2;
        }
        else
        {
            //땀 파티클 비활성화
            particleEffect.transform.GetChild(0).gameObject.SetActive(false);    //LeftSweat:왼쪽 땀
            particleEffect.transform.GetChild(1).gameObject.SetActive(false);    //RightSweat:왼쪽 땀
        }

        //Cheack HP and ST
        if (HP <= 0)
        {
            HP = 0;
            isDead = true;

            partsSpriteRenderer[0].gameObject.SetActive(false);
            partsSpriteRenderer[7].gameObject.SetActive(false);

            animator.SetBool("move", false);
            animator.SetBool("hide", false);
            animator.SetBool("hand", false);
            animator.SetBool("weapon", false);
            animator.SetBool("Tool", false);
            animator.SetBool("dead", true);
        }
        else
        {
            isDead = false;
            animator.SetBool("dead", false);
        }

        if (ST <= 0)
        {
            ST = 0;
            isTired = true;
            animator.SetBool("move", false);
            animator.SetBool("hide", false);
            animator.SetBool("hand", false);
            animator.SetBool("weapon", false);
            animator.SetBool("Tool", false);
            animator.SetBool("tired", true);
        }
        else
        {
            isTired = false;
            animator.SetBool("tired", false);
        }
    }

    void Move()
    {
        //Direction
        if (movePattern == 0)
        {
            Direction = 0;
            movement = Vector2.up;
        }
        else if (movePattern == 1)
        {
            Direction = 1;
            movement = Vector2.left;
        }
        else if (movePattern == 2)
        {
            Direction = 2;
            movement = Vector2.down;
        }
        else if (movePattern == 3)
        {
            Direction = 3;
            movement = Vector2.right;
        }
        else if (movePattern == 4)
        {
            movement = Vector2.zero;
        }

        //Move Timer

        if (moveTime > 0)
        {
            moveTime -= 1 * Time.deltaTime;
        }
        else
        {
            if (delay == 0)
            {
                if (pattern == 0) //Normal
                {
                    if (movePattern == 4) //Stand
                    {
                        movePattern = UnityEngine.Random.Range(0, 5);
                        moveTime = UnityEngine.Random.Range(1.0f, 5.0f);
                    }
                    else //Work
                    {
                        movePattern = 4;
                        moveTime = UnityEngine.Random.Range(1.0f, 5.0f);
                    }
                }
                else //Combat
                {
                    movePattern = 5;
                    delay = 0.3f;
                }
            }
        }


        //MovePower
        Collider2D[] moveCol = Physics2D.OverlapBoxAll(transform.position + movement * 1, new Vector2(4, 5.5f) * transform.localScale.x, 0);

        if(moveCol.Length == 1)
        {
            if (pattern == 0)   //전투 중이 아니면:normal
            {
                if (mod != "Attack" && mod != "Interaction" && mod != "Hit")
                {
                    rigid.velocity = movement * movePower / 2;
                }
                else
                {
                    rigid.velocity = Vector2.zero;
                }
            }
            else    //전투 중이면:combat
            {
                if (mod != "Attack" && mod != "Interaction" && mod != "Hit")
                {
                    rigid.velocity = movement * movePower;

                    //지속적으로 스태미나 소모
                    if (combatTime < 5)
                    {
                        combatTime += 1 * Time.deltaTime;
                    }
                    else
                    {
                        ST -= 1;
                        combatTime = 0;
                    }
                }
                else
                {
                    rigid.velocity = Vector2.zero;
                }
            }
        }
        else
        {
            if (pattern == 0)
            {
                rigid.velocity = Vector2.zero;
            }
            else
            {
                if (mod != "Attack" && mod != "Interaction" && mod != "Hit")
                {
                    if (moveCol[1].tag == gameObject.tag || moveCol[1].tag == "Wall")
                    {
                        //방향 전환
                        if (movePattern < 3)
                        {
                            movePattern += 1;
                        }
                        else
                        {
                            movePattern = 0;
                        }
                    }
                    else
                    {
                        if (moveCol[1].tag != "Item")
                        {
                            rigid.velocity = Vector2.zero;
                            Attack();
                        }
                    }
                }
            }
        }

        if (Direction == 0)
        {
            parts["Dir"] = 0;
        }
        else if (Direction == 1)
        {
            parts["Dir"] = 1;
        }
        else if (Direction == 2)
        {
            parts["Dir"] = 2;
        }
        else if (Direction == 3)
        {
            parts["Dir"] = 3;
        }
    }

    float RecogDelay;

    void Combat()
    {
        Collider2D[] aggroCol = Physics2D.OverlapCircleAll(transform.position, aggroRadius);

        if (aggroCol.Length == 1)
        {
            pattern = 0;
        }
        else
        {
            if (tag == "NPC")
            {
                foreach (Collider2D col in aggroCol)
                {
                    if (col.tag == "Enemy" || (col.tag == "Player" && PlayerCtrl.instance.isVillan))
                    {
                        pattern = 1;

                        RecogDelay = 2;

                        CheckCombatType(col);

                        break;
                    }
                    else
                    {
                        if( RecogDelay > 0)
                        {
                            RecogDelay -= 1 * Time.deltaTime;
                        }
                        else
                        {
                            RecogDelay = 0;
                            pattern = 0;
                        }
                    }
                }
            }
            else if (tag == "Enemy")
            {
                foreach (Collider2D col in aggroCol)
                {
                    if (col.tag == "NPC" || col.tag == "Player")
                    {
                        pattern = 1;

                        RecogDelay = 2;

                        CheckCombatType(col);

                        break;
                    }
                    else
                    {
                        if (RecogDelay > 0)
                        {
                            RecogDelay -= 1 * Time.deltaTime;
                        }
                        else
                        {
                            RecogDelay = 0;
                            pattern = 0;
                        }
                    }
                }
            }
            else if (tag == "Party")
            {
                foreach (Collider2D col in aggroCol)
                {
                    if (col.tag == "Enemy")
                    {
                        pattern = 1;

                        RecogDelay = 2;

                        CheckCombatType(col);

                        break;
                    }
                    else
                    {
                        if (RecogDelay > 0)
                        {
                            RecogDelay -= 1 * Time.deltaTime;
                        }
                        else
                        {
                            RecogDelay = 0;
                            pattern = 0;
                        }
                    }
                }
            }
        }

        void CheckCombatType(Collider2D col)
        {
            if (mod != "Attack" && mod != "Interaction" && mod != "Hit")
            {
                Collider2D[] rangeCol = Physics2D.OverlapBoxAll(transform.position + movement * (range / 2), new Vector3(4 + ((range / 1.5f) * Mathf.Abs(movement.x)), 5.5f + ((range / 1.5f - 1.5f) * Mathf.Abs(movement.y)), 5) * transform.localScale.x, 0);
                GameObject target = col.gameObject;
                Vector2 moveDir = Vector2.zero;

                moveTime = 0;

                if (movePattern == 5)
                {
                    if (combatType == CombatPersonalityType.Offensive)
                    {
                        moveDir = target.transform.position - transform.position;

                        if (rangeCol.Length > 1)
                        {
                            if (col != rangeCol[1]) //out Range
                            {
                                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                                {
                                    if (moveDir.x > 0)
                                    {
                                        movePattern = 3;
                                    }
                                    else
                                    {
                                        movePattern = 1;
                                    }
                                }
                                else
                                {
                                    if (moveDir.y > 0)
                                    {
                                        movePattern = 0;
                                    }
                                    else
                                    {
                                        movePattern = 2;
                                    }
                                }
                            }
                            else //in Range
                            {
                                Attack();
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                            {
                                if (moveDir.x > 0)
                                {
                                    movePattern = 3;
                                }
                                else
                                {
                                    movePattern = 1;
                                }
                            }
                            else
                            {
                                if (moveDir.y > 0)
                                {
                                    movePattern = 0;
                                }
                                else
                                {
                                    movePattern = 2;
                                }
                            }
                        }
                    }
                    else if (combatType == CombatPersonalityType.Defensive)
                    {
                        List<GameObject> inAggroCharacters = new List<GameObject>();

                        foreach(Collider2D protect in aggroCol)
                        {
                            if (protect.tag == gameObject.tag)
                            {
                                if (protect.GetComponent<NpcCtrl>().combatType == CombatPersonalityType.Chicken)
                                {
                                    inAggroCharacters.Add(protect.gameObject);
                                }
                            }
                        }

                        if(inAggroCharacters.Count >= 1)
                        {
                            Vector3 center = new Vector3((target.transform.position.x + inAggroCharacters[0].transform.position.x) / 2, (target.transform.position.y + inAggroCharacters[0].transform.position.y) / 2, 0);
                            moveDir = center - transform.position;
                        }
                        else
                        {
                            moveDir = target.transform.position - transform.position;
                        }

                        if (rangeCol.Length > 1)
                        {
                            if (col != rangeCol[1]) //out Range
                            {
                                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                                {
                                    if (moveDir.x > 0)
                                    {
                                        movePattern = 3;
                                    }
                                    else
                                    {
                                        movePattern = 1;
                                    }
                                }
                                else
                                {
                                    if (moveDir.y > 0)
                                    {
                                        movePattern = 0;
                                    }
                                    else
                                    {
                                        movePattern = 2;
                                    }
                                }
                            }
                            else //in Range
                            {
                                Attack();
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                            {
                                if (moveDir.x > 0)
                                {
                                    movePattern = 3;
                                }
                                else
                                {
                                    movePattern = 1;
                                }
                            }
                            else
                            {
                                if (moveDir.y > 0)
                                {
                                    movePattern = 0;
                                }
                                else
                                {
                                    movePattern = 2;
                                }
                            }
                        }
                    }
                    else if (combatType == CombatPersonalityType.Strategic)
                    {
                        moveDir = target.transform.position - transform.position;

                        if (rangeCol.Length > 1)
                        {
                            if (col != rangeCol[1]) //out Range
                            {
                                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                                {
                                    if (moveDir.x > aggroRadius / 2)
                                    {
                                        if (moveDir.y > 4)
                                        {
                                            movePattern = 0;
                                        }
                                        else if (moveDir.y <= 4 && moveDir.y >= -4)
                                        {
                                            if (moveDir.x > 0)
                                            {
                                                movePattern = 3;
                                            }
                                            else
                                            {
                                                movePattern = 1;
                                            }
                                        }
                                        else if (moveDir.y < -4)
                                        {
                                            movePattern = 2;
                                        }
                                    }
                                    else if (moveDir.x < -aggroRadius / 2)
                                    {
                                        if (moveDir.y > 4)
                                        {
                                            movePattern = 0;
                                        }
                                        else if (moveDir.y <= 4 && moveDir.y >= -4)
                                        {
                                            if (moveDir.x > 0)
                                            {
                                                movePattern = 3;
                                            }
                                            else
                                            {
                                                movePattern = 1;
                                            }
                                        }
                                        else if (moveDir.y < -4)
                                        {
                                            movePattern = 2;
                                        }
                                    }
                                    else
                                    {
                                        if (moveDir.x > 0)
                                        {
                                            movePattern = 1;
                                        }
                                        else
                                        {
                                            movePattern = 3;
                                        }
                                    }
                                }
                                else
                                {
                                    if (moveDir.y > aggroRadius / 2)
                                    {
                                        if (moveDir.x > 4)
                                        {
                                            movePattern = 3;
                                        }
                                        else if (moveDir.x <= 4 && moveDir.x >= -4)
                                        {
                                            if (moveDir.y > 0)
                                            {
                                                movePattern = 0;
                                            }
                                            else
                                            {
                                                movePattern = 2;
                                            }
                                        }
                                        else if (moveDir.x < -4)
                                        {
                                            movePattern = 1;
                                        }
                                    }
                                    else if (moveDir.y < -aggroRadius / 2)
                                    {
                                        if (moveDir.x > 4)
                                        {
                                            movePattern = 3;
                                        }
                                        else if (moveDir.x <= 4 && moveDir.x >= -4)
                                        {
                                            if (moveDir.y > 0)
                                            {
                                                movePattern = 0;
                                            }
                                            else
                                            {
                                                movePattern = 2;
                                            }
                                        }
                                        else if (moveDir.x < -4)
                                        {
                                            movePattern = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (moveDir.y > 0)
                                        {
                                            movePattern = 2;
                                        }
                                        else
                                        {
                                            movePattern = 0;
                                        }
                                    }
                                }
                            }
                            else //in Range
                            {
                                Attack();
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                            {
                                if (moveDir.x > aggroRadius / 2)
                                {
                                    if (moveDir.y > 4)
                                    {
                                        movePattern = 0;
                                    }
                                    else if (moveDir.y <= 4 && moveDir.y >= -4)
                                    {
                                        if (moveDir.x > 0)
                                        {
                                            movePattern = 3;
                                        }
                                        else
                                        {
                                            movePattern = 1;
                                        }
                                    }
                                    else if (moveDir.y < -4)
                                    {
                                        movePattern = 2;
                                    }
                                }
                                else if (moveDir.x < -aggroRadius / 2)
                                {
                                    if (moveDir.y > 4)
                                    {
                                        movePattern = 0;
                                    }
                                    else if (moveDir.y <= 4 && moveDir.y >= -4)
                                    {
                                        if (moveDir.x > 0)
                                        {
                                            movePattern = 3;
                                        }
                                        else
                                        {
                                            movePattern = 1;
                                        }
                                    }
                                    else if (moveDir.y < -4)
                                    {
                                        movePattern = 2;
                                    }
                                }
                                else
                                {
                                    if (moveDir.x > 0)
                                    {
                                        movePattern = 1;
                                    }
                                    else
                                    {
                                        movePattern = 3;
                                    }
                                }
                            }
                            else
                            {
                                if (moveDir.y > aggroRadius / 2)
                                {
                                    if (moveDir.x > 4)
                                    {
                                        movePattern = 3;
                                    }
                                    else if (moveDir.x <= 4 && moveDir.x >= -4)
                                    {
                                        if (moveDir.y > 0)
                                        {
                                            movePattern = 0;
                                        }
                                        else
                                        {
                                            movePattern = 2;
                                        }
                                    }
                                    else if (moveDir.x < -4)
                                    {
                                        movePattern = 1;
                                    }
                                }
                                else if (moveDir.y < -aggroRadius / 2)
                                {
                                    if (moveDir.x > 4)
                                    {
                                        movePattern = 3;
                                    }
                                    else if (moveDir.x <= 4 && moveDir.x >= -4)
                                    {
                                        if (moveDir.y > 0)
                                        {
                                            movePattern = 0;
                                        }
                                        else
                                        {
                                            movePattern = 2;
                                        }
                                    }
                                    else if (moveDir.x < -4)
                                    {
                                        movePattern = 1;
                                    }
                                }
                                else
                                {
                                    if (moveDir.y > 0)
                                    {
                                        movePattern = 2;
                                    }
                                    else
                                    {
                                        movePattern = 0;
                                    }
                                }
                            }
                        }
                    }
                    else if (combatType == CombatPersonalityType.Chicken)
                    {
                        if (isScared)   //겁먹었으면
                        {
                            movePattern = 4;
                            chickenTime = 0;
                        }
                        else
                        {
                            moveDir = transform.position - target.transform.position;

                            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                            {
                                if (moveDir.x > 0)
                                {
                                    movePattern = 3;
                                }
                                else
                                {
                                    movePattern = 1;
                                }
                            }
                            else
                            {
                                if (moveDir.y > 0)
                                {
                                    movePattern = 0;
                                }
                                else
                                {
                                    movePattern = 2;
                                }
                            }

                            chickenTime += 1;

                            if(chickenTime >= 10)    //5초간 도망치면 겁먹음
                            {
                                isScared = true;
                            }
                        }
                    }
                }
            }
        }
    }

    void Attack()
    {
        if (mod != "Attack" && mod != "Interaction" && mod != "Hit")
        {
            delay = 1;
            mod = "Attack";

            Invoke("AttackInstantiate", 0.8f);

            //ani
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

    void AttackInstantiate()
    {
        ST -= 2;

        GameObject attackObj = Instantiate(attackPrefab, transform.position + new Vector3(0, -1.5f, 0), Quaternion.identity, gameObject.transform);
        
        if(tag == "NPC")
        {
            attackObj.tag = "NpcAttack";
            attackObj.GetComponent<SpriteRenderer>().color = new Color(1, 0.6f, 0);
        }
        else if (tag == "Enemy")
        {
            attackObj.tag = "EnemyAttack";

            attackObj.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0, 0);
        }
        else if (tag == "Party")
        {
            attackObj.tag = "PartyAttack";
            attackObj.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.7f, 0);
        }

        attackObj.GetComponent<AttackCtrl>().dir = Direction;
        attackObj.GetComponent<AttackCtrl>().deterrentDamage = deterrent;

        //weapon Type
        if (parts["LHand"] == 0)
        {
            attackObj.GetComponent<SpriteRenderer>().sprite = attackSpr[0];

            attackObj.GetComponent<AttackCtrl>().damage = strikeDamage;
            attackObj.GetComponent<AttackCtrl>().lifeTime = 0.1f;
            attackObj.GetComponent<AttackCtrl>().speed = 30f;
        }
        else if (parts["LHand"] == 1)
        {
            attackObj.GetComponent<SpriteRenderer>().sprite = attackSpr[1];
            attackObj.GetComponent<AttackCtrl>().damage = strikeDamage;
            attackObj.GetComponent<AttackCtrl>().lifeTime = 0.2f;
            attackObj.GetComponent<AttackCtrl>().speed = 80f;
        }
        else if (parts["LHand"] == 2)
        {
            attackObj.GetComponent<SpriteRenderer>().sprite = attackSpr[2];
            attackObj.GetComponent<AttackCtrl>().damage = strikeDamage;
            attackObj.GetComponent<AttackCtrl>().lifeTime = 0.15f;
            attackObj.GetComponent<AttackCtrl>().speed = 50f;
        }
        else if (parts["LHand"] == 3)
        {
            attackObj.GetComponent<SpriteRenderer>().sprite = attackSpr[4];
            attackObj.GetComponent<AttackCtrl>().damage = strikeDamage;
            attackObj.GetComponent<AttackCtrl>().lifeTime = 1;
            attackObj.GetComponent<AttackCtrl>().speed = 50f;
        }

        //Critical Check
        float R = UnityEngine.Random.Range(0.0f, 100.0f);
        if (R < critical)
        {
            attackObj.GetComponent<AttackCtrl>().isCritical = true;
        }

        attackObj.AddComponent<PolygonCollider2D>();
        attackObj.GetComponent<PolygonCollider2D>().isTrigger = true;
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
        partsSpriteRenderer[7].sprite = directionSpr[parts["Dir"]];
    }

    void AniCtrl()
    {
        //Move Animation
        if (rigid.velocity != Vector2.zero)
        {
            animator.SetBool("move", true);
        }
        else
        {
            animator.SetBool("move", false);
        }
        
        //Emoticon
        if(pattern == 1)
        {
            if (isScared)
            {
                parts["Emoticon"] = 5;
            }
            else
            {
                parts["Emoticon"] = 1;
            }

            if (partsSpriteRenderer[0].sprite == EmoticonSpr[1])
            {
                partsSpriteRenderer[0].gameObject.SetActive(true);
            }
            else if (partsSpriteRenderer[0].sprite == EmoticonSpr[5])
            {
                partsSpriteRenderer[0].gameObject.SetActive(true);
            }
        }
        else
        {
            partsSpriteRenderer[0].gameObject.SetActive(false);
        }
    }

    public void HitEffect()
    {
        delay = 0.2f;
        mod = "Hit";

        for (int i = 1; i < partsSpriteRenderer.Length - 1; i++)
        {
            partsSpriteRenderer[i].material.color = Color.red;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
        Gizmos.DrawWireSphere(transform.position, range);
        Gizmos.DrawWireCube(transform.position + movement * 1, new Vector3(4, 5.5f, 5) * transform.localScale.x);
        Gizmos.DrawWireCube(transform.position + movement * (range / 2), new Vector3(4 + ((range / 1.5f) * Mathf.Abs(movement.x)), 5.5f + ((range / 1.5f - 1.5f) * Mathf.Abs(movement.y)), 5) * transform.localScale.x);
    }

    public enum CharacterType
    {
        Custum,
        Cherra,
        Steve,
        Liam,
        Jade,
        Homeless,
        Citizen,
        Guard,
        Beggars,
        Bandit,
        BanditBoss,
        Rat,
        Bug,
        Dog,
        Wolf,
        Slime,
        Goblin,
        Oak,
        Ogre,
        Troll,
        Cyclops,
        Laplace
    }

    public enum NpcType
    {
        NPC,
        Enemy,
        Party
    }

    public enum CombatPersonalityType
    {
        Offensive,
        Defensive,
        Strategic,
        Chicken
    }
}
