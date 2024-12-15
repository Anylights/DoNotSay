using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word_Andy : MonoBehaviour
{
    public GameObject tileMap; // 需要隐藏的tileMap
    public DialogueLine ChangeDialogueLine; // 对话行
    public string correctAnswer; // 正确答案

    void Start()
    {

        if (string.IsNullOrEmpty(correctAnswer))
        {
            Debug.LogError("correctAnswer 未设置，请在编辑器中设置正确答案。");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Word"))
        {
            if (PlayerManager.Instance.wordInHand == correctAnswer)
            {
                tileMap.SetActive(false);
                other.gameObject.SetActive(false);
                WordPackageManager.Instance.RemoveWord(correctAnswer);
                ChangeDialogueLine.dialogueText = "你叫什么名字来着？Evan？";
            }
        }
    }
}
