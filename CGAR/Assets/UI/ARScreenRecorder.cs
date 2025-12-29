using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_IOS
using UnityEngine.Apple.ReplayKit;
#endif

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
#if UNITY_IOS
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
#else
        Debug.Log("ARScreenRecorder: iOS only");
        yield break;
#endif
    }

#if UNITY_IOS
    void StartRecording()
    {
        if (!ready || isRecording) return;
        ReplayKit.StartRecording(false, false);
        uiGroup.SetActive(false);
        stopButton.gameObject.SetActive(true);
        isRecording = true;
    }

    void StopRecording()
    {
        if (!ready || !isRecording) return;
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
#endif

    void ExitApp()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
