using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Febucci.UI;

public class NPCManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public TextMeshProUGUI dialogueText; // TextMeshPro文本框
    public List<DialogueLine> dialogueLines; // 每段对话及其绑定的碰撞体
    public TypewriterByCharacter typewriter; // TypewriterEffect 插件

    public float timeBetweenLines = 2f;  // 每段对话的显示时间

    [Header("Raycast Settings")]
    public Transform raycastEndPoint;    // 射线终点

    private bool isPlayerInRange = false;
    private int currentLine = 0;
    private bool isDialoguePlaying = false;
    private bool prevIsPlayerInRange = false;
    private float dialogueTimer = 0f;
    private float dialogueTimeout = 5f;

    void Start()
    {
        // 初始化代码
    }

    void Update()
    {
        CheckForPlayer();
        HandleDialogueInput();
        CheckDialogueTimeout();
    }

    private void CheckForPlayer()
    {
        // 计算当前物体到射线终点的方向和距离
        Vector2 direction = raycastEndPoint.position - transform.position;
        float distance = direction.magnitude;

        // 发射射线进行检测
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, LayerMask.GetMask("Player"));

        // 检查射线是否与玩家发生碰撞
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            float playerDistance = Mathf.Abs(hit.point.x - transform.position.x);
            if (playerDistance <= 1f)
            {
                isPlayerInRange = true;
                if (!prevIsPlayerInRange && !isDialoguePlaying)
                {
                    dialogueText.text = "按F键进行对话";
                }
            }
            else
            {
                isPlayerInRange = false;
                if (!isDialoguePlaying)
                {
                    dialogueText.text = "";
                }
            }
        }
        else
        {
            isPlayerInRange = false;
            if (!isDialoguePlaying)
            {
                dialogueText.text = "";
            }
        }
        prevIsPlayerInRange = isPlayerInRange;
    }

    private void HandleDialogueInput()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && !isDialoguePlaying)
        {
            if (currentLine < dialogueLines.Count)
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
        typewriter.ShowText(dialogueLines[currentLine].dialogueText);
        EnableColliders(currentLine);
        currentLine++;

        yield return null; // 等待玩家按F显示下一句
    }

    private void DisplayNextLine()
    {
        DisableColliders(currentLine - 1);

        if (currentLine < dialogueLines.Count)
        {
            dialogueTimer = 0f; // 重置对话计时器
            typewriter.ShowText(dialogueLines[currentLine].dialogueText);
            EnableColliders(currentLine);
            currentLine++;
        }
        else
        {
            dialogueText.text = "";
            currentLine = 0; // 重新开始对话
            isDialoguePlaying = false;
        }
    }

    private void CheckDialogueTimeout()
    {
        if (isDialoguePlaying)
        {
            dialogueTimer += Time.deltaTime;
            if (dialogueTimer >= dialogueTimeout)
            {
                dialogueText.text = "";
                currentLine = 0; // 重新开始对话
                isDialoguePlaying = false;
            }
        }
    }

    private void EnableColliders(int lineIndex)
    {
        if (dialogueLines[lineIndex].colliders != null)
        {
            foreach (GameObject colliderObject in dialogueLines[lineIndex].colliders)
            {
                Collider2D collider = colliderObject.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = true;
                }
            }
        }
    }

    private void DisableColliders(int lineIndex)
    {
        if (dialogueLines[lineIndex].colliders != null)
        {
            foreach (GameObject colliderObject in dialogueLines[lineIndex].colliders)
            {
                Collider2D collider = colliderObject.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }
    }
}
