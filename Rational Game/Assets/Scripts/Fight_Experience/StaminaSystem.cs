using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StaminaSystem : MonoBehaviour
{
    [Header("设置")]
    public float burnRatePerSecond = 1.0f; // 消耗速度：每秒扣多少体力？
                                           // 假设每小时恢复10点，如果你想让玩家挂机1小时，那这里消耗也要配合好
                                           // 比如：如果填 1/60f，就是每分钟扣1点。

    [Header("UI 绑定")]
    public GameObject outOfStaminaPanel; // 拖入刚才做的耗尽界面
    public Button backButton;            // 拖入返回按钮
    public string mainMenuSceneName = "MainMenu";

    private bool isRunning = true;

    void Start()
    {
        // 只有挂机模式才启用持续扣体力逻辑
        // 如果是爬塔模式（通常是进门票制），就不需要在 Update 里扣了
        if (PlayerBaseData.Instance.isTowerMode)
        {
            this.enabled = false; // 直接禁用这个脚本的Update
            return;
        }

        // 绑定返回按钮事件
        backButton.onClick.AddListener(ReturnToMenu);

        if (outOfStaminaPanel != null)
            outOfStaminaPanel.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;

        var db = PlayerBaseData.Instance;

        // 1. 扣除体力
        if (db.currentStamina > -5)
        {
            db.currentStamina -= burnRatePerSecond * Time.deltaTime;
        }
        else
        {
            // 2. 体力归零，触发暂停
            HandleStaminaDepleted();
        }
    }

    void HandleStaminaDepleted()
    {
        isRunning = false;

        // 修正数据防止变成负数
        PlayerBaseData.Instance.currentStamina = 0;

        // 游戏暂停
        Time.timeScale = 0;

        Debug.Log("体力耗尽，强制暂停！");

        // 弹出界面
        if (outOfStaminaPanel != null)
        {
            outOfStaminaPanel.SetActive(true);
        }
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1; // 恢复时间

        // 销毁数据包防止重复（根据之前的逻辑）
        if (PlayerBaseData.Instance != null)
        {
            Destroy(PlayerBaseData.Instance.gameObject);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}