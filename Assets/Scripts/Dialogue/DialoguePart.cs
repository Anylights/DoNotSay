using UnityEngine;
using System.Collections.Generic;

public class DialoguePart : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public string partName;  // 用于识别和切换的唯一名称
    public List<DialogueLine> dialogueLines;
}