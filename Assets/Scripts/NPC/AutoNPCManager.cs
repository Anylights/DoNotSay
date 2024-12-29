using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Febucci.UI;

public class AutoNPCManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TypewriterByCharacter typewriter;

    [Header("Dialogue Parts")]
    public List<DialoguePart> dialogueParts;  // 存储所有对话部分
    public string initialPartName;            // 初始对话部分的名称

    public float timeBetweenLines = 3f; // 设置自动切换下一句的时间

    protected DialoguePart currentPart;
    protected int currentLineIndex = 0;
    protected bool isPlayerInRange = false;
    protected bool isDialoguePlaying = false;
    protected float lineTimer = 0f;

    void Start()
    {
        // 设置初始对话部分
        SwitchToDialoguePart(initialPartName);
    }

    // 切换到指定名称的对话部分
    public void SwitchToDialoguePart(string targetPartName)
    {
        DialoguePart targetPart = dialogueParts.Find(part => part.partName == targetPartName);

        if (targetPart == null)
        {
            Debug.LogError($"找不到名为 {targetPartName} 的对话部分。可用对话部分有：");
            foreach (var part in dialogueParts)
            {
                Debug.LogError(part.partName);
            }
            return;
        }

        // 如果有当前对话部分，禁用其所有碰撞体
        if (currentPart != null)
        {
            DisableAllColliders(currentPart);
        }

        currentPart = targetPart;
        currentLineIndex = 0;
        isDialoguePlaying = false;
        DisableAllColliders(currentPart);
    }

    private void Update()
    {
        // 移除原先的 lineTimer 逻辑等
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 如果当前没有在播放对话，则从头开始自动播放
            if (!isDialoguePlaying && currentLineIndex < currentPart.dialogueLines.Count)
            {
                StartCoroutine(AutoNextLineCoroutine());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            ResetDialogue(); // 退出碰撞范围则重置
        }
    }

    private IEnumerator AutoNextLineCoroutine()
    {
        isDialoguePlaying = true;
        // 循环显示当前对话部分中的所有行
        while (currentLineIndex < currentPart.dialogueLines.Count)
        {
            dialogueText.text = "";
            typewriter.ShowText(currentPart.dialogueLines[currentLineIndex].dialogueText);
            EnableColliders(currentLineIndex);
            currentLineIndex++;

            // 等待当前行文本完成显示
            yield return new WaitUntil(() => !typewriter.isShowingText);
            // 再等待 2 秒后自动显示下一行
            yield return new WaitForSeconds(2f);
        }

        // 当前对话全部播放结束后执行
        dialogueText.text = "";
        DisableColliders(currentLineIndex - 1);
        currentLineIndex = 0;
        isDialoguePlaying = false;

        // 调用虚函数，让子类有机会在对话完全结束时执行自己的逻辑
        OnAllLinesDisplayed();

        // 如果玩家依旧在碰撞范围内，则 3 秒后从头开始播放对话
        if (isPlayerInRange)
        {
            yield return new WaitForSeconds(3f);
            if (isPlayerInRange)
            {
                StartCoroutine(AutoNextLineCoroutine());
            }
        }
    }

    // 新增可被子类重写的方法
    protected virtual void OnAllLinesDisplayed()
    {
        // 默认不执行任何操作，子类可 override
    }

    private void ResetDialogue()
    {
        dialogueText.text = "";
        DisableColliders(currentLineIndex - 1);
        currentLineIndex = 0;
        isDialoguePlaying = false;
        lineTimer = 0f;
    }

    private void EnableColliders(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= currentPart.dialogueLines.Count) return;
        if (currentPart.dialogueLines[lineIndex].colliders != null)
        {
            foreach (GameObject obj in currentPart.dialogueLines[lineIndex].colliders)
            {
                var col = obj.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;
            }
        }
    }

    protected void DisableColliders(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= currentPart.dialogueLines.Count) return;
        if (currentPart.dialogueLines[lineIndex].colliders != null)
        {
            foreach (GameObject obj in currentPart.dialogueLines[lineIndex].colliders)
            {
                var col = obj.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
            }
        }
    }

    protected void DisableAllColliders(DialoguePart part)
    {
        for (int i = 0; i < part.dialogueLines.Count; i++)
        {
            DisableColliders(i);
        }
    }

    public void EndCurrentDialogue()
    {
        dialogueText.text = "";
        DisableColliders(currentLineIndex - 1); // 关闭当前对话的碰撞体
        currentLineIndex = 0;
        isDialoguePlaying = false;
    }
}
