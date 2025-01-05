using UnityEngine;

public class NPC_jiangshu : NPCManager
{
    public Collider2D targetCollider; // 添加公共 Collider2D 引用
    protected override void DisplayNextLine()
    {
        base.DisplayNextLine();

        if (currentPart.partName == "Part1" && currentLineIndex >= currentPart.dialogueLines.Count)
        {
            if (targetCollider != null)
            {
                targetCollider.enabled = false;
                SwitchToDialoguePart("Part2");
            }
        }
    }

}
