using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public GameObject Player;

    private void FixedUpdate()
    {
        Camera.main.transform.position = Player.transform.position + new Vector3(0, 0, -10);
    }
}
