using UnityEngine;

public class PlayerController : MonoBehaviour
{
  
    // 当前状态
    public PlayerState currentState = PlayerState.Moving;

    // 当发生物理碰撞开始时
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果碰到了 "Enemy" 标签的物体
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentState = PlayerState.Attacking;
            // 广播：大家停下来，我要开始打架了！
            GameEventManager.CallPlayerStateChanged(PlayerState.Attacking);
            Debug.Log("碰到敌人 -> 进入战斗状态");
        }
    }

    // 当物理碰撞结束时（敌人死了或者被销毁了）
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 只有当没有接触任何敌人时，才恢复移动
            // 这里为了简化逻辑，直接恢复
            currentState = PlayerState.Moving;
            // 广播：没人了，继续跑！
            GameEventManager.CallPlayerStateChanged(PlayerState.Moving);
            Debug.Log("敌人消失 -> 恢复移动");
        }
    }

    // 监听换装信号（为了以后做动画留的接口）
    void Awake()
    {
        GameEventManager.OnWeaponSwapped += HandleWeaponSwap;
        // 【强制恢复时间】
        // 防止上一局游戏死掉后时间被设为0，导致新游戏里怪物不刷新、动画不播放
        Time.timeScale = 1;

        // 【新增】进场时，强制告诉全世界：我已经准备好，开始移动了！
        // 这样 EnemySpawner 就算初始化晚了，也能收到这个信号
        currentState = PlayerState.Moving;
        GameEventManager.CallPlayerStateChanged(PlayerState.Moving);


    }
    void OnDestroy()
    {
        GameEventManager.OnWeaponSwapped -= HandleWeaponSwap;
    }
    void HandleWeaponSwap(int weaponID)
    {
        Debug.Log($"玩家手里武器变了，ID: {weaponID}，可以在这里切换攻击动画");
    }
}