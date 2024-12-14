using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 正常移动速度
    public float rollSpeedMultiplier = 2f; // 翻滚时的速度增益倍数
    public float rollDuration = 0.5f; // 翻滚持续时间
    public AnimationCurve rollSpeedCurve; // 控制翻滚速度变化的曲线

    private Vector2 moveDirection;
    private float rollTime = 0f;
    private bool isRolling = false;
    private float currentSpeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleInput();
        MovePlayer();
    }

    private void HandleInput()
    {
        // 获取玩家输入，支持八方向移动
        float horizontal = Input.GetAxisRaw("Horizontal");  // A/D 或 左右箭头键
        float vertical = Input.GetAxisRaw("Vertical");      // W/S 或 上下箭头键

        moveDirection = new Vector2(horizontal, vertical).normalized;

        // 按下空格或A键触发翻滚
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("A"))  // Xbox手柄A键映射为 "A"
        {
            if (!isRolling) // 确保翻滚只能触发一次
            {
                isRolling = true;
                rollTime = 0f; // 重置翻滚时间
            }
        }
    }

    private void MovePlayer()
    {
        if (isRolling)
        {
            // 增加翻滚时间
            rollTime += Time.deltaTime;

            // 计算当前的翻滚速度，根据曲线来调整
            float curveValue = rollSpeedCurve.Evaluate(rollTime / rollDuration);
            currentSpeed = moveSpeed + (moveSpeed * (rollSpeedMultiplier - 1) * curveValue);

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
}
