using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackCtrl : MonoBehaviour
{
    public GameObject damageTextPrefab;

    public int dir;

    public int damage;
    public int deterrentDamage;

    public float lifeTime;
    float spawnTime;

    public float speed;

    public bool isCritical;
    public bool isMissed;

    private void Start()
    {
        spawnTime = Time.time;

        if (dir == 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
            transform.position += new Vector3(0, 3, 0);
        }
        else if (dir == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
            transform.position += new Vector3(-3, 0, 0);
        }
        else if (dir == 2)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position += new Vector3(0, -3, 0);
        }
        else if (dir == 3)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            transform.position += new Vector3(3, 0, 0);
        }

        if(isCritical)
        {
            damage = (int)(damage * 1.5f);
        }

        float R = Random.Range(0.9f, 1.1f);

        damage = (int)(damage * R);
        deterrentDamage = (int)(deterrentDamage * R);
    }

    private void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        if(Time.time - spawnTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.transform.parent == collision.gameObject || collision.tag == "PlayerAttack" || collision.tag == "NpcAttack" || collision.tag == "EnemyAttack" || collision.tag == "PartyAttack") //Same Attack and Caster
        {
            //pass
        }
        else
        {
            //Check Self Tag
            if (gameObject.tag == "PlayerAttack")
            {
                if (collision.tag != "Item")
                {
                    if (collision.tag == "NPC")
                    {
                        CheckDefenseAndDodge();

                        PlayerCtrl.instance.isVillan = true;
                    }
                    else if (collision.tag == "Enemy")
                    {
                        CheckDefenseAndDodge();
                    }
                    else if (collision.tag == "Party")
                    {
                        CheckDefenseAndDodge();
                    }

                    //deterrent 저지력 텍스트
                    GameObject damageDeterrentText = Instantiate(damageTextPrefab, new Vector2(transform.position.x + 1.3f, transform.position.y - 1.3f), Quaternion.identity, collision.transform);
                    GameObject deterrentTextchild = damageDeterrentText.transform.GetChild(0).gameObject;

                    deterrentTextchild.GetComponent<TMP_Text>().text = deterrentDamage.ToString();
                    deterrentTextchild.GetComponent<TMP_Text>().color = new Color(0.85f, 1, 0.36f);

                    GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, collision.transform);
                    GameObject Textchild = damageText.transform.GetChild(0).gameObject;

                    if (isMissed) //빗나감
                    {
                        Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
                        Textchild.GetComponent<TMP_Text>().text = "Miss";
                        Textchild.GetComponent<TMP_Text>().color = new Color(0.85f, 0.4f, 1);

                        Destroy(damageDeterrentText);
                    }
                    else //적중
                    {
                        if (isCritical)
                        {
                            Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(4, 4);
                            Textchild.GetComponent<TMP_Text>().text = damage.ToString();
                            Textchild.GetComponent<TMP_Text>().color = new Color(0.85f, 0.4f, 1);
                        }
                        else
                        {
                            Textchild.GetComponent<TMP_Text>().text = damage.ToString();
                            Textchild.GetComponent<TMP_Text>().color = new Color(1, 0.8f, 0.2f);
                        }
                    }

                    Destroy(gameObject);
                }
            }
            else if (gameObject.tag == "NpcAttack")
            {
                if (collision.tag != "Item")
                {
                    if (collision.tag == "NPC")
                    {
                        CheckDefenseAndDodge();
                    }
                    else if (collision.tag == "Enemy")
                    {
                        CheckDefenseAndDodge();
                    }
                    else if (collision.tag == "Player")
                    {
                        //후방 공격인지 체크
                        if (dir == collision.GetComponent<PlayerCtrl>().Direction)
                        {
                            if (!isCritical) //크리티컬이 아니면 크리티컬로 변경
                            {
                                damage = (int)(damage * 1.5f);
                                isCritical = true;
                            }
                        }

                        //데미지에서 방어력만큼 뺌, 방어력 계산
                        damage -= collision.GetComponent<PlayerCtrl>().defense;

                        if (damage <= 0)
                        {
                            damage = 1;
                        }

                        float R = Random.Range(0.0f, 100.0f);

                        if (R < collision.GetComponent<PlayerCtrl>().dodge)
                        {
                            isMissed = true;
                        }
                        else
                        {
                            collision.GetComponent<PlayerCtrl>().HP -= damage;
                            collision.GetComponent<PlayerCtrl>().ST -= deterrentDamage;
                            collision.GetComponent<PlayerCtrl>().HitEffect();
                        }
                    }

                    //deterrent 저지력 텍스트
                    GameObject damageDeterrentText = Instantiate(damageTextPrefab, new Vector2(transform.position.x + 1.3f, transform.position.y - 1.3f), Quaternion.identity, collision.transform);
                    GameObject deterrentTextchild = damageDeterrentText.transform.GetChild(0).gameObject;

                    deterrentTextchild.GetComponent<TMP_Text>().text = deterrentDamage.ToString();
                    deterrentTextchild.GetComponent<TMP_Text>().color = new Color(0.85f, 1, 0.36f);

                    GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, collision.transform);
                    GameObject Textchild = damageText.transform.GetChild(0).gameObject;

                    if (isMissed) //빗나감
                    {
                        Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
                        Textchild.GetComponent<TMP_Text>().text = "Miss";
                        Textchild.GetComponent<TMP_Text>().color = new Color(1, 0.36f, 0.23f);

                        Destroy(damageDeterrentText);
                    }
                    else //적중
                    {
                        if (isCritical)
                        {
                            Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(4, 4);
                            Textchild.GetComponent<TMP_Text>().text = damage.ToString();
                            Textchild.GetComponent<TMP_Text>().color = new Color(1, 0.36f, 0.23f);
                        }
                        else
                        {
                            Textchild.GetComponent<TMP_Text>().text = damage.ToString();
                            Textchild.GetComponent<TMP_Text>().color = new Color(0.6f, 0.1f, 0);
                        }
                    }

                    Destroy(gameObject);
                }
            }
            else if (gameObject.tag == "EnemyAttack")
            {
                if (collision.tag != "Item")
                {
                    if (collision.tag == "NPC")
                    {
                        CheckDefenseAndDodge();
                    }
                    else if (collision.tag == "Enemy")
                    {
                        CheckDefenseAndDodge();
                    }
                    else if (collision.tag == "Player")
                    {
                        //후방 공격인지 체크
                        if (dir == collision.GetComponent<PlayerCtrl>().Direction)
                        {
                            if (!isCritical) //크리티컬이 아니면 크리티컬로 변경
                            {
                                damage = (int)(damage * 1.5f);
                                isCritical = true;
                            }
                        }

                        //데미지에서 방어력만큼 뺌, 방어력 계산
                        damage -= collision.GetComponent<PlayerCtrl>().defense;

                        if (damage <= 0)
                        {
                            damage = 1;
                        }

                        float R = Random.Range(0.0f, 100.0f);

                        if (R < collision.GetComponent<PlayerCtrl>().dodge)
                        {
                            isMissed = true;
                        }
                        else
                        {
                            collision.GetComponent<PlayerCtrl>().HP -= damage;
                            collision.GetComponent<PlayerCtrl>().ST -= deterrentDamage;
                            collision.GetComponent<PlayerCtrl>().HitEffect();
                        }
                    }

                    //deterrent 저지력 텍스트
                    GameObject damageDeterrentText = Instantiate(damageTextPrefab, new Vector2(transform.position.x + 1.3f, transform.position.y - 1.3f), Quaternion.identity, collision.transform);
                    GameObject deterrentTextchild = damageDeterrentText.transform.GetChild(0).gameObject;

                    deterrentTextchild.GetComponent<TMP_Text>().text = deterrentDamage.ToString();
                    deterrentTextchild.GetComponent<TMP_Text>().color = new Color(0.85f, 1, 0.36f);

                    GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, collision.transform);
                    GameObject Textchild = damageText.transform.GetChild(0).gameObject;

                    if (isMissed) //빗나감
                    {
                        Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
                        Textchild.GetComponent<TMP_Text>().text = "Miss";
                        Textchild.GetComponent<TMP_Text>().color = new Color(1, 0.36f, 0.23f);

                        Destroy(damageDeterrentText);
                    }
                    else //적중
                    {
                        if (isCritical)
                        {
                            Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(4, 4);
                            Textchild.GetComponent<TMP_Text>().text = damage.ToString();
                            Textchild.GetComponent<TMP_Text>().color = new Color(1, 0.36f, 0.23f);
                        }
                        else
                        {
                            Textchild.GetComponent<TMP_Text>().text = damage.ToString();
                            Textchild.GetComponent<TMP_Text>().color = new Color(0.6f, 0.1f, 0);
                        }
                    }

                    Destroy(gameObject);
                }
            }
            else if (gameObject.tag == "PartyAttack")
            {
                if (collision.tag != "Item")
                {
                    if (collision.tag == "NPC")
                    {
                        CheckDefenseAndDodge();
                    }
                    else if (collision.tag == "Enemy")
                    {
                        CheckDefenseAndDodge();
                    }
                    else if (collision.tag == "Player")
                    {
                        //후방 공격인지 체크
                        if (dir == collision.GetComponent<PlayerCtrl>().Direction)
                        {
                            if (!isCritical) //크리티컬이 아니면 크리티컬로 변경
                            {
                                damage = (int)(damage * 1.5f);
                                isCritical = true;
                            }
                        }

                        //데미지에서 방어력만큼 뺌, 방어력 계산
                        damage -= collision.GetComponent<PlayerCtrl>().defense;

                        if (damage <= 0)
                        {
                            damage = 1;
                        }

                        float R = Random.Range(0.0f, 100.0f);

                        if (R < collision.GetComponent<PlayerCtrl>().dodge)
                        {
                            isMissed = true;
                        }
                        else
                        {
                            collision.GetComponent<PlayerCtrl>().HP -= damage;
                            collision.GetComponent<PlayerCtrl>().ST -= deterrentDamage;
                            collision.GetComponent<PlayerCtrl>().HitEffect();
                        }
                    }

                    //deterrent 저지력 텍스트
                    GameObject damageDeterrentText = Instantiate(damageTextPrefab, new Vector2(transform.position.x + 1.3f, transform.position.y - 1.3f), Quaternion.identity, collision.transform);
                    GameObject deterrentTextchild = damageDeterrentText.transform.GetChild(0).gameObject;

                    deterrentTextchild.GetComponent<TMP_Text>().text = deterrentDamage.ToString();
                    deterrentTextchild.GetComponent<TMP_Text>().color = new Color(0.85f, 1, 0.36f);

                    GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, collision.transform);
                    GameObject Textchild = damageText.transform.GetChild(0).gameObject;

                    if (isMissed) //빗나감
                    {
                        Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
                        Textchild.GetComponent<TMP_Text>().text = "Miss";
                        Textchild.GetComponent<TMP_Text>().color = new Color(0.6f, 1, 1);

                        Destroy(damageDeterrentText);
                    }
                    else //적중
                    {
                        if (isCritical)
                        {
                            Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(4, 4);
                            Textchild.GetComponent<TMP_Text>().text = damage.ToString();
                            Textchild.GetComponent<TMP_Text>().color = new Color(0.6f, 1, 1);
                        }
                        else
                        {
                            Textchild.GetComponent<TMP_Text>().text = damage.ToString();
                            Textchild.GetComponent<TMP_Text>().color = new Color(0, 0.7f, 0.7f);
                        }
                    }

                    Destroy(gameObject);
                }
            }
        }

        void CheckDefenseAndDodge()
        {
            //후방 공격인지 체크
            if (dir == collision.GetComponent<NpcCtrl>().Direction)
            {
                if (!isCritical) //크리티컬이 아니면 크리티컬로 변경
                {
                    damage = (int)(damage * 1.5f);
                    isCritical = true;
                }
            }

            //데미지에서 방어력만큼 뺌, 방어력 계산
            damage -= collision.GetComponent<NpcCtrl>().defense;

            if (damage <= 0)
            {
                damage = 1;
            }

            float R = Random.Range(0.0f, 100.0f);

            if (R < collision.GetComponent<NpcCtrl>().dodge)
            {
                isMissed = true;
            }
            else
            {
                collision.GetComponent<NpcCtrl>().HP -= damage;
                collision.GetComponent<NpcCtrl>().ST -= deterrentDamage;
                collision.GetComponent<NpcCtrl>().HitEffect();

                if(collision.GetComponent<NpcCtrl>().combatType == NpcCtrl.CombatPersonalityType.Chicken)
                {
                    collision.GetComponent<NpcCtrl>().isScared = false;
                    collision.GetComponent<NpcCtrl>().particleEffect.transform.GetChild(2).gameObject.SetActive(true);    //PainFace:고통스런 얼굴
                }
            }
        }
    }
}
