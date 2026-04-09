using UnityEngine;

public class PlayerHurtBox : MonoBehaviour
{
    private PlayerRespawn player;

    void Awake()
    {
        player = GetComponentInParent<PlayerRespawn>();
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

        // boss（支持子物体）
        BossController boss = other.GetComponentInParent<BossController>();

        if (boss != null)
        {
            player.Die();
        }
    }
}