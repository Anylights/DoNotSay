using System.Collections;
using UnityEngine;

public class EndManager : AutoNPCManager
{
    // 用于延时启用的碰撞体
    public Collider2D finalCollider;
    // 用于 0~1 透明度渐变的精灵
    public SpriteRenderer finalSprite;

    public AudioSource Ambient;

    private void OnEnable()
    {
        EventCenter.Instance.Subscribe("TrueEndTriggered", OnTrueEndTriggered);
        EventCenter.Instance.Subscribe("FakeEndTriggered", OnFakeEndTriggered);
    }

    private void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("TrueEndTriggered", OnTrueEndTriggered);
        EventCenter.Instance.Unsubscribe("FakeEndTriggered", OnFakeEndTriggered);
    }

    private void OnTrueEndTriggered()
    {
        StartCoroutine(PlayEndSequence("TrueEnd"));
    }

    private void OnFakeEndTriggered()
    {
        StartCoroutine(PlayEndSequence("FakeEnd"));
    }

    private IEnumerator PlayEndSequence(string dialoguePartName)
    {
        // 确保碰撞体先禁用
        if (finalCollider != null)
        {
            finalCollider.enabled = false;
        }

        // 精灵初始化为全透明
        if (finalSprite != null)
        {
            Color c = finalSprite.color;
            c.a = 0f;
            finalSprite.color = c;
        }

        // 初始化 Ambient 音量
        float initialVolume = Ambient.volume;

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

            yield return null;
        }

        // 10秒后启用碰撞体
        if (finalCollider != null)
        {
            finalCollider.enabled = true;
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
