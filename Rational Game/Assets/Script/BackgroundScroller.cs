using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // 在 Inspector 面板中可以自由调整移动速度
    [Range(0, 5f)]
    public float scrollSpeed = 0.5f;

    private Material targetMaterial;
    private Vector2 offset;

    void Start()
    {
        // 获取当前物体渲染器的材质
        targetMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // 根据时间和速度计算偏移量
        // Time.deltaTime 确保在不同帧率下移动速度一致
        float xOffset = Time.time * scrollSpeed;

        // 更新材质的偏移，Vector2(水平偏移, 垂直偏移)
        // 如果是横向滚动，修改 X；纵向滚动，修改 Y
        offset = new Vector2(xOffset, 0);

        targetMaterial.mainTextureOffset = offset;
    }
}