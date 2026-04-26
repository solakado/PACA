using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public Action<int, int> onHealthChanged; // (当前血量, 最大血量)
    private HealthUI healthUI;
    public bool isInvincible = false;
    public float invincibleTime = 1f;
    private PlayerRespawn respawn;
    private FlashController flash;
    void Start()
    {
        currentHealth = maxHealth;
        respawn = GetComponent<PlayerRespawn>();
        healthUI = FindObjectOfType<HealthUI>();
        flash = GetComponent<FlashController>();

        healthUI.Init(maxHealth);
        onHealthChanged += healthUI.UpdateHealth;

        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibleCoroutine());
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        if (respawn != null)
        {
            respawn.Die();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    //private System.Collections.IEnumerator InvincibleCoroutine()
    //{
    //    isInvincible = true;



    //    float timer = 0f;


    //    FlashController flash = GetComponent<FlashController>();

    //    while (timer < invincibleTime)
    //    {
    //        flash.Flash(0.5f, 0.1f);
    //        yield return new WaitForSeconds(0.2f);
    //    }
    //    isInvincible = false;
    //}
    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        float timer = 0f;

        while (timer < invincibleTime)
        {
            if (flash != null)
            {
                flash.Flash(0.5f, 0.1f);
            }

            yield return new WaitForSeconds(0.2f);

            timer += 0.2f;
        }

        isInvincible = false;
    }
}
