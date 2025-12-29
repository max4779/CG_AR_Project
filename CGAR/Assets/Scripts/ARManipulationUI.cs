using UnityEngine;

public class ARManipulationUI : MonoBehaviour
{
    public void OnUp()
    {
        ARManipulationMode.Instance.OnUp();
    }

    public void OnDown()
    {
        ARManipulationMode.Instance.OnDown();
    }

    public void OnLeft()
    {
        ARManipulationMode.Instance.OnLeft();
    }

    public void OnRight()
    {
        ARManipulationMode.Instance.OnRight();
    }

    // ===== 모드 전환 =====

    public void SetMoveMode()
    {
        ARManipulationMode.Instance.SetMoveMode();
    }

    public void SetRotateMode()
    {
        ARManipulationMode.Instance.SetRotateMode();
    }

    public void SetScaleMode()
    {
        ARManipulationMode.Instance.SetScaleMode();
    }
}
