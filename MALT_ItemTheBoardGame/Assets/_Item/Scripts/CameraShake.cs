﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform _transform;

    private bool isShaking;
    private float shakeTimeLeft;

    [SerializeField]
    private float magnitude;
    [SerializeField]
    private float frequency;


    private void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        if (isShaking)
        {
            Vector2 movement = PerlinShake();
            _transform.localPosition = new Vector3(movement.x, movement.y,-10);
        }
    }

    public void Shake(float _time, float _magnitude, float _frequency)
    {
        shakeTimeLeft = _time;
        magnitude = _magnitude;
        frequency = _frequency;

        isShaking = true;
        StartCoroutine(shakeTimer());
    }

    private Vector2 PerlinShake()
    {
        Vector2 result;
        float seed = Time.time * frequency;
        result.x = Mathf.Clamp01(Mathf.PerlinNoise(seed, 0f)) - 0.5f;
        result.y = Mathf.Clamp01(Mathf.PerlinNoise(0f, seed)) - 0.5f;
        result = result * magnitude;
        return result;
    }

    IEnumerator shakeTimer()
    {
        while (shakeTimeLeft > 0)
        {
            yield return new WaitForEndOfFrame();
            shakeTimeLeft -= Time.deltaTime;
        }

        isShaking = false;
        _transform.localPosition = new Vector3(0, 0, -10);
    }

}