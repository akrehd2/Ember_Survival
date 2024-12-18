using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem instance { get; private set; }

    private const float DAY_LENGTH = 600f;
    private float currentTime;

    public float GameTime => currentTime / DAY_LENGTH;

    public GameObject Sun;
    public GameObject Clock;

    private void Awake()
    {
        instance = this;

        Initialize();
    }

    private void Update()
    {
        UpdateTime(Time.deltaTime); // �� �����Ӹ��� �ð� ������Ʈ

        //�ؿ� �ð�
        Sun.GetComponent<Light2D>().intensity = Mathf.Lerp(0.2f, 1f, Mathf.PingPong(GameTime * 2, 1)); // ��� ����

        float degrees = GameTime * 360f; // 0�� ~ 360�� ��ȯ
        Clock.transform.localRotation = Quaternion.Euler(0, 0, -degrees - 180); // �ð� �������� ȸ��
    }

    public void Initialize()
    {
        currentTime = 200f; // �ʱ� �ð� ����
    }

    public void UpdateTime(float deltaTime)
    {
        currentTime += deltaTime;
        ProcessDayChange();
    }

    private void ProcessDayChange()
    {
        if (currentTime >= DAY_LENGTH)
        {
            int daysPassed = Mathf.FloorToInt(currentTime / DAY_LENGTH);
            currentTime = currentTime % DAY_LENGTH;

            for (int i = 0; i < daysPassed; i++)
            {
                OnNewDay();
            }
        }
    }

    private void OnNewDay()
    {
        Debug.Log("���ο� �Ϸ簡 ���۵Ǿ����ϴ�!");
    }

    public void Sleep(float sleepDuration)
    {
        currentTime += sleepDuration;
        ProcessDayChange();
        Debug.Log($"���ڱ� �� �ð�: {currentTime}��");
    }
}
