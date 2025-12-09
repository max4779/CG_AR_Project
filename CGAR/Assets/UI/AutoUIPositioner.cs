using UnityEngine;

public class AutoUIPositioner : MonoBehaviour
{
    [Header("Buttons")]
    public RectTransform recordButton;     // 녹화 시작 버튼
    public RectTransform stopButton;       // 녹화 중단 버튼 (recordButton과 동일 위치)
    public RectTransform exitButton;       // 앱 종료 버튼 (왼쪽 아래)

    [Header("Button Settings")]
    public float buttonSize = 160f;
    public float padding = 60f;    // 오른쪽/왼쪽 여백
    public float midOffset = 0f;   // 중앙 기준 위/아래 미세 조정

    private ScreenOrientation lastOrientation;

    void Start()
    {
        lastOrientation = Screen.orientation;
        ApplyLayout();
    }

    void Update()
    {
        // 화면 회전 감지
        if (Screen.orientation != lastOrientation)
        {
            lastOrientation = Screen.orientation;
            ApplyLayout();
        }
    }

    void ApplyLayout()
    {
        float w = Screen.width;
        float h = Screen.height;

        // 화면 비율에 따라 버튼 크기 조절
        float scaleFactor = Mathf.Clamp(h / 1500f, 0.75f, 1.3f);
        float finalSize = buttonSize * scaleFactor;
        Vector2 sizeDelta = new Vector2(finalSize, finalSize);

        // ======================================================
        // Exit Button — 왼쪽 아래
        // ======================================================
        if (exitButton != null)
        {
            exitButton.anchorMin = new Vector2(0f, 0f);
            exitButton.anchorMax = new Vector2(0f, 0f);
            exitButton.pivot = new Vector2(0.5f, 0.5f);

            exitButton.sizeDelta = sizeDelta;
            exitButton.anchoredPosition = new Vector2(padding, padding);
        }

        // ======================================================
        // Record Button — 오른쪽 중단
        // ======================================================
        if (recordButton != null)
        {
            recordButton.anchorMin = new Vector2(1f, 0.5f);
            recordButton.anchorMax = new Vector2(1f, 0.5f);
            recordButton.pivot = new Vector2(0.5f, 0.5f);

            recordButton.sizeDelta = sizeDelta;
            recordButton.anchoredPosition = new Vector2(-padding, midOffset);
        }

        // ======================================================
        // Stop Button — Record Button 위치와 동일
        // ======================================================
        if (stopButton != null)
        {
            stopButton.anchorMin = new Vector2(1f, 0.5f);
            stopButton.anchorMax = new Vector2(1f, 0.5f);
            stopButton.pivot = new Vector2(0.5f, 0.5f);

            stopButton.sizeDelta = sizeDelta;
            stopButton.anchoredPosition = new Vector2(-padding, midOffset);
        }
    }
}
