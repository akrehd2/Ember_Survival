using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICtrl : MonoBehaviour
{
    //Portrait
    public Image FaceRenderer;

    public Sprite[] FaceSprites;

    //Slider
    public TextMeshProUGUI HpText;
    public TextMeshProUGUI HgText;
    public TextMeshProUGUI StText;
    public Slider HpSlider;
    public Slider HgSlider;
    public Slider StSlider;

    private void Update()
    {
        PortraitUiCtrl();
        SliderUiCtrl();
    }

    void PortraitUiCtrl()
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

    void SliderUiCtrl()
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
}
