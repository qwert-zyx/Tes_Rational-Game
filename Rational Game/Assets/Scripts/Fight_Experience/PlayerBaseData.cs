using UnityEngine;
using System; // 必须引用，用于处理时间

public class PlayerBaseData : MonoBehaviour
{
    public static PlayerBaseData Instance; // 单例

    [Header("存档数据")]
    public int level = 1; // 默认1级
    public int currentXP;
    public int currentWeaponID;

    [Header("运行时计算数据")]
    public int nextLevelXP;
    public float finalMaxHP;
    public float finalATK;
    public float finalDEF;
    public float currentHP;
    public float weaponATK;
    public float weaponHP;

    [Header("敌人模板数据")]
    public float enemyTemplateHP;
    public float enemyTemplateATK;
    public int enemyTemplateXP;
    public int enemyTemplateGroupID;

    // ==========================================
    // 【新增 1】 游戏模式变量 (解决 isTowerMode 报错)
    // ==========================================
    [Header("游戏模式状态")]
    public bool isTowerMode = false;  // false=挂机, true=爬塔
    public int targetLevelID = 1;     // 爬塔目标层数

    // ==========================================
    // 【新增 2】 体力系统变量
    // ==========================================
    [Header("体力系统数据")]
    public float currentStamina;
    public float maxStamina = 100f;
    public string lastStaminaUpdateTime; // 上次离线时间
    public float staminaRegenPerHour = 10f; // 每小时回多少

    void Awake()
    {
        // 保证全局唯一且不销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==========================================
    // 体力恢复核心逻辑
    // ==========================================
    public void CheckStaminaRegen()
    {
        // 1. 如果没有记录时间（第一次玩），就记录当前时间并跳过
        if (string.IsNullOrEmpty(lastStaminaUpdateTime))
        {
            lastStaminaUpdateTime = DateTime.Now.ToString();
            return;
        }

        // 2. 计算离线时长
        DateTime lastTime;
        // 尝试解析时间，防止存档损坏报错
        if (!DateTime.TryParse(lastStaminaUpdateTime, out lastTime))
        {
            lastStaminaUpdateTime = DateTime.Now.ToString();
            return;
        }

        DateTime nowTime = DateTime.Now;
        TimeSpan timePassed = nowTime - lastTime;

        // 3. 计算应回多少体力 (TotalHours 支持小数，比如0.5小时)
        float recoverAmount = (float)timePassed.TotalHours * staminaRegenPerHour;

        if (recoverAmount > 0)
        {
            float oldStamina = currentStamina;
            currentStamina = Mathf.Min(maxStamina, currentStamina + recoverAmount);

            // 更新时间戳
            lastStaminaUpdateTime = nowTime.ToString();

            Debug.Log($"<color=green>离线恢复结算: 离线{timePassed.TotalMinutes:F1}分钟, 恢复{recoverAmount:F1}体力 (由{oldStamina} -> {currentStamina})</color>");
        }
    }

    // 每次退出游戏时，自动保存时间
    void OnApplicationQuit()
    {
        lastStaminaUpdateTime = DateTime.Now.ToString();
        // 这里以后加上 SaveSystem.Save();
    }
}