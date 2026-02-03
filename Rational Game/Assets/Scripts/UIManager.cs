using UnityEngine;
using UnityEngine.UI; // 必须引用，用于控制 Slider
using TMPro;          // 必须引用，用于控制 TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("UI 组件绑定")]
    public Slider hpSlider;
    public Slider xpSlider;
    public TMP_Text infoText; // 如果你用的是旧版 Text，这里就写 Text

    void Awake()
    {
        // 监听两个信号：
        // 1. 属性更新了 (比如攻击力变了，血上限变了)
        GameEventManager.OnStatsChanged += RefreshUI;
        // 2. 数据变动了 (比如经验涨了，或者怪打我扣血了)
        GameEventManager.OnDataNeedUpdate += RefreshUI;
    }

    void OnDestroy()
    {
        GameEventManager.OnStatsChanged -= RefreshUI;
        GameEventManager.OnDataNeedUpdate -= RefreshUI;
    }

    // 刷新界面的核心逻辑
    void RefreshUI()
    {
        var db = PlayerBaseData.Instance;

        // 1. 更新等级和攻击力文本
        // f0 表示不保留小数
        infoText.text = $"Lv.{db.level}   ATK: {db.finalATK}   Weapon: {db.currentWeaponID}";

        // 2. 更新血条 (当前血量 / 最大血量)
        // 注意：如果要防止除以0报错，可以加个判断
        if (db.finalMaxHP > 0)
        {
            hpSlider.value = db.currentHP / db.finalMaxHP;
        }

        // 3. 更新经验条 (当前经验 / 升级所需经验)
        if (db.nextLevelXP > 0)
        {
            xpSlider.value = (float)db.currentXP / db.nextLevelXP;
        }
    }
}