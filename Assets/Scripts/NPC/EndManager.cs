using System.Collections;
using UnityEngine;

public class EndManager : AutoNPCManager
{
    // 用于延时启用的碰撞体

    // 用于 0~1 透明度渐变的精灵
    public SpriteRenderer finalSprite;

    public AudioSource Ambient;
    public AudioSource Ambient2;

    private void OnEnable()
    {
        EventCenter.Instance.Subscribe("TrueEndTriggered", OnTrueEndTriggered);
        EventCenter.Instance.Subscribe("FakeEndTriggered", OnFakeEndTriggered);
        EventCenter.Instance.Subscribe("RestartTriggered", OnRestartTriggered);
    }

    private void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("TrueEndTriggered", OnTrueEndTriggered);
        EventCenter.Instance.Unsubscribe("FakeEndTriggered", OnFakeEndTriggered);
        EventCenter.Instance.Unsubscribe("RestartTriggered", OnRestartTriggered);
    }

    private void OnTrueEndTriggered()
    {
        StartCoroutine(PlayEndSequence("TrueEnd"));
    }

    private void OnFakeEndTriggered()
    {
        StartCoroutine(PlayEndSequence("FakeEnd"));
    }

    private void OnRestartTriggered()
    {
        StopAllCoroutines();
        if (finalSprite != null)
        {
            Color c = finalSprite.color;
            c.a = 0f;
            finalSprite.color = c;
        }
        StartCoroutine(RestoreVolumesCoroutine());
    }

    private IEnumerator RestoreVolumesCoroutine()
    {
        // 等待 3 秒（现实时间）
        yield return new WaitForSecondsRealtime(3f);

        float duration = 1f;
        float elapsed = 0f;
        float startVol = Ambient ? Ambient.volume : 0f;
        float startVol2 = Ambient2 ? Ambient2.volume : 0f;

        // 在 1 秒内将音量从当前值渐变到目标值
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (Ambient != null) Ambient.volume = Mathf.Lerp(startVol, 0.3f, t);
            if (Ambient2 != null) Ambient2.volume = Mathf.Lerp(startVol2, 1f, t);

            yield return null;
        }
    }

    private IEnumerator PlayEndSequence(string dialoguePartName)
    {

        // 精灵初始化为全透明
        if (finalSprite != null)
        {
            Color c = finalSprite.color;
            c.a = 0f;
            finalSprite.color = c;
        }

        // 初始化 Ambient 音量
        float initialVolume = Ambient.volume;

        float initialVolume2 = Ambient2.volume;
        // 等待 PlayEndSequence 的剩余逻辑（透明度渐变和碰撞体启用）
        float timer = 0f;
        while (timer < 10f)
        {
            timer += Time.unscaledDeltaTime; // 使用 unscaledDeltaTime 以基于现实时间

            // 在 5~8 秒之间进行透明度渐变
            if (timer >= 5f && timer <= 8f && finalSprite != null)
            {
                float t = Mathf.InverseLerp(5f, 8f, timer); // 计算插值从0到1
                Color c = finalSprite.color;
                c.a = t;
                finalSprite.color = c;
            }

            // 在 0~10 秒之间将 Ambient 音量降到 0
            if (Ambient != null)
            {
                Ambient.volume = Mathf.Lerp(initialVolume, 0f, timer / 10f);
            }
            if (Ambient2 != null)
            {
                Ambient2.volume = Mathf.Lerp(initialVolume, 0f, timer / 10f);
            }

            yield return null;
        }

        // 切换到指定的对话部分
        SwitchToDialoguePart(dialoguePartName);

        // 开始自动播放对话
        StartCoroutine(AutoNextLineCoroutine());

        AudioManager.Instance.Play("Final");
    }

    protected override void OnAllLinesDisplayed()
    {
        base.OnAllLinesDisplayed();
        // 如果当前对话为 TrueEnd 或 FakeEnd，切换到 Caidan
        if (currentPart != null &&
           (currentPart.partName == "TrueEnd" || currentPart.partName == "FakeEnd"))
        {
            SwitchToDialoguePart("Caidan");
            StartCoroutine(AutoNextLineCoroutine());
        }
        else
        {
            // 不进行任何操作，保持最后一句对话显示
        }
    }

    // ...existing code...
}
