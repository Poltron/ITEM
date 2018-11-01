using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public class CoroutineRunner : MonoBehaviour
{
    public static event Action OnUpdate;

    private static CoroutineRunner m_host;
    private static CoroutineRunner Host
    {
        get
        {
            if (m_host != null) return m_host;
            else
            {
                GameObject coroutineRunner = new GameObject("CoroutineRunner");
                if (Application.isPlaying) DontDestroyOnLoad(coroutineRunner);
                m_host = coroutineRunner.AddComponent<CoroutineRunner>();
                return m_host;
            }
        }
    }

    private void Update()
    {
        if (OnUpdate != null) OnUpdate();
    }

    public static Coroutine Start(IEnumerator p_coroutine)
    {
        return Host.StartCoroutine(p_coroutine);
    }

    public static void Stop(Coroutine p_coroutine)
    {
        Host.StopCoroutine(p_coroutine);
    }

    public static void Invoke(UnityAction p_action, float p_timeInSeconds, MonoBehaviour p_host = null)
    {
        if (p_host == null) Start(_Invoke(p_action, p_timeInSeconds));
        else p_host.StartCoroutine(_Invoke(p_action, p_timeInSeconds));
    }

    private static IEnumerator _Invoke(UnityAction p_action, float p_timeInSeconds)
    {
        yield return new WaitForSeconds(p_timeInSeconds);
        p_action.Invoke();
    }

    public static void Invoke(IEnumerator p_coroutine, float p_timeInSeconds)
    { Start(_Invoke(p_coroutine, p_timeInSeconds)); }
    private static IEnumerator _Invoke(IEnumerator p_coroutine, float p_timeInSeconds)
    {
        yield return new WaitForSeconds(p_timeInSeconds);
        yield return p_coroutine;
    }
}
