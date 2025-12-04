using UnityEngine;
using UnityEngine.Events;

public class IntValueCheckerBehavior : MonoBehaviour
{
    public UnityEvent trueEvent, falseEvent;

    [SerializeField] private IntData value1;

    [Tooltip("Value 2 or intValue are optional and the program defaults to 0 if no value is entered.")]
    [SerializeField] private IntData value2;

    [SerializeField] private int intValue;

    public enum InputMode
    {
        UseValue2,
        UseIntValue
    }

    [Header("Integer/IntData")]
    [SerializeField] private InputMode inputMode;
    

    private int GetSecondValue()
    {
        switch (inputMode)
        {
            case InputMode.UseValue2:
                return value2 != null ? value2.Value : 0;

            case InputMode.UseIntValue:
                return intValue;

            default:
                return 0;
        }
    }

    public void CompareValues()
    {
        int compareValue = value1.Value;
        int compareValue2 = GetSecondValue();
        
        if(compareValue > compareValue2)
            trueEvent.Invoke();
        else
            falseEvent.Invoke();
    }
}