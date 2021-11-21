using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Health health;
    Slider healthSlider;

    void Start()
    {
        health = GetComponentInParent<Health>();
        healthSlider = GetComponent<Slider>();
        healthSlider.maxValue = health.MaxHealth;
        healthSlider.value = health.MaxHealth;

        health.ClientOnHealthUpdate += SetCurrentHealth;
    }

    void OnDestroy()
    {
        health.ClientOnHealthUpdate -= SetCurrentHealth;
    }

    void SetCurrentHealth(int currentHealthValue, int maxHealth)
    {
        healthSlider.value = (float)currentHealthValue;
    }
}
