using UnityEngine;
using System.Collections;
using TMPro;
using Cinemachine; // 添加 Cinemachine 命名空间

public class Shengwu : MonoBehaviour
{
    [Header("Effect Settings")]
    public GameObject successParticlePrefab;
    public float particleLifetime = 3f;    // 粒子特效存在时间
    private Collider2D collider;

    private SpriteRenderer sprite; // 修正类型名为 SpriteRenderer

    [Header("Screen Shake Settings")]
    public float shakeAmount = 0.5f;       // 可在Inspector中调整晃动幅度
    public float shakeDuration = 0.2f;     // 晃动持续时间

    private Camera mainCamera;

    private CinemachineImpulseSource impulseSource; // 添加 CinemachineImpulseSource 引用

    void Start()
    {
        mainCamera = Camera.main;
        impulseSource = GetComponent<CinemachineImpulseSource>(); // 初始化 impulseSource
        collider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
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
            AudioManager.Instance.Play("End");
            impulseSource.GenerateImpulse(); // 使用 Cinemachine 生成脉冲以实现摄像机震动
            other.gameObject.SetActive(false);

            StartCoroutine(SlowTimeCoroutine()); // 开始时间缩放协程
            collider.enabled = false;
            sprite.enabled = false;
        }
    }

    private IEnumerator SlowTimeCoroutine()
    {
        float targetScale = 0.05f;
        float duration = 1f; // 线性缩放的持续时间
        float elapsed = 0f;
        float initialScale = Time.timeScale;

        // 线性缩放时间到0.2f
        while (elapsed < duration)
        {
            Time.timeScale = Mathf.Lerp(initialScale, targetScale, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = targetScale;

        // 等待10秒真实时间
        yield return new WaitForSecondsRealtime(10f);

        // 线性恢复时间到1f
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
