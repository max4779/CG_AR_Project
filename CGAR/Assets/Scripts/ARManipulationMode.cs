using UnityEngine;

public enum ManipulationMode
{
    Move,
    Rotate,
    Scale
}

public class ARManipulationMode : MonoBehaviour
{
    public static ARManipulationMode Instance;

    private Transform target;

    [Header("Step Settings")]
    public float moveStep = 0.05f;
    public float rotateStep = 10f;
    public float scaleStep = 0.05f;

    public ManipulationMode currentMode = ManipulationMode.Move;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public void ClearTarget()
    {
        target = null;
    }

    // =========================
    // 방향 버튼 (공통)
    // =========================

    public void OnUp()
    {
        if (target == null) return;

        switch (currentMode)
        {
            case ManipulationMode.Move:
                MoveForward();
                break;
            case ManipulationMode.Rotate:
                RotateUp();
                break;
            case ManipulationMode.Scale:
                ScaleUp();
                break;
        }
    }

    public void OnDown()
    {
        if (target == null) return;

        switch (currentMode)
        {
            case ManipulationMode.Move:
                MoveBackward();
                break;
            case ManipulationMode.Rotate:
                RotateDown();
                break;
            case ManipulationMode.Scale:
                ScaleDown();
                break;
        }
    }

    public void OnLeft()
    {
        if (target == null) return;

        switch (currentMode)
        {
            case ManipulationMode.Move:
                MoveLeft();
                break;
            case ManipulationMode.Rotate:
                RotateLeft();
                break;
            case ManipulationMode.Scale:
                ScaleDown();
                break;
        }
    }

    public void OnRight()
    {
        if (target == null) return;

        switch (currentMode)
        {
            case ManipulationMode.Move:
                MoveRight();
                break;
            case ManipulationMode.Rotate:
                RotateRight();
                break;
            case ManipulationMode.Scale:
                ScaleUp();
                break;
        }
    }

    // =========================
    // 이동 (Move)
    // =========================

    void MoveForward()
    {
        Vector3 dir = Camera.main.transform.forward;
        dir.y = 0;
        target.position += dir.normalized * moveStep;
    }

    void MoveBackward()
    {
        Vector3 dir = Camera.main.transform.forward;
        dir.y = 0;
        target.position -= dir.normalized * moveStep;
    }

    void MoveLeft()
    {
        Vector3 dir = Camera.main.transform.right;
        dir.y = 0;
        target.position -= dir.normalized * moveStep;
    }

    void MoveRight()
    {
        Vector3 dir = Camera.main.transform.right;
        dir.y = 0;
        target.position += dir.normalized * moveStep;
    }

    // =========================
    // 회전 (Rotate)
    // =========================

    void RotateLeft()
    {
        target.Rotate(0, -rotateStep, 0, Space.World);
    }

    void RotateRight()
    {
        target.Rotate(0, rotateStep, 0, Space.World);
    }

    void RotateUp()
    {
        target.Rotate(-rotateStep, 0, 0, Space.World);
    }

    void RotateDown()
    {
        target.Rotate(rotateStep, 0, 0, Space.World);
    }

    // =========================
    // 크기 조절 (Scale)
    // =========================

    void ScaleUp()
    {
        Vector3 newScale = target.localScale + Vector3.one * scaleStep;
        target.localScale = ClampScale(newScale);
    }

    void ScaleDown()
    {
        Vector3 newScale = target.localScale - Vector3.one * scaleStep;
        target.localScale = ClampScale(newScale);
    }

    Vector3 ClampScale(Vector3 scale)
    {
        float min = 0.1f;
        float max = 5.0f;

        scale.x = Mathf.Clamp(scale.x, min, max);
        scale.y = Mathf.Clamp(scale.y, min, max);
        scale.z = Mathf.Clamp(scale.z, min, max);

        return scale;
    }

    // =========================
    // 모드 전환
    // =========================

    public void SetMoveMode()
    {
        currentMode = ManipulationMode.Move;
    }

    public void SetRotateMode()
    {
        currentMode = ManipulationMode.Rotate;
    }

    public void SetScaleMode()
    {
        currentMode = ManipulationMode.Scale;
    }
}
