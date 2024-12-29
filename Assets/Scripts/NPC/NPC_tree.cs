using UnityEngine;

public class NPC_tree : AutoNPCManager
{
    private void OnEnable()
    {
        EventCenter.Instance.Subscribe("NPC_lanluren_Part1done", OnLanlurenPart1Done);
        EventCenter.Instance.Subscribe("NPC_jiaozhu_Part1done", OnJiaozhuPart1Done);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.Unsubscribe("NPC_lanluren_Part1done", OnLanlurenPart1Done);
        EventCenter.Instance.Unsubscribe("NPC_jiaozhu_Part1done", OnJiaozhuPart1Done);
    }

    private void OnLanlurenPart1Done()
    {
        SwitchToDialoguePart("Tree_2");
    }

    private void OnJiaozhuPart1Done()
    {
        SwitchToDialoguePart("Tree_4");
    }

    protected override void OnAllLinesDisplayed()
    {
        base.OnAllLinesDisplayed();

        // 对话全部播放完毕时，针对不同的 partName 触发自定义事件
        if (currentPart != null)
        {
            if (currentPart.partName == "Tree_2")
            {
                EventCenter.Instance.TriggerEvent("Tree_2_Completed");
            }
            else if (currentPart.partName == "Tree_3")
            {
                EventCenter.Instance.TriggerEvent("Tree_2_Completed");
                EventCenter.Instance.TriggerEvent("Tree_3_Completed");
            }
        }
    }
}
