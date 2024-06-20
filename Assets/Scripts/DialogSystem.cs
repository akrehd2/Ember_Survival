using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    public static DialogSystem instance;

    public GameObject nowNPC;

    public bool coroutineIsRunning;

    public GameObject dialog;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogText;

    public string nameString;
    public string dialogString;

    public float lodingDelay;

    private void Awake()
    {
        instance = this;
    }

    //Init Dialog
    IEnumerator DialogInit()
    {
        coroutineIsRunning = true;

        dialog.SetActive(true);

        nameText.text = nameString;
        dialogText.text = "";

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i <= dialogString.Length; i++)
        {
            dialogText.text = dialogString.Substring(0, i);

            yield return new WaitForSeconds(lodingDelay);
        }

        coroutineIsRunning = false;
    }

    public void InitDialog()
    {
        StartCoroutine(DialogInit());
    }

    //next Dialog
    IEnumerator DialogLoding()
    {
        coroutineIsRunning = true;

        nameText.text = nameString;
        dialogText.text = "";

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i <= dialogString.Length; i++)
        {
            dialogText.text = dialogString.Substring(0, i);

            yield return new WaitForSeconds(lodingDelay);
        }

        coroutineIsRunning = false;
    }

    public void StartDialog()
    {
        StartCoroutine(DialogLoding());
    }

    //Button to Next
    public void ButtonToNextDialog()
    {
        nowNPC.GetComponent<DialogSet>().NextDialog();
    }

    //Finish Dialog
    public void FinishDialog()
    {
        //다이얼로그 종료 애니메이션
        dialog.GetComponent<Animator>().SetBool("Reverse", true);
        Invoke("OffDialog", 0.3f);
    }
    
    void OffDialog()
    {
        nowNPC.GetComponent<NpcCtrl>().delay = 0;

        nowNPC = null;

        dialog.SetActive(false);

        PlayerCtrl.instance.delay = 0;

        StopAllCoroutines();
        coroutineIsRunning = false;
    }
}