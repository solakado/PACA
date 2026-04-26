using UnityEngine;

public class PlayerHurtBox : MonoBehaviour
{
    private PlayerRespawn player;

    private PlayerHealth health;
    //private float lastHitTime = 0f;   //  上次受伤时间
    //public float damageInterval = 0.5f; //  冷却时间

    void Awake()
    {
        player = GetComponentInParent<PlayerRespawn>();
        health = GetComponentInParent<PlayerHealth>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (player == null || player.isDead) return;

        Debug.Log("HurtBox碰到: " + other.name);

        // trap
        if (other.CompareTag("trap"))
        {
            player.Die();
            return;
        }

        //boss（支持子物体）

        if (other.CompareTag("Boss"))
        {
            if (health != null)
            {

                health.TakeDamage(1);

            }
        }


    }

}