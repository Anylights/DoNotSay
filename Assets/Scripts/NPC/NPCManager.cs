using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Febucci.UI;

public class NPCManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    public TextMeshProUGUI dialogueText;
    public TypewriterByCharacter typewriter;

    [Header("Dialogue Parts")]
    public List<DialoguePart> dialogueParts;  // 存储所有对话部分
    public string initialPartName;            // 初始对话部分的名称

    [Header("Dialogue Settings")]
    public float dialogueTimeout = 10f;

    protected DialoguePart currentPart;
    protected int currentLineIndex = 0;
    protected bool isDialoguePlaying = false; // 修改为 protected
    protected bool isPlayerInRange = false; // 修改为 protected
    protected float dialogueTimer = 0f; // 修改为 protected

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
            Debug.LogError($"找不到名为 {targetPartName} 的对话部分");
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

    void Update()
    {
        HandleDialogueInput();
        CheckDialogueTimeout();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (!isDialoguePlaying)
            {
                dialogueText.text = "";
            }
            else
            {
                // 玩家离开范围时，结束对话并关闭碰撞体
                dialogueText.text = "";
                DisableColliders(currentLineIndex - 1);
                currentLineIndex = 0;
                isDialoguePlaying = false;
            }
        }
    }

    private void HandleDialogueInput()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && !isDialoguePlaying)
        {
            if (currentLineIndex < currentPart.dialogueLines.Count)
            {
                StartCoroutine(DisplayDialogue());
            }
        }
        else if (isDialoguePlaying && Input.GetKeyDown(KeyCode.F) && isPlayerInRange)
        {
            DisplayNextLine();
        }
    }

    private IEnumerator DisplayDialogue()
    {
        isDialoguePlaying = true;
        dialogueTimer = 0f; // 重置对话计时器
        typewriter.ShowText(currentPart.dialogueLines[currentLineIndex].dialogueText);
        EnableColliders(currentLineIndex);
        currentLineIndex++;

        yield return null; // 等待玩家按F显示下一句
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
        dialogueTimer = 0f;
        typewriter.ShowText(currentPart.dialogueLines[currentLineIndex].dialogueText);
        EnableColliders(currentLineIndex);
        currentLineIndex++;
    }

    private void CheckDialogueTimeout()
    {
        if (isDialoguePlaying)
        {
            dialogueTimer += Time.deltaTime;
            if (dialogueTimer >= dialogueTimeout)
            {
                dialogueText.text = "";
                DisableColliders(currentLineIndex - 1); // 关闭当前对话的碰撞体
                currentLineIndex = 0;
                isDialoguePlaying = false;
            }
        }
    }

    private void EnableColliders(int lineIndex)
    {
        if (currentPart.dialogueLines[lineIndex].colliders != null)
        {
            foreach (GameObject colliderObject in currentPart.dialogueLines[lineIndex].colliders)
            {
                Collider2D collider = colliderObject.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = true;
                }
            }
        }
    }

    protected void DisableColliders(int lineIndex) // 修改为 protected
    {
        if (currentPart.dialogueLines[lineIndex].colliders != null)
        {
            foreach (GameObject colliderObject in currentPart.dialogueLines[lineIndex].colliders)
            {
                Collider2D collider = colliderObject.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }
    }

    private void DisableAllColliders(DialoguePart part)
    {
        for (int i = 0; i < part.dialogueLines.Count; i++)
        {
            DisableColliders(i);
        }
    }
}
