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
        if (isDialoguePlaying)
        {
            lineTimer += Time.deltaTime;
            if (lineTimer >= timeBetweenLines)
            {
                DisplayNextLine();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!isDialoguePlaying && currentLineIndex < currentPart.dialogueLines.Count)
            {
                StartCoroutine(DisplayDialogue());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            ResetDialogue();
        }
    }

    private IEnumerator DisplayDialogue()
    {
        isDialoguePlaying = true;
        lineTimer = 0f;
        typewriter.ShowText(currentPart.dialogueLines[currentLineIndex].dialogueText);
        EnableColliders(currentLineIndex);
        currentLineIndex++;
        yield return null;
    }

    protected virtual void DisplayNextLine()
    {
        // 如果已经到达或超过最后一行，则结束对话
        if (currentLineIndex >= currentPart.dialogueLines.Count)
        {
            dialogueText.text = "";
            DisableColliders(currentLineIndex - 1); // 关闭最后一句对话的碰撞体
            currentLineIndex = 0;
            isDialoguePlaying = false;
            return;
        }

        DisableColliders(currentLineIndex - 1); // 关闭上一句对话的碰撞体
        lineTimer = 0f;
        typewriter.ShowText(currentPart.dialogueLines[currentLineIndex].dialogueText);
        EnableColliders(currentLineIndex);
        currentLineIndex++;
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
}
