using UnityEngine;

public class AutoUIScaler : MonoBehaviour
{
    // reference size (iPad Pro 11" 기준)
    public float referenceWidth = 1668f;

    // desired button size on reference device
    public Vector2 referenceSize = new Vector2(260f, 120f);

    // edge margin on reference device
    public Vector2 referenceMargin = new Vector2(100f, 80f);

    // min/max scale clamp
    public float minScale = 0.7f;
    public float maxScale = 1.3f;

    // Start: 버튼 크기와 위치를 탄력적으로 조절한다
    void Awake()
    {
        RectTransform rt = GetComponent<RectTransform>();

        float scale = Screen.width / referenceWidth;

        // 스케일을 클램프하여 과도하게 커지거나 작아지지 않게 함
        scale = Mathf.Clamp(scale, minScale, maxScale);

        // 크기 적용
        rt.sizeDelta = referenceSize * scale;

        // 위치 조정
        Vector2 anchoredPos = rt.anchoredPosition;
        anchoredPos.x = referenceMargin.x * scale * Mathf.Sign(anchoredPos.x);
        anchoredPos.y = referenceMargin.y * scale * Mathf.Sign(anchoredPos.y);
        rt.anchoredPosition = anchoredPos;
    }
}
