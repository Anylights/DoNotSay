using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 originalScale;
    public Color originalTextColor;
    public float scaleDuration = 0.1f;
    public float colorDuration = 0.1f;
    private Coroutine currentScaleCoroutine;
    private Coroutine currentColorCoroutine;
    private TextMeshProUGUI textMeshPro;

    void OnEnable()
    {
        // 确保 textMeshPro 已正确引用
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI 组件未找到，请确保它是按钮的子对象。");
            return;
        }

        originalScale = transform.localScale;
        originalTextColor = textMeshPro.color;
    }

    // 实现 IPointerEnterHandler 接口
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标进入时开始缩放和改变颜色
        if (currentScaleCoroutine != null) StopCoroutine(currentScaleCoroutine);
        if (currentColorCoroutine != null) StopCoroutine(currentColorCoroutine);

        currentScaleCoroutine = StartCoroutine(ScaleButton(transform.localScale, originalScale * 1.1f, scaleDuration));
        currentColorCoroutine = StartCoroutine(ChangeColor(textMeshPro.color, Color.yellow, colorDuration));
    }

    // 实现 IPointerExitHandler 接口
    public void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标离开时开始恢复缩放和颜色
        if (currentScaleCoroutine != null) StopCoroutine(currentScaleCoroutine);
        if (currentColorCoroutine != null) StopCoroutine(currentColorCoroutine);

        currentScaleCoroutine = StartCoroutine(ScaleButton(transform.localScale, originalScale, scaleDuration));
        currentColorCoroutine = StartCoroutine(ChangeColor(textMeshPro.color, originalTextColor, colorDuration));
    }

    public void OnButtonClick()
    {
        // 按钮点击时的动画效果
        if (currentScaleCoroutine != null) StopCoroutine(currentScaleCoroutine);
        HandleButtonFunction();

        StartCoroutine(ScaleButton(transform.localScale, originalScale * 0.95f, 0.1f)); // 按下时稍微缩小
        StartCoroutine(ScaleButton(originalScale * 0.95f, originalScale, 0.1f)); // 松开时恢复原大小
    }

    private IEnumerator ScaleButton(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale; // 确保最后设置为目标缩放值
    }

    private IEnumerator ChangeColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            textMeshPro.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMeshPro.color = endColor; // 确保最后设置为目标颜色
    }

    private void HandleButtonFunction()
    {
        EventCenter.Instance.TriggerEvent("PickUpWord", textMeshPro.text);
    }
}