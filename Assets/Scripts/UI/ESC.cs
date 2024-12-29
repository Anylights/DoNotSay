using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ESC : MonoBehaviour
{
    public Animator animator;        // 引用动画机
    public GameObject effectPrefab;  // 生成特效使用
    public TextMeshProUGUI textMesh; // 需要清空的文本

    // Start is called before the first frame update
    void Start()
    {
        // 若需要在此处获取 Animator，可使用:
        animator = GetComponent<Animator>();
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            animator.SetBool("IsESCPressed", true);
            // 启用自身的碰撞体
            GetComponent<Collider2D>().enabled = true;
            StopAllCoroutines();  // 防止重复计时
            StartCoroutine(ResetESCPressed());
        }
    }

    private IEnumerator ResetESCPressed()
    {
        yield return new WaitForSeconds(5f);
        animator.SetBool("IsESCPressed", false);
        // 禁用自身的碰撞体
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Word"))
        {
            if (effectPrefab != null)
            {
                var effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 3f);
            }
            if (textMesh != null)
            {
                textMesh.text = "";
            }
            StartCoroutine(ExitGameAfterDelay(3f));
        }
    }

    private IEnumerator ExitGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Exiting game...");
        Application.Quit();

    }
}
