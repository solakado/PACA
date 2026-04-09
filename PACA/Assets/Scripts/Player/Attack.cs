using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage = 10;

    // 防止一刀多次命中
    private bool hasHit = false;

    void OnEnable()
    {
        hasHit = false; // 每次攻击开始重置
    }


    void OnTriggerEnter2D(Collider2D other)
    {

        if (hasHit) return;

        if (other.CompareTag("Boss"))
        {
            //Debug.Log("碰到: " + other.name);

            BossController boss = other.GetComponentInParent<BossController>();

            if (boss == null)
            {
                //Debug.LogError("没找到BossController！！");
                return;
            }

            //Debug.Log("成功获取BossController");

            boss.TakeDamage(damage);
            hasHit = true;
        }
        //if (hasHit) return;

        //if (other.CompareTag("Boss"))
        //{
        //    // 改成找父物体（关键优化）
        //    BossController boss = other.GetComponentInParent<BossController>();
        //    Debug.Log("碰到: " + other.name);

        //    if (boss != null)
        //    {
        //        Debug.Log("打到Boss: " + boss.name);

        //        boss.TakeDamage(damage);

        //        hasHit = true; // 防止这一刀重复命中
        //    }
        //}
    }
}