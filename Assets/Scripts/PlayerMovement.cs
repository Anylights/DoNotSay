using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //引用的外部组件
    public Animator animator;

    //角色可设置属性
    public float moveSpeed = 5f; // 正常移动速度

    // 发射预制体相关属性
    public GameObject WordPrehab; // 预制体
    public float initialSpeed = 20f; // 初始速度
    public float decayRate = 2f; // 速度衰减率
    public float WordLifeTime = 2f; // 预制体存在时间

    //角色内部变量
    private float currentSpeed;
    private Vector2 moveDirection;
    private GameObject currentProjectile; // 当前存在的预制体
    private Vector2 projectileDirection; // 发射体的方向
    private float projectileStartTime; // 发射体开始时间

    void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        MoveProjectile();
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
        }
        else if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // 按下鼠标右键发射预制体
        if (Input.GetMouseButtonDown(1) && currentProjectile == null)
        {
            FireProjectile();
        }
    }

    private void MovePlayer()
    {
        currentSpeed = moveSpeed;
        // 修复角色不论按A还是按D都会向右走的问题
        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);
    }

    private void FireProjectile()
    {
        Vector3 mousePosition = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
        projectileDirection = (mousePosition - transform.position).normalized;

        currentProjectile = Instantiate(WordPrehab, transform.position, Quaternion.identity);
        currentProjectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, projectileDirection); // 设置预制件的旋转方向
        projectileStartTime = Time.time; // 记录发射时间
        StartCoroutine(DestroyProjectileAfterTime(currentProjectile, WordLifeTime));
    }

    private void MoveProjectile()
    {
        if (currentProjectile != null)
        {
            float elapsedTime = Time.time - projectileStartTime;
            float speed = initialSpeed * Mathf.Exp(-decayRate * elapsedTime);
            currentProjectile.transform.Translate(projectileDirection * speed * Time.deltaTime, Space.World);
        }
    }

    private IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(projectile);
        currentProjectile = null;
    }
}