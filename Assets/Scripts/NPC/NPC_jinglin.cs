using UnityEngine;

public class NPC_jinglin : NPCManager
{
    void OnEnable()
    {
        EventCenter.Instance.Subscribe("PlayerTransportedToTele0", OnPlayerTransportedToTele0);
    }

    void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("PlayerTransportedToTele0", OnPlayerTransportedToTele0);
    }

    private void OnPlayerTransportedToTele0()
    {
        SwitchToDialoguePart("Part2");
    }

}
