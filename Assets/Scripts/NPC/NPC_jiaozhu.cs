using UnityEngine;

public class NPC_jiaozhu : NPCManager
{
    public Collider2D targetCollider; // 添加公共 Collider2D 引用
    protected override void DisplayNextLine()
    {
        base.DisplayNextLine();

        if (currentPart.partName == "Part1" && currentLineIndex >= currentPart.dialogueLines.Count)
        {
            EventCenter.Instance.TriggerEvent("NPC_jiaozhu_Part1done");
            SwitchToDialoguePart("Part2");
            if (targetCollider != null)
            {
                targetCollider.enabled = true; // 启用碰撞体

            }
        }
    }

}
