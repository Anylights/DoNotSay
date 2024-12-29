using UnityEngine;
using System.Collections;
using TMPro;
using Cinemachine; // 添加 Cinemachine 命名空间

public class Word_Zhidian : MonoBehaviour
{
    public NPCManager npc;

    public DialogueLine dialogueLine;

    public Collider2D Fengzi_collider;

    public Collider2D Fengzi_Dialogue_collider;
    public SpriteRenderer Fengzi_sprite;

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
            AudioManager.Instance.Play("Word_break");
            impulseSource.GenerateImpulse(); // 使用 Cinemachine 生成脉冲以实现摄像机震动
            other.gameObject.SetActive(false);
            npc.EndCurrentDialogue(); // 调用 NPC_zhiwen 脚本关闭当前的对话

            if (dialogueLine.dialogueText == "<color=red><shake>他似乎就在附近</>")
            {
                dialogueLine.dialogueText = "<color=red><shake>他似乎不在附近</>";
                Fengzi_collider.enabled = false;
                Fengzi_sprite.enabled = false;
                Fengzi_Dialogue_collider.enabled = false;
            }
            else
            {
                dialogueLine.dialogueText = "<color=red><shake>他似乎就在附近</>";
                Fengzi_collider.enabled = true;
                Fengzi_sprite.enabled = true;
                Fengzi_Dialogue_collider.enabled = true;
            }
        }
    }
}
