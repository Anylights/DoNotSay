using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CursorManager : MonoBehaviour
{

    // 四个分割后的精灵，顺序分别代表鼠标序号 1,2,3,4
    public Sprite[] splittedSprites;
    private Texture2D[] cursorTextures; // 转换后的 Texture2D
    private bool isMouseVisible = false;
    private bool isCharging = false;
    private Coroutine chargingRoutine;

    private static CursorManager instance;
    public static CursorManager Instance => instance;

    // 新增：粒子特效预制体
    public GameObject disappearParticlePrefab;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // 初始时隐藏系统光标
        Cursor.visible = false;

        // 将分割的 Sprite 转换为 Texture2D
        cursorTextures = new Texture2D[splittedSprites.Length];
        for (int i = 0; i < splittedSprites.Length; i++)
        {
            cursorTextures[i] = ConvertSpriteToTexture(splittedSprites[i]);
        }
    }

    private void OnEnable()
    {
        EventCenter.Instance.Subscribe("PlayerStartCharging", OnCharging);
        EventCenter.Instance.Subscribe("PlayerStartFiring", OnFiring);
    }

    private void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("PlayerStartCharging", OnCharging);
        EventCenter.Instance.Unsubscribe("PlayerStartFiring", OnFiring);
    }

    // 将单个 Sprite 转成 Texture2D
    private Texture2D ConvertSpriteToTexture(Sprite sprite)
    {
        var rect = sprite.rect;
        Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        Color[] pixels = sprite.texture.GetPixels(
            (int)rect.x,
            (int)rect.y,
            (int)rect.width,
            (int)rect.height
        );
        newTex.SetPixels(pixels);
        newTex.Apply();
        return newTex;
    }

    // (1) 显示鼠标动画：在0-0.4秒内，每0.1秒切换一次贴图，顺序1->2->3->4，然后保持可见且始终显示第4个贴图
    public void ShowMouse()
    {
        if (isMouseVisible) return;
        StopAllCoroutines();
        StartCoroutine(ShowMouseAnimation());
    }

    private IEnumerator ShowMouseAnimation()
    {
        isMouseVisible = true;
        Cursor.visible = true;
        float interval = 0.05f;
        // 顺序 0->1->2->3 对应 (1,2,3,4)
        for (int i = 0; i < 4; i++)
        {
            Cursor.SetCursor(cursorTextures[i], Vector2.zero, CursorMode.Auto);
            yield return new WaitForSeconds(interval);
        }
        // 最后贴图保持第4个
        Cursor.SetCursor(cursorTextures[3], Vector2.zero, CursorMode.Auto);
    }

    // (2) 消失鼠标动画：在0-0.4秒内，每0.1秒切换一次贴图，顺序4->3->2->1，然后不可见
    public void HideMouse()
    {
        if (!isMouseVisible) return;
        StopAllCoroutines();
        StartCoroutine(HideMouseAnimation());
    }

    private IEnumerator HideMouseAnimation()
    {
        isMouseVisible = false;
        float interval = 0.05f;
        // 顺序 3->2->1->0 对应 (4,3,2,1)
        for (int i = 3; i >= 0; i--)
        {
            Cursor.SetCursor(cursorTextures[i], Vector2.zero, CursorMode.Auto);
            yield return new WaitForSeconds(interval);
        }
        // 最后不可见
        Cursor.visible = false;

        // 新增：在鼠标位置生成粒子特效
        if (disappearParticlePrefab != null)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            GameObject particle = Instantiate(disappearParticlePrefab, mousePosition, Quaternion.identity);
            Destroy(particle, 3f); // 3秒后销毁粒子特效
        }
    }

    // (3) 蓄力时的动画：每0.5秒切换一次贴图，顺序4->2->1（对应下标 3->1->0），若仍在蓄力则保持贴图1
    public void StartCharging()
    {
        if (!isMouseVisible) return;
        if (chargingRoutine != null) StopCoroutine(chargingRoutine);
        chargingRoutine = StartCoroutine(ChargingAnimation());
    }

    private IEnumerator ChargingAnimation()
    {
        isCharging = true;
        float interval = 1f;

        // 第一次切换到贴图 4 (index=3)
        Cursor.SetCursor(cursorTextures[3], Vector2.zero, CursorMode.Auto);
        yield return new WaitForSeconds(interval);
        if (!isCharging) yield break;

        // 切换到贴图 2 (index=1)
        Cursor.SetCursor(cursorTextures[1], Vector2.zero, CursorMode.Auto);
        yield return new WaitForSeconds(interval);
        if (!isCharging) yield break;

        // 切换到贴图 1 (index=0)，并保持
        Cursor.SetCursor(cursorTextures[0], Vector2.zero, CursorMode.Auto);
    }

    // (4) 结束蓄力后发射动画：在0-0.4秒内，每0.1秒切换一次贴图，顺序1->2->3->4，然后保持第4个
    public void EndChargingAndFire()
    {
        if (!isMouseVisible) return;
        isCharging = false;
        if (chargingRoutine != null)
        {
            StopCoroutine(chargingRoutine);
            chargingRoutine = null;
        }
        StopAllCoroutines();
        StartCoroutine(FireAnimation());
    }

    private IEnumerator FireAnimation()
    {
        float interval = 0.1f;
        // 顺序0->1->2->3 (对应贴图1->2->3->4)
        for (int i = 0; i < 4; i++)
        {
            Cursor.SetCursor(cursorTextures[i], Vector2.zero, CursorMode.Auto);
            yield return new WaitForSeconds(interval);
        }
        // 最后保持第4个
        Cursor.SetCursor(cursorTextures[3], Vector2.zero, CursorMode.Auto);
    }

    // 接收“开始蓄力”的事件，播放蓄力动画
    private void OnCharging()
    {
        StartCharging();
    }

    // 接收“开始发射”的事件，播放发射动画
    private void OnFiring()
    {
        EndChargingAndFire();
    }

    // 获取鼠标在世界空间的位置
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane; // 设置一个合适的Z值
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
