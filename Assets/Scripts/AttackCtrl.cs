using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackCtrl : MonoBehaviour
{
    public GameObject damageTextPrefab;

    public int dir;

    public int damage;

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

                GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, collision.transform);
                GameObject Textchild = damageText.transform.GetChild(0).gameObject;

                if (isMissed) //빗나감
                {
                    Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
                    Textchild.GetComponent<TMP_Text>().text = "Miss";
                    Textchild.GetComponent<TMP_Text>().color = new Color(0.85f, 0.4f, 1);
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
            else if (gameObject.tag == "NpcAttack")
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
                        collision.GetComponent<PlayerCtrl>().HitEffect();
                    }
                }

                GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, collision.transform);
                GameObject Textchild = damageText.transform.GetChild(0).gameObject;

                if (isMissed) //빗나감
                {
                    Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
                    Textchild.GetComponent<TMP_Text>().text = "Miss";
                    Textchild.GetComponent<TMP_Text>().color = new Color(1, 0.36f, 0.23f);
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
            else if (gameObject.tag == "EnemyAttack")
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
                        collision.GetComponent<PlayerCtrl>().HitEffect();
                    }
                }

                GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, collision.transform);
                GameObject Textchild = damageText.transform.GetChild(0).gameObject;

                if (isMissed) //빗나감
                {
                    Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
                    Textchild.GetComponent<TMP_Text>().text = "Miss";
                    Textchild.GetComponent<TMP_Text>().color = new Color(1, 0.36f, 0.23f);
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
            else if (gameObject.tag == "PartyAttack")
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
                        collision.GetComponent<PlayerCtrl>().HitEffect();
                    }
                }

                GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, collision.transform);
                GameObject Textchild = damageText.transform.GetChild(0).gameObject;

                if (isMissed) //빗나감
                {
                    Textchild.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
                    Textchild.GetComponent<TMP_Text>().text = "Miss";
                    Textchild.GetComponent<TMP_Text>().color = new Color(0.6f, 1, 1);
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

        void CheckDefenseAndDodge()
        {
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
                collision.GetComponent<NpcCtrl>().HitEffect();
            }
        }
    }
}
