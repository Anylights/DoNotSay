using UnityEngine;

public class UIFollow : MonoBehaviour
{
    private Vector3 initialScale; // 初始缩放
    private Transform parentTransform; // 父物体的 Transform

    void Start()
    {
        // 记录初始缩放
        initialScale = transform.localScale;
        // 记录父物体的 Transform
        parentTransform = transform.parent;
    }

    void LateUpdate()
    {
        // 计算父物体的缩放因子
        Vector3 parentScale = parentTransform != null ? parentTransform.lossyScale : Vector3.one;
        // 保持初始缩放
        transform.localScale = new Vector3(
            initialScale.x / parentScale.x,
            initialScale.y / parentScale.y,
            initialScale.z / parentScale.z
        );
    }
}