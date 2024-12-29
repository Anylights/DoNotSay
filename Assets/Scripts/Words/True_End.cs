using UnityEngine;
using System.Collections;
using TMPro;
using Cinemachine; // 添加 Cinemachine 命名空间

public class True_End : MonoBehaviour
{
    [Header("NPC Type Settings")]
    public AutoNPCManager npc;

    [Header("Effect Settings")]
    public GameObject successParticlePrefab;
    public float particleLifetime = 3f;    // 粒子特效存在时间

    [Header("Screen Shake Settings")]
    public float shakeAmount = 0.5f;       // 可在Inspector中调整晃动幅度
    public float shakeDuration = 0.2f;     // 晃动持续时间

    private Camera mainCamera;
    private CinemachineImpulseSource impulseSource; // 添加 CinemachineImpulseSource 引用

    void Start()
    {
        mainCamera = Camera.main;
        impulseSource = GetComponent<CinemachineImpulseSource>(); // 初始化 impulseSource
        if (impulseSource == null)
        {
            Debug.LogError("CinemachineImpulseSource 组件未找到");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Word"))
        {
            if (successParticlePrefab != null)
            {
                GameObject particle = Instantiate(successParticlePrefab, transform.position, Quaternion.identity);
                Destroy(particle, particleLifetime);  // 3秒后销毁粒子特效
            }
            EventCenter.Instance.TriggerEvent("TrueEndTriggered");
            AudioManager.Instance.Play("End");
            impulseSource.GenerateImpulse(); // 使用 Cinemachine 生成脉冲以实现摄像机震动
            other.gameObject.SetActive(false);
            npc.EndCurrentDialogue(); // 调用 NPC_zhiwen 脚本关闭当前的对话

            StartCoroutine(SlowTimeCoroutine()); // 开始时间缩放协程
        }
    }

    private IEnumerator SlowTimeCoroutine()
    {
        float targetScale = 0.05f;
        float duration = 1f;
        float elapsed = 0f;
        float initialScale = Time.timeScale;

        // 线性缩放至 targetScale
        while (elapsed < duration)
        {
            Time.timeScale = Mathf.Lerp(initialScale, targetScale, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = targetScale;

        // 等待10秒真实时间
        yield return new WaitForSecondsRealtime(10f);

        // 线性恢复到1
        elapsed = 0f;
        while (elapsed < duration)
        {
            Time.timeScale = Mathf.Lerp(targetScale, 1f, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1f;
    }
}
