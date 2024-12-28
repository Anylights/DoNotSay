using UnityEngine;
using System.Collections.Generic;

public class DialoguePart : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public string partName;  // 用于识别和切换的唯一名称
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    void Awake()
    {
        // 自动获取子物体并按 Inspector 面板中的顺序排列
        dialogueLines.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            DialogueLine dialogueLine = child.GetComponent<DialogueLine>();
            if (dialogueLine != null)
            {
                dialogueLines.Add(dialogueLine);
            }
        }
    }
}