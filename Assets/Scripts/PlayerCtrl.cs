using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    public static PlayerCtrl instance;

    [Header("Main Stat")]
    //main
    public int level;
    public int exp;
    public int MaxExp;

    public int interactionRadius;

    [Space()]
    public int power;
    public int dexterity;
    public int intellect;
    public int charisama;

    [Space()]
    public int HP;
    public int MaxHP;
    public int HG;
    public int MaxHG;
    public int ST;
    public int MaxST;

    [Header("Additional Stat")]
    //consider with power
    public int strikeDamage;
    public int defense;
    public int movePower;

    [Space()]
    //consider with dexterity
    public float stealth;
    public float dodge;
    public float critical;

    [Space()]
    //consider with intellect
    public int magicDamage;

    [Space()]
    //consider with charisama
    public int charm;

    [Header("Equip Stat")]
    public int weaponDamage;
    public int weaponEnergy;
    public int equipDefense;
    public int equipWeight;
    public int equipCharm;

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

    //dir
    int Direction = 2; // 0 = up, 1 = left, 2 = down, 3 = right

    [Header("Attack")]
    public GameObject attackPrefab;

    public Sprite[] attackSpr;

    [Header("Mod")]
    public GameObject particleEffect;
    public string mod;
    public float delay;
    public bool isVillan;
    public bool isDead;
    public bool isTired;

    [Header("Interaction")]
    public BoxCollider2D interactionColider;
    public LayerMask interactionLayer;

    //base
    Rigidbody2D rigid;

    Animator animator;

    Vector3 movement;

    private void Awake()
    {
        instance = this;

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
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
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
                Attack();
                Interaction();
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

            for(int i = 1; i < partsSpriteRenderer.Length - 1; i++)
            {
                partsSpriteRenderer[i].material.color = Color.white;
            }

            delay = 0;
        }
    }

    void StatCtrl()
    {
        #region EquipStat

        int WD = 0;
        int WE = 0;
        int ED = 0;
        int EW = 0;
        int EC = 0;

        //Left Hand
        if (parts["LHand"] == 0)
        {
            WD += 0;
            EW += 0;
        }
        else if (parts["LHand"] == 1)
        {
            WD += 10;
            EW += 1;
            EC -= 3;
        }

        //Right Hand
        if (parts["RHand"] == 0)
        {
            ED += 0;
            EW += 0;
        }
        else if (parts["RHand"] == 1)
        {
            ED += 3;
            EW += 1;
            EC -= 1;
        }

        //Head
        if (parts["Head"] == 0)
        {
            ED += 0;
            EW += 0;
        }
        else if (parts["Head"] == 1)
        {
            ED += 1;
            EW += 0;
        }

        //Body
        if (parts["Body"] == 0)
        {
            ED += 0;
            EW += 0;
        }
        else if (parts["Body"] == 1)
        {
            ED += 5;
            EW += 1;
        }

        //Shoes
        if (parts["LLeg"] == 0)
        {
            ED += 0;
            EW += 0;
        }
        else if (parts["LLeg"] == 1)
        {
            ED += 0;
            EW += -1;
            EC += 1;
        }


        weaponDamage = WD;
        weaponEnergy = WE;
        equipDefense = ED;
        equipWeight = EW;
        equipCharm = EC;

        #endregion

        //Set Stat Formula
        MaxHP = 30 + power * 2;
        MaxST = 100 + dexterity * 4;

        strikeDamage = (int)(power * 1.2f) + weaponDamage;
        defense = (int)(power * 0.5f) + equipDefense;

        if (mod == "Hide" || mod == "HideAttack")
        {
            movePower = (15 + (int)(power * 0.2f) - equipWeight) / 3;

            movePower = (int)(movePower * (ST / (float)MaxST));

            if (movePower <= ((15 + (int)(power * 0.2f) - equipWeight) / 3) / 2)
            {
                particleEffect.SetActive(true);
                movePower = ((15 + (int)(power * 0.2f) - equipWeight) / 3) / 2;
            }
            else
            {
                particleEffect.SetActive(false);
            }

            stealth = 100 * (1 - Mathf.Exp(-0.05f * dexterity));
        }
        else
        {
            movePower = 15 + (int)(power * 0.2f) - equipWeight;

            movePower = (int)(movePower * (ST / (float)MaxST));

            if (movePower <= (15 + (int)(power * 0.2f) - equipWeight) / 2)
            {
                particleEffect.SetActive(true);
                movePower = (15 + (int)(power * 0.2f) - equipWeight) / 2;
            }
            else
            {
                particleEffect.SetActive(false);
            }

            stealth = (100 * (1 - Mathf.Exp(-0.05f * dexterity))) / 2;
        }

        dodge = 100 * (1 - Mathf.Exp(-0.05f * dexterity)) / 2.8f;
        critical = 100 * (1 - Mathf.Exp(-0.02f * dexterity));

        magicDamage = intellect * 1 + weaponEnergy;

        charm = 5 + charisama * 1 + equipCharm;

        //Cheack HP and ST
        if(HP <= 0)
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

    void Attack()
    {
        if(Input.GetKeyDown(KeyCode.Z) && mod != "Hide" && mod != "Attack" && mod != "HideAttack" && mod != "Interaction" && mod != "Hit")
        {
            delay = 1;
            mod = "Attack";

            Invoke("AttackInstantiate", 0.6f);
            
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
        //HideAttack
        else if (Input.GetKeyDown(KeyCode.Z) && mod == "Hide" && mod != "Attack" && mod != "HideAttack" && mod != "Interaction" && mod != "Hit")
        {
            delay = 1;
            mod = "HideAttack";

            Invoke("AttackInstantiate", 0.6f);

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
        GameObject attackObj = Instantiate(attackPrefab, transform.position + new Vector3(0, -1.5f, 0), Quaternion.identity, gameObject.transform);
        attackObj.tag = "PlayerAttack";
        attackObj.GetComponent<AttackCtrl>().dir = Direction;

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
            attackObj.GetComponent<AttackCtrl>().lifeTime = 0.1f;
            attackObj.GetComponent<AttackCtrl>().speed = 50f;
        }

        //Critical Check
        float R = Random.Range(0.0f, 100.0f);
        if (R < critical)
        {
            attackObj.GetComponent<AttackCtrl>().isCritical = true;
        }

        attackObj.AddComponent<PolygonCollider2D>();
        attackObj.GetComponent<PolygonCollider2D>().isTrigger = true;
    }

    void Interaction()
    {
        Collider2D colTrigger = Physics2D.OverlapCircle(transform.position, interactionRadius, interactionLayer);

        if (colTrigger != null)
        {
            //Emoticon
            if(mod != "Attack" && mod != "Interaction" && mod != "Hit")
            {
                if (colTrigger.tag == "NPC" && !isVillan && mod != "Hide")
                {
                    parts["Emoticon"] = 2;

                    if (partsSpriteRenderer[0].sprite == EmoticonSpr[2])
                    {
                        partsSpriteRenderer[0].gameObject.SetActive(true);
                    }
                }
                else if (colTrigger.tag == "Item")
                {
                    parts["Emoticon"] = 1;

                    if (partsSpriteRenderer[0].sprite == EmoticonSpr[1])
                    {
                        partsSpriteRenderer[0].gameObject.SetActive(true);
                    }
                }
                else
                {
                    partsSpriteRenderer[0].gameObject.SetActive(false);
                }
            }
            else
            {
                partsSpriteRenderer[0].gameObject.SetActive(false);
            }


            //Start Interaction
            if (Input.GetKeyDown(KeyCode.Space) && mod != "Attack" && mod != "HideAttack" && mod != "Interaction" && mod != "Hit")
            {
                if (colTrigger.tag == "Item")
                {
                    UICtrl.instance.AddInventory(colTrigger.gameObject, colTrigger.GetComponent<SpriteRenderer>().sprite);
                }
                else if (colTrigger.tag == "NPC" && !isVillan && mod != "Hide")
                {
                    delay = 9999;
                    mod = "Interaction";

                    colTrigger.gameObject.GetComponent<NpcCtrl>().delay = 9999;
                    colTrigger.gameObject.GetComponent<NpcCtrl>().mod = "Interaction";
                    colTrigger.gameObject.GetComponent<DialogSet>().SetDialog();
                }
            }
            else if(Input.GetKeyDown(KeyCode.Space) && mod == "Interaction") //During Interaction
            {
                DialogSystem.instance.nowNPC.GetComponent<DialogSet>().NextDialog();
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

        if (mod != "Attack" && mod != "HideAttack" && mod != "Interaction" && mod != "Hit")
        {
            rigid.velocity = movement * movePower;
        }
        else
        {
            rigid.velocity = Vector2.zero;
        }

        //Direction
        if (mod != "Attack" && mod != "HideAttack" && mod != "Interaction" && mod != "Hit")
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                Direction = 0;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                Direction = 1;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                Direction = 2;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                Direction = 3;
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

    public void HitEffect()
    {
        delay = 0.2f;
        mod = "Hit";

        for (int i = 1; i < partsSpriteRenderer.Length - 1; i++)
        {
            partsSpriteRenderer[i].material.color = Color.red;
        }
    }

    void AniCtrl()
    {
        //move and hide
        if(Input.GetKey(KeyCode.LeftShift) && mod != "Attack" && mod != "HideAttack" && mod != "Interaction" && mod != "Hit")
        {
            delay = 9999;
            mod = "Hide";
            animator.SetBool("hide", true);

            for (int i = 1; i < partsSpriteRenderer.Length - 1; i++)
            {
                partsSpriteRenderer[i].material.color = new Color(1, 1, 1, 0.7f);
            }
        }
        else
        {
            if(mod == "Hide")
            {
                delay = 0;
            }

            for (int i = 1; i < partsSpriteRenderer.Length - 1; i++)
            {
                partsSpriteRenderer[i].material.color = Color.white;
            }

            animator.SetBool("hide", false);
        }

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow))
            && mod != "Attack" && mod != "HideAttack" && mod != "Interaction" && mod != "Hit")
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
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
