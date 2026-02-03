using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("基础设置")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 3f; // 几秒刷一次波次

    [Header("刷怪数量池 (伪随机)")]
    // 在Inspector里填入数字，比如 [1, 1, 1, 2, 3]
    // 这样抽到 1 的概率就是 60%，抽到 3 的概率只有 20%
    public int[] spawnCountPool = { 1 };

    [Header("怪物间距")]
    public float enemySpacing = 1.5f; // 每个怪之间隔开多少米

    private float timer;
    private bool canSpawn = true;

    void Awake()
    {
        GameEventManager.OnPlayerStateChanged += HandleStateChange;
    }

    void OnDestroy()
    {
        GameEventManager.OnPlayerStateChanged -= HandleStateChange;
    }

    void HandleStateChange(PlayerState state)
    {
        canSpawn = (state == PlayerState.Moving);
    }

    void Update()
    {
        if (!canSpawn) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnWave(); // 这里改成生成“一波”
            timer = 0;
        }
    }

    // 生成一波怪物
    void SpawnWave()
    {
        if (enemyPrefab == null || spawnCountPool.Length == 0) return;

        // 1. 从数组里随机抽一个数字
        // Random.Range(min, max) 对于整数是“包头不包尾”的，所以要用 Length
        int randomIndex = Random.Range(0, spawnCountPool.Length);
        int countToSpawn = spawnCountPool[randomIndex];

        // 2. 循环生成
        for (int i = 0; i < countToSpawn; i++)
        {
            // 计算偏移量：第1个在 0，第2个在 1.5，第3个在 3.0 ...
            // Vector3.right 就是 (1, 0, 0)
            Vector3 offset = Vector3.right * (i * enemySpacing);
            Vector3 finalPos = spawnPoint.position + offset;

            CreateEnemy(finalPos);
        }

        Debug.Log($"生成了一波怪物，数量：{countToSpawn}");
    }

    // 单个怪物的具体生成逻辑
    void CreateEnemy(Vector3 pos)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        // 注入数据
        var db = PlayerBaseData.Instance;
        var enemyData = newEnemy.GetComponent<SingleEnemyData>();

        if (enemyData != null)
        {
            enemyData.Init(db.enemyTemplateHP, db.enemyTemplateATK, db.enemyTemplateXP, db.enemyTemplateGroupID);
        }
    }
}