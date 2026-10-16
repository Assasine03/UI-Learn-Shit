using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public const float MaxHealth = 100f;

    [SerializeField, Range(0f, MaxHealth)] float health = MaxHealth;

    public float Health => health;
    public event Action<float> HealthChanged;

    public void TakeDamage(float damage)
    {
        SetHealth(health - Mathf.Max(0f, damage));
    }

    public void SetHealth(float value)
    {
        float nextHealth = Mathf.Clamp(value, 0f, MaxHealth);
        if (Mathf.Approximately(health, nextHealth))
        {
            return;
        }

        health = nextHealth;
        HealthChanged?.Invoke(health);
    }

    void OnValidate()
    {
        health = Mathf.Clamp(health, 0f, MaxHealth);
    }
}
