using UnityEngine;
using System.Collections;
using Cinemachine; // 添加 Cinemachine 命名空间

public class Word_LanlurenRangbu : MonoBehaviour
{
    [Header("Word Settings")]
    // public string correctAnswer; // 移除这个字段

    [Header("Effect Settings")]
    public GameObject successParticlePrefab;
    public float particleLifetime = 3f;    // 粒子特效存在时间

    [Header("Screen Shake Settings")]
    public float shakeAmount = 0.1f;       // 可在Inspector中调整晃动幅度
    public float shakeDuration = 0.2f;     // 晃动持续时间

    [Header("Collider Settings")]
    public Collider2D npcCollider;     // 添加碰撞体引用

    private Camera mainCamera;
    private NPC_lanluren npcLanluren;
    private bool dialogueEnded = false; // 添加标志位
    private CinemachineImpulseSource impulseSource; // 添加 CinemachineImpulseSource 引用

    void Start()
    {
        // if (string.IsNullOrEmpty(correctAnswer))
        // {
        //     Debug.LogError("correctAnswer 未设置");
        // }
        mainCamera = Camera.main;
        npcLanluren = FindObjectOfType<NPC_lanluren>();
        if (npcLanluren == null)
        {
            Debug.LogError("找不到 NPC_lanluren 脚本");
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
            npcCollider.isTrigger = true;
            other.gameObject.SetActive(false);
            npcLanluren.EndCurrentDialogue(); // 调用 NPC_lanluren 脚本关闭当前的对话
            npcLanluren.SwitchToDialoguePart("Lanlu_3");
        }
    }
}
