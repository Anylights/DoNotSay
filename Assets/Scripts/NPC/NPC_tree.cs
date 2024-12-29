using UnityEngine;

public class NPC_tree : AutoNPCManager
{
    private void OnEnable()
    {
        EventCenter.Instance.Subscribe("NPC_lanluren_Part1done", OnLanlurenPart1Done);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.Unsubscribe("NPC_lanluren_Part1done", OnLanlurenPart1Done);
    }

    private void OnLanlurenPart1Done()
    {
        SwitchToDialoguePart("Tree_2");
    }

    protected override void DisplayNextLine()
    {
        base.DisplayNextLine();

        if (currentPart.partName == "Tree_2" && currentLineIndex >= currentPart.dialogueLines.Count)
        {
            EventCenter.Instance.TriggerEvent("Tree_2_Completed");
        }

        else if (currentPart.partName == "Tree_3" && currentLineIndex >= currentPart.dialogueLines.Count)
        {
            EventCenter.Instance.TriggerEvent("Tree_2_Completed");
            EventCenter.Instance.TriggerEvent("Tree_3_Completed");
        }
    }
}
