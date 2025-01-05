using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // 添加这个命名空间引用
                   // 或使用 UnityEngine.Rendering.Universal

public class PlayerMovement : MonoBehaviour
{
    //引用的外部组件
    public Animator animator;

    public RectTransform ESCrectTransform; // 对话框
    public RectTransform ENDrectTransform; // 对话框


    //角色可设置属性
    public float moveSpeed = 5f; // 正常移动速度
    // public float runMultiplier = 1.5f; // 新增跑步速度倍率

    [Header("Projectile Settings")]
    public bool canFireProjectile = false; // 添加标志位
    public GameObject WordPrehab;
    public float minInitialSpeed = 10f;    // 最小初始速度
    public float maxInitialSpeed = 50f;    // 最大初始速度
    public float maxChargeTime = 3f;       // 最大蓄力时间
    public float decayRate = 10f;          // 固定衰减率
    public float WordLifeTime = 0.5f;      // 固定存在时间
    public float rotationFactor = 50f;     // 旋转系数

    [Header("Screen Shake Settings")]
    public float minShakeAmount = 0.02f;   // 最小晃动幅度
    public float maxShakeAmount = 0.2f;    // 最大晃动幅度

    [Header("Light 2D Settings")]
    public UnityEngine.Rendering.Universal.Light2D mainLight2D;         // 替换为 Light2D
    public UnityEngine.Rendering.Universal.Light2D otherPointLight2D;   // 替换为 Light2D

    //角色内部变量
    private float currentSpeed;
    private Vector2 moveDirection;
    private GameObject currentProjectile; // 当前存在的预制体
    private Vector2 projectileDirection; // 发射体的方向
    private float projectileStartTime; // 发射体开始时间
    private float chargeStartTime;     // 开始蓄力的时间
    private bool isCharging;           // 是否正在蓄力
    private Camera mainCamera;         // 主相机引用
    private float initialSpeed;        // 添加这个变量来存储发射速度
    private CinemachineImpulseSource impulseSource;
    private float rotationSpeed;       // 用于记录当前发射体的旋转速度


    void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = moveSpeed;
        mainCamera = Camera.main;
        impulseSource = GetComponent<CinemachineImpulseSource>(); // 获取 ImpulseSource
    }

    void OnEnable()
    {
        EventCenter.Instance.Subscribe("Tree_2_Completed", EnableFireProjectile);
    }

    void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("Tree_2_Completed", EnableFireProjectile);
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        MoveProjectile();
        if (isCharging)
        {
            HandleChargeShake();
        }
        // bool isRunning = Input.GetKey(KeyCode.LeftShift) && moveDirection.x != 0;
        // animator.SetBool("isRunning", isRunning);
    }

    private void HandleInput()
    {
        // 获取玩家输入，只允许左右移动
        float horizontal = Input.GetAxisRaw("Horizontal");  // A/D 或 左右箭头键

        animator.SetBool("isMoving", horizontal != 0);

        moveDirection = new Vector2(horizontal, 0).normalized;

        // 角色在向左走时旋转180度
        if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            ESCrectTransform.localScale = new Vector3(-1, 1, 1);
            ENDrectTransform.localScale = new Vector3(-1, 1, 1);
        }
        else if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            ESCrectTransform.localScale = new Vector3(1, 1, 1);
            ENDrectTransform.localScale = new Vector3(1, 1, 1);
        }

        //rectTransform.localScale = new Vector3(1.0775f, 1.0775f, 1.0775f);

        // 处理蓄力发射
        if (canFireProjectile && Input.GetMouseButtonDown(0) && currentProjectile == null)
        {
            StartCharging();
        }
        else if (canFireProjectile && Input.GetMouseButtonUp(0) && isCharging)
        {
            FireProjectile();
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeStartTime = Time.time;
        EventCenter.Instance.TriggerEvent("PlayerStartCharging");
    }

    private void HandleChargeShake()
    {
        float chargeTime = Mathf.Min(Time.time - chargeStartTime, maxChargeTime);
        if (chargeTime <= 0) return;
        float chargeProgress = chargeTime / maxChargeTime;
        float currentShakeAmount = Mathf.Lerp(minShakeAmount, maxShakeAmount, chargeProgress);

        // 如果使用 Light2D，则修改 intensity
        if (mainLight2D != null)
        {
            mainLight2D.intensity = Mathf.Lerp(1f, 0.2f, chargeProgress);
        }
        if (otherPointLight2D != null)
        {
            otherPointLight2D.intensity = Mathf.Lerp(0f, 20f, chargeProgress);
        }

        if (currentShakeAmount > 0)
        {
            impulseSource.GenerateImpulse(currentShakeAmount);
        }
    }

    private void MovePlayer()
    {
        currentSpeed = moveSpeed;

        // 处理跑步速度
        // if (Input.GetKey(KeyCode.LeftShift))
        // {
        //     currentSpeed = moveSpeed * runMultiplier;
        // }
        // else
        // {
        //     currentSpeed = moveSpeed;
        // }

        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);
    }

    private void FireProjectile()
    {
        AudioManager.Instance.Play("Fire");
        isCharging = false;
        // 恢复 Light2D 的初始值
        if (mainLight2D != null) mainLight2D.intensity = 1f;
        if (otherPointLight2D != null) otherPointLight2D.intensity = 0f;

        mainCamera.transform.position = new Vector3(
            mainCamera.transform.position.x,
            mainCamera.transform.position.y,
            mainCamera.transform.position.z
        );

        float chargeTime = Mathf.Min(Time.time - chargeStartTime, maxChargeTime);
        float chargeProgress = chargeTime / maxChargeTime;
        float initialSpeed = Mathf.Lerp(minInitialSpeed, maxInitialSpeed, chargeProgress);

        Vector3 mousePosition = new Vector3(
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
            0
        );
        projectileDirection = (mousePosition - transform.position).normalized;

        currentProjectile = Instantiate(WordPrehab, transform.position, Quaternion.identity);
        currentProjectile.tag = "Word";
        currentProjectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, projectileDirection);
        projectileStartTime = Time.time;
        this.initialSpeed = initialSpeed;  // 设置实际发射速度
        rotationSpeed = initialSpeed * rotationFactor; // 根据发射速度计算旋转速度
        StartCoroutine(DestroyProjectileAfterTime(currentProjectile, WordLifeTime));
        EventCenter.Instance.TriggerEvent("PlayerStartFiring");
    }

    private void MoveProjectile()
    {
        if (currentProjectile != null)
        {
            float elapsedTime = Time.time - projectileStartTime;
            float currentSpeed = initialSpeed * Mathf.Exp(-decayRate * elapsedTime);
            currentProjectile.transform.Translate(projectileDirection * currentSpeed * Time.deltaTime, Space.World);

            // 根据当前速度计算旋转速度
            float currentRotationSpeed = currentSpeed * rotationFactor;
            currentProjectile.transform.Rotate(0f, 0f, currentRotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(projectile);
        currentProjectile = null;
    }

    public void EnableFireProjectile()
    {
        canFireProjectile = true; // 启用发射预制体的功能
        CursorManager.Instance.ShowMouse();
    }
}