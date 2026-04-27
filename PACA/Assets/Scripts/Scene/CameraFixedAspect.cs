using UnityEngine;

[ExecuteAlways]
public class CameraFixedAspect : MonoBehaviour
{
    public float designWidth = 1280;
    public float designHeight = 720;

    void Awake()
    {
        Camera cam = GetComponent<Camera>();

        float targetAspect = designWidth / designHeight;
        cam.aspect = targetAspect;
    }
}