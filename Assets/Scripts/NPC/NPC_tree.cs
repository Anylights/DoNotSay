using UnityEngine;

public class NPC_tree : AutoNPCManager
{
    private void OnEnable()
    {
        EventCenter.Instance.Subscribe("NPC_lanluren_Part1done", OnLanlurenPart1Done);
    }

    private void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("NPC_lanluren_Part1done", OnLanlurenPart1Done);
    }

    private void OnLanlurenPart1Done()
    {
        SwitchToDialoguePart("Tree_2");
    }
}
