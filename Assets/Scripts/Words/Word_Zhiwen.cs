using UnityEngine;
using System.Collections;

public class Word_Zhiwen : MonoBehaviour
{
    [Header("Effect Settings")]
    public GameObject successParticlePrefab;
    public float particleLifetime = 3f;    // 粒子特效存在时间

    [Header("Screen Shake Settings")]
    public float shakeAmount = 0.1f;       // 可在Inspector中调整晃动幅度
    public float shakeDuration = 0.2f;     // 晃动持续时间

    private Camera mainCamera;
    private NPC_zhiwen npcZhiwen;
    private bool dialogueEnded = false; // 添加标志位

    void Start()
    {
        mainCamera = Camera.main;
        npcZhiwen = FindObjectOfType<NPC_zhiwen>();
        if (npcZhiwen == null)
        {
            Debug.LogError("找不到 NPC_zhiwen 脚本");
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
            StartCoroutine(ShakeCamera());
            other.gameObject.SetActive(false);
            npcZhiwen.EndCurrentDialogue(); // 调用 NPC_zhiwen 脚本关闭当前的对话
            npcZhiwen.SwitchToDialoguePart("Zhiwen_2");
        }
    }

    private IEnumerator ShakeCamera()
    {
        Vector3 originalPosition = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            mainCamera.transform.position = new Vector3(
                originalPosition.x + x,
                originalPosition.y + y,
                originalPosition.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPosition;
    }
}
