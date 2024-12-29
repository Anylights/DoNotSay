using UnityEngine;

public class NPC_kanshouren : NPCManager
{
    public Collider2D targetCollider; // 添加公共 Collider2D 引用

    void OnEnable()
    {
        EventCenter.Instance.Subscribe("FengziAwakened", OnFengziAwakened);
    }

    void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("FengziAwakened", OnFengziAwakened);
    }

    protected override void DisplayNextLine()
    {
        base.DisplayNextLine();

        if (currentPart.partName == "Part3" && currentLineIndex >= currentPart.dialogueLines.Count)
        {
            if (targetCollider != null)
            {
                targetCollider.isTrigger = true;
            }
            else
            {
                Debug.LogWarning("targetCollider 未分配");
            }
        }
    }

    private void OnFengziAwakened()
    {
        SwitchToDialoguePart("Part3");
    }
}

