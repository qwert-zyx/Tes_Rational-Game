using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 3f;
    public int[] spawnCountPool = { 1 };

    private float timer;
    private bool canSpawn = false; // 默认设为 false，看它会不会变 true

    void Awake()
    {
        GameEventManager.OnPlayerStateChanged += HandleStateChange;
        Debug.Log("【侦探 1】Spawner Awake 执行了，开始监听信号");
    }

    void Start()
    {
        // 这里做一个保险：直接看看玩家现在的状态
        // 如果 Spawner 醒来晚了，错过了广播，这里可以手动补救
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null && player.currentState == PlayerState.Moving)
        {
            Debug.Log("【侦探 2】Spawner Start 检测到玩家已经在移动了，强制开启刷怪");
            canSpawn = true;
        }
        else
        {
            Debug.Log("【侦探 2】Spawner Start 没检测到玩家或玩家没动。");
        }
    }

    void OnDestroy()
    {
        GameEventManager.OnPlayerStateChanged -= HandleStateChange;
    }

    void HandleStateChange(PlayerState state)
    {
        // 打印一下接收到的状态
        Debug.Log($"【侦探 3】Spawner 收到信号: {state}");
        canSpawn = (state == PlayerState.Moving);
    }

    void Update()
    {
        // 如果不能刷，就不执行后面，打印一下原因（为了防止刷屏，只在第一次被阻挡时打印可以，但这里为了调试先简单处理）
        if (!canSpawn) return;

        // 检查预制体是否存在
        if (enemyPrefab == null)
        {
            Debug.LogError("【严重凶手】Enemy Prefab 居然是空的！请检查 Inspector 面板！");
            canSpawn = false;
            return;
        }

        timer += Time.deltaTime;

        // 打印 timer 看看时间走没走（这会刷屏，调试完记得删掉）
        // Debug.Log($"【侦探 4】Timer: {timer}"); 

        if (timer >= spawnInterval)
        {
            Debug.Log("【侦探 5】时间到！准备生成一波怪物");
            SpawnWave();
            timer = 0;
        }
    }

    void SpawnWave()
    {
        // 原有逻辑...
        if (spawnCountPool.Length == 0) return;
        int randomIndex = Random.Range(0, spawnCountPool.Length);
        int countToSpawn = spawnCountPool[randomIndex];

        for (int i = 0; i < countToSpawn; i++)
        {
            Vector3 offset = Vector3.right * (i * 1.5f);

            // 检查生成点
            Vector3 finalPos = (spawnPoint != null) ? spawnPoint.position + offset : transform.position + offset;

            Instantiate(enemyPrefab, finalPos, Quaternion.identity);
        }
        Debug.Log($"【侦探 6】成功生成了 {countToSpawn} 只怪");
    }
}