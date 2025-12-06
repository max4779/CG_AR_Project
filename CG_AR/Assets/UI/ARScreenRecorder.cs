using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class ARScreenRecorder : MonoBehaviour
{
    // iOS AVAssetWriter 네이티브 함수 연결
    [DllImport("__Internal")] private static extern void StartCGRecord();
    [DllImport("__Internal")] private static extern void StopCGRecord();

    // 녹화 상태
    private bool isRecording = false;

    // UI 그룹
    public GameObject uiGroup;

    // 버튼
    public Button recordButton;
    public Button exitButton;

    // 타이머 표시용 TMP Text
    public TextMeshProUGUI recordingTimerText;

    // 타이머
    private float timer = 0f;

    void Start()
    {
        recordButton.onClick.AddListener(ToggleRecording);
        exitButton.onClick.AddListener(ExitApp);

        recordingTimerText.gameObject.SetActive(false);
    }

    void ToggleRecording()
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (!isRecording)
        {
            // Unity UI 숨김 → 화면에 보이지 않음 → 녹화에 포함 X
            HideUI();

            // iOS 네이티브 녹화 시작 (AR 화면만 녹화)
            StartCGRecord();

            StartTimer();
            isRecording = true;
        }
        else
        {
            // 네이티브 녹화 종료
            StopCGRecord();

            // Unity UI 다시 표시
            ShowUI();

            StopTimer();
            isRecording = false;
        }
#endif
    }

    void HideUI()
    {
        uiGroup.SetActive(false);
    }

    void ShowUI()
    {
        uiGroup.SetActive(true);
    }

    void StartTimer()
    {
        timer = 0f;
        recordingTimerText.text = "00:00";
        recordingTimerText.gameObject.SetActive(true);
        InvokeRepeating(nameof(UpdateTimer), 1f, 1f);
    }

    void StopTimer()
    {
        CancelInvoke(nameof(UpdateTimer));
        recordingTimerText.gameObject.SetActive(false);
    }

    void UpdateTimer()
    {
        timer += 1f;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        recordingTimerText.text = $"{minutes:00}:{seconds:00}";
    }

    void ExitApp()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
