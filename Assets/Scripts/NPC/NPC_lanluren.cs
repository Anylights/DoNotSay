using UnityEngine;

public class NPC_lanluren : NPCManager
{
    private bool part1Done = false;

    protected override void DisplayNextLine()
    {
        base.DisplayNextLine();

        if (!part1Done && currentLineIndex >= currentPart.dialogueLines.Count)
        {
            part1Done = true;
            EventCenter.Instance.TriggerEvent("NPC_lanluren_Part1done");
            SwitchToDialoguePart("Lanlu_2");
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
