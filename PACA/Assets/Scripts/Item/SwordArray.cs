using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordArray : MonoBehaviour
{
   
    void OnTriggerEnter2D(Collider2D other)
    {

        //Debug.Log("碰到" + other.name);
        // 碰到玩家 或 非Boss物体时销毁
        if (other.CompareTag("isGround"))
        {
            Destroy(gameObject);
        }
    }
}
