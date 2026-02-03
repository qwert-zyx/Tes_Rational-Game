using UnityEngine;

public class BackgroundScroller2D : MonoBehaviour
{
    [Header("滚动速度")]
    public float scrollSpeed = 2f;

    private SpriteRenderer spriteRenderer;
    private Vector2 startSize;
    private bool isScrolling = true; // 默认一开始是滚动的

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 记录一开始图片的大小
        startSize = spriteRenderer.size;

        // 【核心】：监听玩家状态改变信号
        GameEventManager.OnPlayerStateChanged += HandleStateChange;
    }

    void OnDestroy()
    {
        // 记得取消监听，防止报错
        GameEventManager.OnPlayerStateChanged -= HandleStateChange;
    }

    // 当收到信号时，执行这个逻辑
    void HandleStateChange(PlayerState state)
    {
        if (state == PlayerState.Moving)
        {
            isScrolling = true;
        }
        else if (state == PlayerState.Attacking)
        {
            isScrolling = false;
        }
    }

    void Update()
    {
        // 如果允许滚动
        if (isScrolling)
        {
            // 计算新的宽度： 原宽度 + (速度 * 时间)
            float newX = spriteRenderer.size.x + (scrollSpeed * Time.deltaTime);

            // 应用新的尺寸
            spriteRenderer.size = new Vector2(newX, startSize.y);

            // 【防溢出逻辑】
            // 如果图片拉伸超过了原始大小的2倍，就把它“弹”回去
            // 这样由于图片是无缝平铺的，肉眼看不出回弹，看起来像无限滚动
            if (spriteRenderer.size.x > startSize.x * 2)
            {
                spriteRenderer.size = new Vector2(startSize.x, startSize.y);
            }
        }
    }
}