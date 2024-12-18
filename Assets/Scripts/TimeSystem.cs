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
        UpdateTime(Time.deltaTime); // 매 프레임마다 시간 업데이트

        //해와 시계
        Sun.GetComponent<Light2D>().intensity = Mathf.Lerp(0.2f, 1f, Mathf.PingPong(GameTime * 2, 1)); // 밝기 조정

        float degrees = GameTime * 360f; // 0도 ~ 360도 변환
        Clock.transform.localRotation = Quaternion.Euler(0, 0, -degrees - 180); // 시계 방향으로 회전
    }

    public void Initialize()
    {
        currentTime = 200f; // 초기 시간 설정
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
        Debug.Log("새로운 하루가 시작되었습니다!");
    }

    public void Sleep(float sleepDuration)
    {
        currentTime += sleepDuration;
        ProcessDayChange();
        Debug.Log($"잠자기 후 시간: {currentTime}초");
    }
}
