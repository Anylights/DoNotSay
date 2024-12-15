using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    //引用的外部组件
    public Animator animator;

    //角色可设置属性
    public float moveSpeed = 5f; // 正常移动速度
    public float rollSpeedMultiplier = 3f; // 翻滚时的速度增益倍数
    public float rollDuration = 0.5f; // 翻滚持续时间

    // 发射预制体相关属性
    public GameObject WordPrehab; // 预制体
    public float WordShootSpeed = 10f; // 发射速度
    public float WordLifeTime = 2f; // 预制体存在时间

    //角色内部变量
    private float rollTime = 0f;
    private bool isRolling = false;
    private float currentSpeed;
    private Vector2 moveDirection;
    private GameObject currentProjectile; // 当前存在的预制体
    private Vector2 projectileDirection; // 发射体的方向

    void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        MovePlayer();
        MoveProjectile();
    }

    private void HandleInput()
    {
        // 获取玩家输入，支持八方向移动
        float horizontal = Input.GetAxisRaw("Horizontal");  // A/D 或 左右箭头键
        float vertical = Input.GetAxisRaw("Vertical");      // W/S 或 上下箭头键

        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);

        moveDirection = new Vector2(horizontal, vertical).normalized;

        // 按下空格或Xbox手柄A键触发翻滚
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))  // Xbox手柄A键映射为 "JoystickButton0"
        {
            if (!isRolling) // 确保翻滚只能触发一次
            {
                isRolling = true;
                rollTime = 0f; // 重置翻滚时间
            }
        }

        // 按下鼠标右键发射预制体
        if (Input.GetMouseButtonDown(1) && currentProjectile == null)
        {
            FireProjectile();
        }
    }

    private void MovePlayer()
    {
        if (isRolling)
        {
            // 增加翻滚时间
            rollTime += Time.deltaTime;

            // 计算当前的翻滚速度，简单的非线性移动
            currentSpeed = moveSpeed * rollSpeedMultiplier;

            if (rollTime >= rollDuration)
            {
                isRolling = false;  // 翻滚结束
                currentSpeed = moveSpeed;  // 恢复正常速度
            }
        }
        else
        {
            // 恢复正常速度
            currentSpeed = moveSpeed;
        }

        // 移动玩家
        transform.Translate(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void FireProjectile()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        projectileDirection = (mousePosition - transform.position).normalized;

        currentProjectile = Instantiate(WordPrehab, transform.position, Quaternion.identity);
        StartCoroutine(DestroyProjectileAfterTime(currentProjectile, WordLifeTime));
    }

    private void MoveProjectile()
    {
        if (currentProjectile != null)
        {
            currentProjectile.transform.Translate(projectileDirection * WordShootSpeed * Time.deltaTime);
        }
    }

    private IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(projectile);
        currentProjectile = null;
    }
}