using System;
using UnityEngine;

public class SharedVariable<T> : ScriptableObject
{
    //You need to asign an initial and current values, because
    //scriptable objects doesnt reset their state on application quit
    [SerializeField] T initialValue, currentValue;

    //The public property to access to operate with the current value
    public T Value
    {
        get => currentValue;
        set
        {
            //if new value is different to the old one then set that value to the currentValue
            //and execute all functions subscribed to the OnValueChanged event
            if (!value.Equals(currentValue))
            {
                currentValue = value;
                OnValueChanged();
            }
        }
    }

    //the event that is called on when current value is changed
    public event Action OnValueChanged;


    //here is where the current value copy initial value on start
    private void OnEnable() => currentValue = initialValue;
}
