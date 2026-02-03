using UnityEngine;
using System.Collections;

public class CheckCSV : MonoBehaviour
{
    // 使用 Awake 确保比 InitializatePlayer 先准备好
    void Awake()
    {
        GameEventManager.OnGameStart += UpdateAllStats;
        GameEventManager.OnDataNeedUpdate += UpdateAllStats;
    }
    void Start() // ✅ 这里是安全的，此时所有脚本的 Awake 都跑完了
    {
        // 强制先读取一次数据，确保怪物有攻击力
        UpdateAllStats();
    }


    void OnDestroy()
    {
        GameEventManager.OnGameStart -= UpdateAllStats;
        GameEventManager.OnDataNeedUpdate -= UpdateAllStats;
    }

    // 核心查表逻辑
    public void UpdateAllStats()
    {
        var db = PlayerBaseData.Instance;

        // ==========================================
        // 第一步：查 PlayerData (根据等级查基础属性)
        // ==========================================
        TextAsset pData = Resources.Load<TextAsset>("CSV_Data/PlayerData");
        string[] pLines = pData.text.Split('\n');

        for (int i = 1; i < pLines.Length; i++)
        {
            string[] row = pLines[i].Split(',');
            if (row.Length < 5) continue;

            if (int.Parse(row[0]) == db.level) // 找到当前等级
            {
                db.nextLevelXP = int.Parse(row[1]);
                db.finalATK = float.Parse(row[2]); // 先存入基础攻击
                // 暂时把基础HP当作MaxHP，后面会加上武器加成
                db.finalMaxHP = float.Parse(row[4]);
                break;
            }
        }

        // ==========================================
        // 第二步：查 WeaponData (根据武器ID查加成)
        // ==========================================
        TextAsset wData = Resources.Load<TextAsset>("CSV_Data/WeaponData");
        string[] wLines = wData.text.Split('\n');

        for (int i = 1; i < wLines.Length; i++)
        {
            string[] row = wLines[i].Split(',');
            if (row.Length < 5) continue;

            if (int.Parse(row[0]) == db.currentWeaponID)
            {
                db.weaponATK = float.Parse(row[2]);
                db.weaponHP = float.Parse(row[4]);
                break;
            }
        }

        // ==========================================
        // 第三步：查 EnemyData (查当前等级怪物的模板)
        // ==========================================
        TextAsset eData = Resources.Load<TextAsset>("CSV_Data/EnemyData");
        string[] eLines = eData.text.Split('\n');

        for (int i = 1; i < eLines.Length; i++)
        {
            string[] row = eLines[i].Split(',');
            if (row.Length < 6) continue;

            // 假设怪物的强度跟随玩家等级 (row[0]是PlayerLevel)
            if (int.Parse(row[0]) == db.level)
            {
                // 存入 enemyTemplate 变量中，供 EnemySpawner 生成怪物时使用
                db.enemyTemplateATK = float.Parse(row[1]);
                db.enemyTemplateHP = float.Parse(row[3]);
                db.enemyTemplateXP = int.Parse(row[4]);
                db.enemyTemplateGroupID = int.Parse(row[5]);
                break;
            }
        }

        // ==========================================
        // 第四步：汇总计算与修正
        // ==========================================

        // 加上武器属性
        db.finalATK += db.weaponATK;
        db.finalMaxHP += db.weaponHP;

        // 【HP 初始化补丁】
        // 如果是刚进入游戏(currentHP<=0)，或者升级/换装导致上限增加
        // 我们这里简单处理：直接把血回满（或者你可以写复杂的逻辑）
        if (db.currentHP <= 0 || db.currentHP > db.finalMaxHP)
        {
            db.currentHP = db.finalMaxHP;
        }

        Debug.Log($"<color=green>数据更新完毕: Lv.{db.level}, ATK:{db.finalATK}, 怪物模板HP:{db.enemyTemplateHP}</color>");

        // ==========================================
        // 第五步：通知全世界
        // ==========================================
        GameEventManager.CallStatsChanged();
    }
}