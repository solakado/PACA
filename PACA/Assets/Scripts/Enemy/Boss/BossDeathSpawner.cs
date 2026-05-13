using UnityEngine;

public class BossDeathSpawner : MonoBehaviour
{
    [Header("两个Boss")]
    public GameObject boss1;
    public GameObject boss2;

    [Header("要生成的物体")]
    public GameObject spawnPrefab;

    [Header("指定生成位置")]
    public Vector3 spawnPos;

    private bool hasSpawned = false;

    void Update()
    {
        if (hasSpawned) return;

        bool boss1Dead = boss1 == null || !boss1.activeSelf;
        bool boss2Dead = boss2 == null || !boss2.activeSelf;

        if (boss1Dead && boss2Dead)
        {
            // 在你指定的spawnPos生成
            Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
            hasSpawned = true;
            Debug.Log("两个Boss全部死亡，指定位置生成物体");
        }
    }
}