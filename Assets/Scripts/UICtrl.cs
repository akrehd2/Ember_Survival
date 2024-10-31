using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICtrl : MonoBehaviour
{
    public static UICtrl instance;

    public GameObject InGameUI;

    [Header("Portrait")]
    public Image FaceRenderer;
    public Sprite[] FaceSprites;

    [Header("Slider")]
    public TextMeshProUGUI HpText;
    public TextMeshProUGUI HgText;
    public TextMeshProUGUI StText;
    public Slider HpSlider;
    public Slider HgSlider;
    public Slider StSlider;

    [Header("Inventory")]
    public GameObject inventory;
    public GameObject itemPrefab;
    public GameObject[] Slots;

    [Header("Equipment")]
    public GameObject equipment;
    public GameObject[] equipSlots;

    [Header("Stat")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public Slider expSlider;
    public GameObject[] StatUIs;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        KeyToUIOpen();
        PortraitUICtrl();
        SliderUICtrl();
        EquipmentUICtrl();
        StatUICtrl();
    }

    void KeyToUIOpen()
    {
        //Inventory
        if(Input.GetKeyDown(KeyCode.I))
        {
            if (inventory.activeSelf == false)
            {
                inventory.SetActive(true);
            }
            else
            {
                inventory.SetActive(false);
            }
        }

        //Equipment
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (equipment.activeSelf == false)
            {
                equipment.SetActive(true);
            }
            else
            {
                equipment.SetActive(false);
            }
        }
    }

    void PortraitUICtrl()
    {
        int PortraitIndex = 0;

        float HpPercent = (PlayerCtrl.instance.HP / (float)PlayerCtrl.instance.MaxHP) * 100;
        float HgPercent = (PlayerCtrl.instance.HG / (float)PlayerCtrl.instance.MaxHG) * 100;
        float StPercent = (PlayerCtrl.instance.ST / (float)PlayerCtrl.instance.MaxST) * 100;

        if (HpPercent > 66 && HgPercent > 66 && StPercent > 66)
        {
            PortraitIndex = 0;
        }
        else if ((HpPercent <= 66 && HpPercent > 33) || (HgPercent <= 66 && HgPercent > 33) || (StPercent <= 66 && StPercent > 33))
        {
            PortraitIndex = 1;

            if (HpPercent <= 33 || HgPercent <= 33 || StPercent <= 33)
            {
                PortraitIndex = 2;
            }
        }
        else if (HpPercent <= 33 || HgPercent <= 33 || StPercent <= 33)
        {
            PortraitIndex = 2;
        }

        FaceRenderer.sprite = FaceSprites[PortraitIndex];
    }

    void SliderUICtrl()
    {
        HpSlider.maxValue = PlayerCtrl.instance.MaxHP;
        HgSlider.maxValue = PlayerCtrl.instance.MaxHG;
        StSlider.maxValue = PlayerCtrl.instance.MaxST;

        HpText.text = PlayerCtrl.instance.HP.ToString();
        HpSlider.value = PlayerCtrl.instance.HP;

        HgText.text = PlayerCtrl.instance.HG.ToString();
        HgSlider.value = PlayerCtrl.instance.HG;

        StText.text = PlayerCtrl.instance.ST.ToString();
        StSlider.value = PlayerCtrl.instance.ST;
    }

    void EquipmentUICtrl()
    {
        //Head
        if (equipSlots[0].transform.childCount == 0)
        {
            PlayerCtrl.instance.parts["Head"] = 0;
        }
        else
        {
            PlayerCtrl.instance.parts["Head"] = int.Parse(equipSlots[2].transform.GetChild(0).GetComponent<Image>().sprite.name.Split("_")[0]);
        }

        //Body
        if (equipSlots[1].transform.childCount == 0)
        {
            PlayerCtrl.instance.parts["Body"] = 0;
        }
        else
        {
            PlayerCtrl.instance.parts["Body"] = int.Parse(equipSlots[2].transform.GetChild(0).GetComponent<Image>().sprite.name.Split("_")[0]);
        }

        //Weapon
        if (equipSlots[2].transform.childCount == 0)
        {
            PlayerCtrl.instance.parts["LHand"] = 0;
        }
        else
        {
            PlayerCtrl.instance.parts["LHand"] = int.Parse(equipSlots[2].transform.GetChild(0).GetComponent<Image>().sprite.name.Split("_")[0]);
        }

        //Sub
        if (equipSlots[3].transform.childCount == 0)
        {
            PlayerCtrl.instance.parts["RHand"] = 0;
        }
        else
        {
            PlayerCtrl.instance.parts["RHand"] = int.Parse(equipSlots[2].transform.GetChild(0).GetComponent<Image>().sprite.name.Split("_")[0]);
        }

        //Shoes
        if (equipSlots[4].transform.childCount == 0)
        {
            PlayerCtrl.instance.parts["LLeg"] = 0;
            PlayerCtrl.instance.parts["RLeg"] = 0;
        }
        else
        {
            PlayerCtrl.instance.parts["LLeg"] = int.Parse(equipSlots[2].transform.GetChild(0).GetComponent<Image>().sprite.name.Split("_")[0]);
            PlayerCtrl.instance.parts["RLeg"] = int.Parse(equipSlots[2].transform.GetChild(0).GetComponent<Image>().sprite.name.Split("_")[0]);
        }
    }

    void StatUICtrl()
    {
        levelText.text = PlayerCtrl.instance.level + " Lv";
        expText.text = PlayerCtrl.instance.exp + "/" + PlayerCtrl.instance.MaxExp;
        expSlider.maxValue = PlayerCtrl.instance.MaxExp;
        expSlider.value = PlayerCtrl.instance.exp;

        StatUIs[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.MaxHP.ToString();
        StatUIs[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.MaxHG.ToString();
        StatUIs[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.MaxST.ToString();
        StatUIs[3].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.strikeDamage.ToString();
        StatUIs[4].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.magicDamage.ToString();
        StatUIs[5].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.deterrent.ToString();
        StatUIs[6].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.defense.ToString();
        StatUIs[7].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.stealth.ToString("N1") + "%";
        StatUIs[8].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.dodge.ToString("N1") + "%";
        StatUIs[9].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.critical.ToString("N1") + "%";
        StatUIs[10].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.charm.ToString();
        StatUIs[11].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerCtrl.instance.movePower.ToString();
    }

    public void AddInventory(GameObject colItem, Sprite imageSprite)
    {
        int slotCount = 0;

        foreach(GameObject EmptySlot in Slots)
        {
            if(EmptySlot.transform.childCount == 0)
            {
                colItem.SetActive(false);

                GameObject newItem = Instantiate(itemPrefab, EmptySlot.transform);

                string newItemType = newItem.GetComponent<Image>().sprite.name.Split('_')[1];

                newItem.GetComponent<Image>().sprite = imageSprite;
                newItem.GetComponent<Image>().SetNativeSize();
                newItem.GetComponent<RectTransform>().sizeDelta = new Vector2(newItem.GetComponent<RectTransform>().sizeDelta.x * 2, newItem.GetComponent<RectTransform>().sizeDelta.y * 2);

                if (newItemType == "Head")
                {
                    newItem.GetComponent<IconDrag>().itemType = ItemType.Head;
                }
                else if (newItemType == "Body")
                {
                    newItem.GetComponent<IconDrag>().itemType = ItemType.Body;
                }
                else if (newItemType == "Weapon")
                {
                    newItem.GetComponent<IconDrag>().itemType = ItemType.Weapon;
                }
                else if (newItemType == "Sub")
                {
                    newItem.GetComponent<IconDrag>().itemType = ItemType.Sub;
                }
                else if (newItemType == "Shoes")
                {
                    newItem.GetComponent<IconDrag>().itemType = ItemType.Shoes;
                }
                else if (newItemType == "Food")
                {
                    newItem.GetComponent<IconDrag>().itemType = ItemType.Food;
                }
                else if (newItemType == "Ingredient")
                {
                    newItem.GetComponent<IconDrag>().itemType = ItemType.Ingredient;
                }
                else if (newItemType == "Quest")
                {
                    newItem.GetComponent<IconDrag>().itemType = ItemType.Quest;
                }
                else if (newItemType == "Etc")
                {
                    newItem.GetComponent<IconDrag>().itemType = ItemType.Etc;
                }

                break;
            }
            else
            {
                slotCount++;
            }
        }

        if(slotCount >= 20)
        {
            //isInventoryFull
        }
    }
}
