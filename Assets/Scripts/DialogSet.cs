using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogSystem;

public class DialogSet : MonoBehaviour
{
    public DialogDataStruct[] dialogDataList;

    public int nowIndex;

    public string[] nameS;
    public string[] dialogS;

    public void SetDialog()
    {
        nowIndex = 0;

        DialogSystem.instance.nowNPC = gameObject;

        DialogSystem.instance.nameString = nameS[nowIndex];
        DialogSystem.instance.dialogString = dialogS[nowIndex];

        DialogSystem.instance.InitDialog();
    }

    public void NextDialog()
    {
        if (DialogSystem.instance.coroutineIsRunning == false)
        {
            nowIndex++;

            if (nowIndex != nameS.Length)
            {
                DialogSystem.instance.nameString = nameS[nowIndex];
                DialogSystem.instance.dialogString = dialogS[nowIndex];

                DialogSystem.instance.StartDialog();
            }
            else //stopDialog
            {
                nowIndex = 0;

                DialogSystem.instance.FinishDialog();
            }
        }
        else
        {
            DialogSystem.instance.StopAllCoroutines();

            DialogSystem.instance.dialogText.text = dialogS[nowIndex];

            DialogSystem.instance.coroutineIsRunning = false;
        }
    }
}
