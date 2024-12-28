using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_zhiwen : NPCManager
{
    private bool part1Done = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void DisplayNextLine()
    {
        base.DisplayNextLine();

        if (!part1Done && currentLineIndex >= currentPart.dialogueLines.Count)
        {
            part1Done = true;
            SwitchToDialoguePart("Zhiwen_2");
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
