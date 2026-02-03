using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private bool isMoving = true;

    void Awake()
    {
        // 监听玩家状态
        GameEventManager.OnPlayerStateChanged += HandleStateChange;
    }

    void OnDestroy()
    {
        GameEventManager.OnPlayerStateChanged -= HandleStateChange;
    }

    void HandleStateChange(PlayerState state)
    {
        // 玩家移动 -> 我也移动
        // 玩家攻击 -> 我停下挨打
        isMoving = (state == PlayerState.Moving);
    }

    void Update()
    {
        if (isMoving)
        {
            // 向左移动 (Vector3.left 就是 x = -1)
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

            // 如果跑出屏幕太远（比如x < -10），销毁自己，节省性能
            if (transform.position.x < -15f)
            {
                Destroy(gameObject);
            }
        }
    }
}