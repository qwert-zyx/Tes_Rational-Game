using UnityEngine;

public class SingleEnemyData : MonoBehaviour
{
    public float currentHP;
    public float maxHP;
    public float atk;
    public int xp;
    public int groupID;

    // 初始化：生成时被调用，把模板数据复制给自己
    public void Init(float hp, float atk, int xp, int groupID)
    {
        this.maxHP = hp;
        this.currentHP = hp;
        this.atk = atk;
        this.xp = xp;
        this.groupID = groupID;
    }

    // 受击逻辑：给玩家攻击用的接口
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"怪物受击: -{damage}, 剩余血量: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 发出死亡广播
        GameEventManager.CallEnemyDead(xp, 0, groupID); // 这里level传0或者当前等级都行
        Destroy(gameObject);
    }
}