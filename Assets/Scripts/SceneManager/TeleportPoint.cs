using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    [Header("Teleport Settings")]
    public TeleportPoint targetPoint;  // 目标传送点
    public bool isActive = true;       // 传送点是否激活

    [Header("Visual Settings")]
    public Color gizmoColor = Color.blue;  // 在Scene视图中的显示颜色

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player") && targetPoint != null)
        {
            // 传送玩家到目标点
            other.transform.position = targetPoint.transform.position;
        }
    }

    // 在Scene视图中绘制可视化标记
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (targetPoint != null)
        {
            // 绘制一条线连接到目标点
            Gizmos.DrawLine(transform.position, targetPoint.transform.position);

            // 绘制箭头指向目标点
            Vector3 direction = (targetPoint.transform.position - transform.position).normalized;
            Vector3 midPoint = Vector3.Lerp(transform.position, targetPoint.transform.position, 0.5f);
            float arrowSize = 0.3f;

            Vector3 right = Quaternion.Euler(0, 0, 45) * -direction * arrowSize;
            Vector3 left = Quaternion.Euler(0, 0, -45) * -direction * arrowSize;

            Gizmos.DrawLine(midPoint, midPoint + right);
            Gizmos.DrawLine(midPoint, midPoint + left);
        }
    }
}