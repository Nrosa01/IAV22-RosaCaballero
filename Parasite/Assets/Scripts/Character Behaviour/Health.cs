using System;
using UnityEngine;

[System.Serializable]
public class Health
{
    [SerializeField] private int startingHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private int minHealth;
    [SerializeField] private int currentHealth;

    public void SubstractHealth(int damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, IsBelowMinHealth);
    }

    public void AddHealth(int health)
    {
        currentHealth = Mathf.Min(currentHealth + health, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, IsBelowMinHealth);
    }

    public bool IsBelowMinHealth => currentHealth <= minHealth;

    public event Action<int, bool> OnHealthChanged;
}