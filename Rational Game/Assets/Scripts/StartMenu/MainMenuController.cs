using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 引用 UI
using TMPro;
using System.Collections; // 【必须引用】用到 IEnumerator

public class MainMenuController : MonoBehaviour
{
    [Header("UI 面板")]
    public GameObject panelRoot;
    public GameObject panelDungeon;
    public GameObject panelJail;

    // 【新增】体力不足提示面板
    public GameObject panelNoStamina;

    [Header("UI 组件")]
    public TMP_Text staminaText;

    // ... (原来的按钮变量保持不变，这里为了省篇幅省略了) ...
    public Button btnToDungeon;
    public Button btnToJail;
    public Button btnStartGrind;
    public Button btnBackFromDungeon;
    public Button btnStartTower;
    public Button btnBackFromJail;

    [Header("场景设置")]
    public string grindSceneName = "GrindScene";
    public string towerSceneName = "TowerScene";

    [Header("参数设置")]
    public float enterTowerCost = 10f;
    public float minGrindStamina = 1f;

    void Start()
    {
        // 1. 数据初始化
        if (PlayerBaseData.Instance == null)
        {
            GameObject go = new GameObject("GlobalData");
            go.AddComponent<PlayerBaseData>();
            PlayerBaseData.Instance.currentStamina = 50;
        }
        PlayerBaseData.Instance.CheckStaminaRegen();
        UpdateStaminaUI();

        // 2. 绑定按钮 (保持原来的逻辑)
        btnToDungeon.onClick.AddListener(() => SwitchPanel(panelDungeon));
        btnToJail.onClick.AddListener(() => SwitchPanel(panelJail));
        btnBackFromDungeon.onClick.AddListener(() => SwitchPanel(panelRoot));
        btnBackFromJail.onClick.AddListener(() => SwitchPanel(panelRoot));
        btnStartGrind.onClick.AddListener(EnterGrindGame);
        btnStartTower.onClick.AddListener(EnterTowerGame);

        // 3. 初始显示设置
        SwitchPanel(panelRoot);

        // 【新增】确保提示框一开始是隐藏的
        if (panelNoStamina != null) panelNoStamina.SetActive(false);
    }

    void Update()
    {
        UpdateStaminaUI();
    }

    void SwitchPanel(GameObject targetPanel)
    {
        panelRoot.SetActive(targetPanel == panelRoot);
        panelDungeon.SetActive(targetPanel == panelDungeon);
        panelJail.SetActive(targetPanel == panelJail);

        // 切换面板时，顺便把提示框关了，防止它卡在屏幕上
        if (panelNoStamina != null) panelNoStamina.SetActive(false);
    }

    void UpdateStaminaUI()
    {
        if (PlayerBaseData.Instance == null) return;
        staminaText.text = $"Stamina: {(int)PlayerBaseData.Instance.currentStamina} / {PlayerBaseData.Instance.maxStamina}";
    }

    // --- 进入逻辑 ---

    void EnterGrindGame()
    {
        var db = PlayerBaseData.Instance;
        if (db.currentStamina >= minGrindStamina)
        {
            db.isTowerMode = false;
            SceneManager.LoadScene(grindSceneName);
        }
        else
        {
            Debug.Log("体力不足！");
            // 【修改点】调用闪烁提示
            StartCoroutine(ShowWarning());
        }
    }

    void EnterTowerGame()
    {
        var db = PlayerBaseData.Instance;
        if (db.currentStamina >= enterTowerCost)
        {
            db.currentStamina -= enterTowerCost;
            db.isTowerMode = true;
            db.targetLevelID = 1;
            SceneManager.LoadScene(towerSceneName);
        }
        else
        {
            Debug.Log($"体力不足！");
            // 【修改点】调用闪烁提示
            StartCoroutine(ShowWarning());
        }
    }

    // 【新增】显示1秒后自动消失的协程
    IEnumerator ShowWarning()
    {
        if (panelNoStamina != null)
        {
            panelNoStamina.SetActive(true); // 显示
            yield return new WaitForSeconds(1f); // 等待1秒
            panelNoStamina.SetActive(false); // 隐藏
        }
    }
}