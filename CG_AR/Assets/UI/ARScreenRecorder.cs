using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용 시 필요
using System.Runtime.InteropServices; 

public class ARScreenRecorder : MonoBehaviour
{
    // StartRecording: iOS 네이티브 녹화 시작을 호출한다
    [DllImport("__Internal")] private static extern void StartRecording();

    // StopRecording: iOS 네이티브 녹화 중지를 호출한다
    [DllImport("__Internal")] private static extern void StopRecording();

    // ShowRECIndicator: iOS 네이티브 REC 아이콘을 표시한다
    [DllImport("__Internal")] private static extern void ShowRECIndicator();

    // HideRECIndicator: iOS 네이티브 REC 아이콘을 숨긴다
    [DllImport("__Internal")] private static extern void HideRECIndicator();

    // isRecording: 녹화 여부를 관리한다
    private bool isRecording = false;

    // uiGroup: 전체 Unity UI를 관리한다
    public GameObject uiGroup;

    // recordButton: 녹화 버튼을 관리한다
    public Button recordButton;

    // exitButton: 종료 버튼을 관리한다
    public Button exitButton;

    // recordingTimerText: 녹화 시간을 표시하는 텍스트를 관리한다
    public TextMeshProUGUI recordingTimerText;

    // timer: 녹화 시간을 누적한다
    private float timer = 0f;


    // Start: UI 초기 설정을 수행한다
    void Start()
    {
        recordButton.onClick.AddListener(ToggleRecording);
        exitButton.onClick.AddListener(ExitApp);

        recordingTimerText.gameObject.SetActive(false);
    }


    // ToggleRecording: 녹화 시작 또는 중지를 수행한다
    void ToggleRecording()
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (!isRecording)
        {
            HideUI();
            StartRecording();
            ShowRECIndicator();
            StartTimer();
            isRecording = true;
        }
        else
        {
            StopRecording();
            HideRECIndicator();
            ShowUI();
            StopTimer();
            isRecording = false;
        }
#endif
    }


    // HideUI: 전체 Unity UI를 숨긴다
    void HideUI()
    {
        uiGroup.SetActive(false);
    }


    // ShowUI: 전체 Unity UI를 표시한다
    void ShowUI()
    {
        uiGroup.SetActive(true);
    }


    // StartTimer: 녹화 시간을 측정하기 시작한다
    void StartTimer()
    {
        timer = 0f;
        recordingTimerText.text = "00:00";
        recordingTimerText.gameObject.SetActive(true);
        InvokeRepeating(nameof(UpdateTimer), 1f, 1f);
    }


    // StopTimer: 녹화 시간 측정을 종료한다
    void StopTimer()
    {
        CancelInvoke(nameof(UpdateTimer));
        recordingTimerText.gameObject.SetActive(false);
    }


    // UpdateTimer: 녹화 시간을 갱신한다
    void UpdateTimer()
    {
        timer += 1f;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        recordingTimerText.text = $"{minutes:00}:{seconds:00}";
    }


    // ExitApp: 애플리케이션을 종료한다
    void ExitApp()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
