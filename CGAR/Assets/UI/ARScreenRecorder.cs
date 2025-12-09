using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Apple.ReplayKit;

public class ARScreenRecorder : MonoBehaviour
{
    public GameObject uiGroup;
    public Button recordButton;
    public Button stopButton;
    public Button exitButton;

    private bool isRecording = false;

    void Start()
    {
        if (!ReplayKit.APIAvailable)
        {
            Debug.Log("ReplayKit is not available on this device.");
            return;
        }

        recordButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        exitButton.onClick.AddListener(ExitApp);

        stopButton.gameObject.SetActive(false);
    }

    void StartRecording()
    {
        if (!ReplayKit.APIAvailable)
            return;

        ReplayKit.StartRecording(false, false); // 마이크/카메라 X

        uiGroup.SetActive(false);
        stopButton.gameObject.SetActive(true);

        isRecording = true;
    }

    void StopRecording()
    {
        if (!ReplayKit.APIAvailable)
            return;

        ReplayKit.StopRecording();

        uiGroup.SetActive(true);
        stopButton.gameObject.SetActive(false);

        isRecording = false;

        // 이 시점에서 video가 저장됨 (Preview 없이 자동 저장 X)
        // Preview를 띄우고 싶으면:
        ReplayKit.Preview();
    }

    void ExitApp()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Application.Quit();
#endif
    }
}

위 코드에서 해당 부분만 수정해봐
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Apple.ReplayKit;

public class ARScreenRecorder : MonoBehaviour
{
    public GameObject uiGroup;
    public Button recordButton;
    public Button stopButton;
    public Button exitButton;

    private bool isRecording = false;

    void Start()
    {
        if (!ReplayKit.APIAvailable)
        {
            Debug.Log("ReplayKit is not available on this device.");
            return;
        }

        recordButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        exitButton.onClick.AddListener(ExitApp);

        stopButton.gameObject.SetActive(false);
    }

    void StartRecording()
    {
        if (!ReplayKit.APIAvailable)
            return;

        ReplayKit.StartRecording(false, false); // 마이크/카메라 X

        uiGroup.SetActive(false);
        stopButton.gameObject.SetActive(true);

        isRecording = true;
    }

    void StopRecording()
    {
        if (!ReplayKit.APIAvailable)
            return;

        ReplayKit.StopRecording();

        uiGroup.SetActive(true);
        stopButton.gameObject.SetActive(false);

        isRecording = false;

        // 이 시점에서 video가 저장됨 (Preview 없이 자동 저장 X)
        // Preview를 띄우고 싶으면:
        ReplayKit.Preview();
    }

    void ExitApp()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
