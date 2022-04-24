using UnityEngine;
using System;

public enum AmountOperation { AddMaxAmount, TrySubstractMaxAmount, TryAddAmount, TrySubstractAmount, TryBlockAmount, TryUnBlockAmount }

[CreateAssetMenu(menuName = "SharedVariables/SharedAmount")]
public class BlockableAmount : ScriptableObject
{
    [SerializeField]  private ReactiveInt3 initialSharedAmount; //Se usa para reestablecer el scriptable tras haber sido modidficado
    [HideInInspector] private ReactiveInt3 currentSharedAmount;

    //Se ejecuta cuando cambia un valor de currentSharedAmount
    public event Action OnValueChanged
    {
        add
        {
            currentSharedAmount.OnValueChanged += value;
        }

        remove
        {
            currentSharedAmount.OnValueChanged -= value;
        }
    }

    private void OnEnable() => currentSharedAmount = initialSharedAmount;

    public int maxAmount { get => currentSharedAmount.maxAmountVal; private set { } }
    public int currentAmount { get => currentSharedAmount.currentAmountVal; private set { } }
    public int blockedAmount { get => currentSharedAmount.blockedAmountVal; private set { } }

    public bool AddMaxAmount()
    {
        currentSharedAmount.maxAmountVal++;
        return true;
    }
    public bool TrySubstractMaxAmount()
    {
        if (currentSharedAmount.maxAmountVal > 1)
        {
            currentSharedAmount.maxAmountVal--;

            if (currentSharedAmount.currentAmountVal == currentSharedAmount.maxAmountVal - currentSharedAmount.blockedAmountVal)
                currentSharedAmount.blockedAmountVal--;

            return true;
        }
        else return false;
    }

    public bool TryAddAmount()
    {
        if (currentSharedAmount.currentAmountVal == currentSharedAmount.maxAmountVal - currentSharedAmount.blockedAmountVal) return false;
        currentSharedAmount.currentAmountVal++;
        return true;
    }

    public bool TrySubstractAmount()
    {
        if (currentSharedAmount.currentAmountVal <= 0) return false;
        currentSharedAmount.currentAmountVal--;
        return true;
    }

    public bool TryBlockAmount()
    {
        if (currentSharedAmount.blockedAmountVal < currentSharedAmount.maxAmountVal)
        {
            currentSharedAmount.blockedAmountVal++;
            return true;
        }
        else return false;
    }

    public bool TryUnBlockAmount()
    {
        if (currentSharedAmount.blockedAmountVal == 0) return false;
        currentSharedAmount.blockedAmountVal--;
        return true;
    }
}

[Serializable]
public struct ReactiveInt3
{
    [SerializeField] private int maxAmount;
    [SerializeField] private int currentAmount;
    [SerializeField] private int blockedAmount;

    public int currentAmountVal
    {
        get => currentAmount;
        set
        {
            if (value != currentAmount)
            {
                currentAmount = value;
                OnValueChanged();
            }
        }
    }

    public int maxAmountVal
    {
        get => maxAmount;
        set
        {
            if (value != maxAmount)
            {
                maxAmount = value;
                OnValueChanged();
            }
        }
    }

    public int blockedAmountVal
    {
        get => blockedAmount;
        set
        {
            if (value != blockedAmount)
            {
                blockedAmount = value;
                OnValueChanged();
            }
        }
    }

    //the event that is called on when current value is changed
    public Action OnValueChanged;
}