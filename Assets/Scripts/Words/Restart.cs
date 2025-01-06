using UnityEngine;
using System.Collections;
using TMPro;
using Cinemachine; // 添加 Cinemachine 命名空间
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{

    public SpriteRenderer finalSprite;

    public Collider2D ShengWuCollider;

    public SpriteRenderer ShengWuSprite;



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

            // 精灵为全透明
            if (finalSprite != null)
            {
                Color c = finalSprite.color;
                c.a = 0f;
                finalSprite.color = c;
            }

            ShengWuCollider.enabled = true;
            ShengWuSprite.enabled = true;

            EventCenter.Instance.TriggerEvent("RestartTriggered");
            AudioManager.Instance.Stop("Final");
            AudioManager.Instance.Play("End");
            impulseSource.GenerateImpulse(); // 使用 Cinemachine 生成脉冲以实现摄像机震动
            other.gameObject.SetActive(false);
            npc.EndCurrentDialogue();

            StartCoroutine(SlowTimeCoroutine()); // 开始时间缩放协程
        }
    }

    private IEnumerator SlowTimeCoroutine()
    {
        float targetScale = 0.05f;
        float durationFirst = 1f;
        float elapsedFirst = 0f;
        float initialScale = Time.timeScale;

        while (elapsedFirst < durationFirst)
        {
            Time.timeScale = Mathf.Lerp(initialScale, targetScale, elapsedFirst / durationFirst);
            elapsedFirst += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = targetScale;

        yield return new WaitForSecondsRealtime(3f);

        Time.timeScale = 1f;
    }
}
