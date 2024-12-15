using System.Collections;
using UnityEngine;
using TMPro;

public class NPCManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public TextMeshProUGUI dialogueText; // TextMeshPro文本框
    public string[] dialogueLines;       // NPC的对话内容
    public float timeBetweenLines = 2f;  // 每段对话的显示时间

    [Header("Raycast Settings")]
    public Transform raycastEndPoint;    // 射线终点

    private bool isPlayerInRange = false;
    private int currentLine = 0;
    private bool isDialoguePlaying = false;

    void Start()
    {
        // 初始化代码
    }

    void Update()
    {
        CheckForPlayer();
        HandleDialogueInput();
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
            isPlayerInRange = true;
            // 确保玩家在物体和射线终点之间
            float playerDistance = (hit.point - (Vector2)transform.position).magnitude;
            if (playerDistance <= distance && !isDialoguePlaying)
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

    private void HandleDialogueInput()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && !isDialoguePlaying)
        {
            if (currentLine < dialogueLines.Length)
            {
                StartCoroutine(DisplayDialogue());
            }
        }
    }

    private IEnumerator DisplayDialogue()
    {
        isDialoguePlaying = true;
        dialogueText.text = dialogueLines[currentLine];
        currentLine++;

        yield return new WaitForSeconds(timeBetweenLines);

        if (currentLine < dialogueLines.Length)
        {
            StartCoroutine(DisplayDialogue());
        }
        else
        {
            dialogueText.text = "";
            currentLine = 0; // 重新开始对话
            isDialoguePlaying = false;
        }
    }
}
