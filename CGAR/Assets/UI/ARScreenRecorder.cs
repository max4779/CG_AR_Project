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

    private bool isRecording = false;
    private bool isStopping = false;
    private Coroutine stopRoutine = null;

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
        if (!ReplayKit.APIAvailable) return;
        if (isRecording) return;
        if (isStopping) return;
        if (ReplayKit.isRecording) return;

        ReplayKit.StartRecording(false, false);

        uiGroup.SetActive(false);
        stopButton.gameObject.SetActive(true);

        isRecording = true;
    }

    void StopRecording()
    {
        if (!ReplayKit.APIAvailable) return;
        if (!isRecording) return;
        if (isStopping) return;

        isStopping = true;

        ReplayKit.StopRecording();

        if (stopRoutine != null)
            StopCoroutine(stopRoutine);

        stopRoutine = StartCoroutine(WaitForRecordingThenPreview());
    }

    private IEnumerator WaitForRecordingThenPreview()
    {
        float timeout = 10f;
        float t = 0f;

        while (!ReplayKit.recordingAvailable && t < timeout)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        uiGroup.SetActive(true);
        stopButton.gameObject.SetActive(false);

        isRecording = false;
        isStopping = false;
        stopRoutine = null;

        if (!ReplayKit.recordingAvailable)
        {
            Debug.LogError("ReplayKit recording is not available for preview (timeout).");
            yield break;
        }

        bool opened = ReplayKit.Preview();
        if (!opened)
        {
            Debug.LogError("ReplayKit.Preview() returned false.");
        }
    }

    void ExitApp()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
