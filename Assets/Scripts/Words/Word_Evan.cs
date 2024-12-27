using UnityEngine;
using System.Collections;

public class Word_Evan : MonoBehaviour
{
    [Header("Word Settings")]
    public string correctAnswer;

    [Header("Effect Settings")]
    public GameObject successParticlePrefab;
    public float particleLifetime = 3f;    // 粒子特效存在时间

    [Header("Screen Shake Settings")]
    public float shakeAmount = 0.1f;       // 可在Inspector中调整晃动幅度
    public float shakeDuration = 0.2f;     // 晃动持续时间

    private Camera mainCamera;

    void Start()
    {
        if (string.IsNullOrEmpty(correctAnswer))
        {
            Debug.LogError("correctAnswer 未设置");
        }
        mainCamera = Camera.main;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Word"))
        {
            if (PlayerManager.Instance.wordInHand == correctAnswer)
            {
                if (successParticlePrefab != null)
                {
                    GameObject particle = Instantiate(successParticlePrefab, transform.position, Quaternion.identity);
                    Destroy(particle, particleLifetime);  // 3秒后销毁粒子特效
                }
                StartCoroutine(ShakeCamera());
                other.gameObject.SetActive(false);
                WordPackageManager.Instance.AddWord("Evan");
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
