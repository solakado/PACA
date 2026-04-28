using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage = 10;

    // 防止一刀多次命中
    private bool hasHit = false;
    public PlayerAttack playerAttack;

    void OnEnable()
    {
        hasHit = false; // 每次攻击开始重置
        //playerAttack = GetComponent<PlayerAttack>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {

        if (hasHit) return;

        if (other.CompareTag("Boss"))
        {
            Debug.Log("碰到: " + other.name);
            //
            playerAttack.AddWaveCount(1);

            BossController boss = other.GetComponentInParent<BossController>();
            XuanWuController xuanwu = other.GetComponentInParent<XuanWuController>();
            ZhuQueController zhuque = other.GetComponentInParent<ZhuQueController>();
            BaiHuController baihu = other.GetComponentInParent<BaiHuController>();
            if (boss !=null)
            {
                boss.TakeDamage(damage);
            }
            if (xuanwu!=null)
            {
                xuanwu.TakeDamage(damage);
            }
            if (zhuque != null)
            {
                zhuque.TakeDamage(damage);
            }
            if (baihu != null)
            {
                baihu.TakeDamage(damage);
            }
            //Debug.Log("成功获取BossController");
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