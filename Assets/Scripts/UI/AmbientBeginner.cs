using System.Collections;
using UnityEngine;

public class AmbientBeginner : MonoBehaviour
{
    // ...existing code...

    private AudioSource audioSource;

    void Start()
    {
        // 获取 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        // 初始化音量为0
        audioSource.volume = 0f;
        // 开始音量渐变协程
        StartCoroutine(FadeInVolume());
    }

    private IEnumerator FadeInVolume()
    {
        // 等待10秒
        yield return new WaitForSeconds(10f);

        float duration = 5f; // 渐变持续时间
        float elapsed = 0f;
        float startVolume = audioSource.volume;
        float targetVolume = 1f;

        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    // ...existing code...
}
