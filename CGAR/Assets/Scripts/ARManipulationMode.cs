using UnityEngine;

public enum ManipulationMode
{
    None,
    Move,
    Rotate
}

public class ARManipulationMode : MonoBehaviour
{
    public static ARManipulationMode Instance;

    private Transform target;
    public ManipulationMode mode = ManipulationMode.None;

    void Awake()
    {
        Instance = this;
    }

    public void SetTarget(Transform t)
    {
        target = t;
        mode = ManipulationMode.None;
    }

    public void ClearTarget()
    {
        target = null;
        mode = ManipulationMode.None;
    }

    public void SetMoveMode()
    {
        mode = ManipulationMode.Move;
    }

    public void SetRotateMode()
    {
        mode = ManipulationMode.Rotate;
    }

    void Update()
    {
        if (target == null) return;
        if (Input.touchCount != 1) return;

        Touch t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Moved) return;

        if (mode == ManipulationMode.Move)
        {
            target.position += new Vector3(
                t.deltaPosition.x * 0.001f,
                0,
                t.deltaPosition.y * 0.001f
            );
        }
        else if (mode == ManipulationMode.Rotate)
        {
            target.Rotate(0, -t.deltaPosition.x * 0.5f, 0);
        }
    }
}
