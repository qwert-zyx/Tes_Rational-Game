using UnityEngine;

public class PlayerBaseData : MonoBehaviour
{
    public static PlayerBaseData Instance; // 单例

    [Header("存档数据")]
    public int level;
    public int currentXP;
    public int currentWeaponID;

    [Header("运行时计算数据")]
    public int nextLevelXP;
    public float finalMaxHP;
    public float finalATK;
    public float finalDEF;

    // 【之前报错就是因为缺了这一行！】
    // 玩家当前的实时血量
    public float currentHP;

    // 临时存武器属性
    public float weaponATK;
    public float weaponHP;

    [Header("敌人模板数据 (从EnemyData表查出来的)")]
    public float enemyTemplateHP;
    public float enemyTemplateATK;
    public int enemyTemplateXP;
    public int enemyTemplateGroupID;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // 切换场景不销毁
    }
}