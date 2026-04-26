using UnityEngine;
using System.Collections;

public class FlashController : MonoBehaviour
{
    private Material[] materials;

    void Awake()
    {
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();

        materials = new Material[srs.Length];

        for (int i = 0; i < srs.Length; i++)
        {
            // 实例化材质（避免全局污染）
            materials[i] = srs[i].material;
        }
    }

    public void Flash(float intensity, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine(intensity, duration));
    }

    IEnumerator FlashCoroutine(float intensity, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float t = 1 - (timer / duration);

            foreach (var mat in materials)
            {
                mat.SetFloat("_FlashAmount", t * intensity);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        foreach (var mat in materials)
        {
            mat.SetFloat("_FlashAmount", 0f);
        }
    }
}