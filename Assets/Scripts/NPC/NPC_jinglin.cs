using UnityEngine;

public class NPC_jinglin : NPCManager
{
    void OnEnable()
    {
        EventCenter.Instance.Subscribe("PlayerTransportedToTele0", OnPlayerTransportedToTele0);

        EventCenter.Instance.Subscribe("Tree_3_Completed", OnTree3Completed);
    }

    void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("PlayerTransportedToTele0", OnPlayerTransportedToTele0);
    }

    private void OnPlayerTransportedToTele0()
    {
        SwitchToDialoguePart("Part2");
    }

    private void OnTree3Completed()
    {
        SwitchToDialoguePart("Part5");
    }

}
