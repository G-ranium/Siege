using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class IEnumeratorCountdown : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 1f;

    [Header("Event fired every cycle")]
    [SerializeField] private UnityEvent onCountdownEnd;

    private Coroutine loopRoutine;

    public void Awake()
    {
        StartCounting();
    }

    // Start the infinite loop
    public void StartCounting()
    {
        if (loopRoutine == null)
            loopRoutine = StartCoroutine(GenerateLoop());
    }

    // Stop the infinite loop (tower dies, sold, disabled, etc.)
    public void StopCounting()
    {
        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }
    }

    private IEnumerator GenerateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(delaySeconds);
            onCountdownEnd?.Invoke();
        }
    }
}