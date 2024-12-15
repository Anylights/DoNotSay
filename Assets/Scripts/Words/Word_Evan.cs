using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word_Evan : MonoBehaviour
{
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
                other.gameObject.SetActive(false);
                WordPackageManager.Instance.AddWord("Evan");
            }
        }
    }
}
