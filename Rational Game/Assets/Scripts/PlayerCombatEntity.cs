using UnityEngine;

public class PlayerCombatEntity : MonoBehaviour
{
    [Header("战斗参数")]
    public float attackInterval = 1.0f; // 攻速：几秒打一下
    public float rayDistance = 2.0f;    // 攻击距离（射线长度）
    public LayerMask enemyLayer;        // 这一层只打敌人

    private float timer;
    private float currentAtk; // 当前攻击力
    private bool isAttacking = false;

    void Start()
    {
        // 游戏一开始，先从数据中心拿一下初始攻击力
        UpdateStats();

        // 监听：如果之后属性变了（升级/换装），自动更新攻击力
        GameEventManager.OnStatsChanged += UpdateStats;
        GameEventManager.OnPlayerStateChanged += HandleStateChange;
    }

    void OnDestroy()
    {
        GameEventManager.OnStatsChanged -= UpdateStats;
        GameEventManager.OnPlayerStateChanged -= HandleStateChange;
    }

    void UpdateStats()
    {
        // 从 PlayerBaseData 获取最新的最终攻击力
        currentAtk = PlayerBaseData.Instance.finalATK;
        Debug.Log($"玩家战斗实体更新：当前攻击力 = {currentAtk}");
    }

    void HandleStateChange(PlayerState state)
    {
        isAttacking = (state == PlayerState.Attacking);
    }

    void Update()
    {
        // 只有在攻击状态下才进行倒计时攻击
        if (isAttacking)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                PerformAttack();
                timer = attackInterval; // 重置计时器
            }
        }
    }

    void PerformAttack()
    {
        // 1. 发射射线
        // origin: 玩家位置, direction: 向右(Vector2.right), distance: 距离, layerMask: 只打敌人层
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, rayDistance, enemyLayer);

        // 为了方便调试，画一条红线（只有在 Scene 窗口能看到，Game 窗口看不到）
        Debug.DrawRay(transform.position, Vector3.right * rayDistance, Color.red, 0.5f);

        // 2. 判定命中
        if (hit.collider != null)
        {
            // 尝试获取怪物身上的脚本
            SingleEnemyData enemy = hit.collider.GetComponent<SingleEnemyData>();
            if (enemy != null)
            {
                // 造成伤害！
                enemy.TakeDamage(currentAtk);
            }
        }
    }
}