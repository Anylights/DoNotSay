using System.Collections;
using UnityEngine;
using TMPro;

public class NPC_zhiwen : NPCManager
{
    [Header("TextMeshPro Settings")]
    public TextMeshProUGUI textMeshPro;         // 用于移动位置的文本组件
    public RectTransform targetRectTransform;   // 移动目标位置

    // ...existing code...

    protected override void HandleDialogueInput()
    {
        // 按下F键且玩家在范围内
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // 只有当typewriter不在播放时才响应F键
            if (!typewriter.isShowingText)
            {
                if (isDialoguePlaying)
                {
                    DisplayNextLine();
                }
                else
                {
                    if (currentLineIndex < currentPart.dialogueLines.Count)
                    {
                        StartCoroutine(DisplayDialogue());
                    }
                }
            }
            // 如果typewriter正在播放，按下F键不做任何操作
        }
    }

    protected override void DisplayNextLine()
    {
        base.DisplayNextLine();

        // 如果对话是 Zhiwen_2，且当前行索引已到最后一行，则先等待再移动 TextMeshPro
        if (currentPart != null &&
            currentPart.partName == "Zhiwen_2" &&
            currentLineIndex == currentPart.dialogueLines.Count)
        {
            StartCoroutine(MoveTextAfterDelay(1.5f));
        }

        // 如果已经播放完 Zhiwen_2，则切换到 Zhiwen_3
        if (currentPart != null &&
            currentPart.partName == "Zhiwen_2" &&
            currentLineIndex >= currentPart.dialogueLines.Count)
        {
            SwitchToDialoguePart("Zhiwen_3");
        }
    }
    private IEnumerator MoveTextAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        Vector3 startPosition = textMeshPro.transform.position;
        Vector3 endPosition = targetRectTransform.position;
        float duration = 1.0f; // 平滑移动的持续时间
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            textMeshPro.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMeshPro.transform.position = endPosition; // 确保最终位置正确
    }
}
