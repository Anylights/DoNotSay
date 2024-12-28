using UnityEngine;
using System.Collections;

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

    [Header("Rigidbody Settings")]
    public Rigidbody2D npcRigidbody; // 让用户在 Inspector 中引用

    private Camera mainCamera;
    private NPC_lanluren npcLanluren;
    private bool dialogueEnded = false; // 添加标志位

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
        if (npcRigidbody == null)
        {
            Debug.LogError("Rigidbody2D 未设置");
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
            npcLanluren.EndCurrentDialogue(); // 调用 NPC_lanluren 脚本关闭当前的对话
            npcLanluren.SwitchToDialoguePart("Lanlu_3");
            if (npcRigidbody != null)
            {
                npcRigidbody.bodyType = RigidbodyType2D.Dynamic; // 将 Rigidbody2D 的 BodyType 改成 Dynamic
            }
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
