#if UNITY_IOS
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Apple.ReplayKit;

public class ARScreenRecorder : MonoBehaviour
{
    public GameObject uiGroup;
    public Button recordButton;
    public Button stopButton;
    public Button exitButton;

    private bool ready = false;
    private bool isRecording = false;

    IEnumerator Start()
    {
        // 최소 2프레임 대기 (공식 OnGUI 효과)
        yield return null;
        yield return null;

        if (!ReplayKit.APIAvailable)
        {
            Debug.Log("ReplayKit not available");
            yield break;
        }

        ready = true;

        recordButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        exitButton.onClick.AddListener(ExitApp);

        stopButton.gameObject.SetActive(false);
    }

    void StartRecording()
    {
        if (!ready) return;
        if (isRecording) return;

        ReplayKit.StartRecording(false, false);

        uiGroup.SetActive(false);
        stopButton.gameObject.SetActive(true);
        isRecording = true;
    }

    void StopRecording()
    {
        if (!ready) return;
        if (!isRecording) return;

        ReplayKit.StopRecording();
        StartCoroutine(WaitAndPreview());
    }

    IEnumerator WaitAndPreview()
    {
        while (!ReplayKit.recordingAvailable)
            yield return null;

        uiGroup.SetActive(true);
        stopButton.gameObject.SetActive(false);
        isRecording = false;

        ReplayKit.Preview();
    }

    void ExitApp()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
#endif
