using UnityEngine;
using System.Collections;
using TMPro;
using Cinemachine; // 添加 Cinemachine 命名空间

public class Word_Zhiwen : MonoBehaviour
{
    [Header("Effect Settings")]
    public GameObject successParticlePrefab;
    public float particleLifetime = 3f;    // 粒子特效存在时间

    [Header("Screen Shake Settings")]
    public float shakeAmount = 0.5f;       // 可在Inspector中调整晃动幅度
    public float shakeDuration = 0.2f;     // 晃动持续时间

    [Header("Collider Settings")]
    public Collider2D ZhiwenCollider;      // 添加碰撞体引用

    private Camera mainCamera;
    private NPC_zhiwen npcZhiwen;
    private bool dialogueEnded = false; // 添加标志位
    private CinemachineImpulseSource impulseSource; // 添加 CinemachineImpulseSource 引用

    void Start()
    {
        mainCamera = Camera.main;
        npcZhiwen = FindObjectOfType<NPC_zhiwen>();
        if (npcZhiwen == null)
        {
            Debug.LogError("找不到 NPC_zhiwen 脚本");
        }
        impulseSource = GetComponent<CinemachineImpulseSource>(); // 初始化 impulseSource
        if (impulseSource == null)
        {
            Debug.LogError("CinemachineImpulseSource 组件未找到");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Word") && !dialogueEnded)
        {
            dialogueEnded = true; // 设置标志位
            if (successParticlePrefab != null)
            {
                GameObject particle = Instantiate(successParticlePrefab, transform.position, Quaternion.identity);
                Destroy(particle, particleLifetime);  // 3秒后销毁粒子特效
            }
            AudioManager.Instance.Play("Word_break");
            impulseSource.GenerateImpulse(); // 使用 Cinemachine 生成脉冲以实现摄像机震动
            other.gameObject.SetActive(false);
            npcZhiwen.EndCurrentDialogue(); // 调用 NPC_zhiwen 脚本关闭当前的对话
            npcZhiwen.SwitchToDialoguePart("Zhiwen_4");
            ZhiwenCollider.isTrigger = true;
        }
    }
}
