using UnityEngine;

using UnityEngine.XR;

public class VRObjectInteractableInitializer : MonoBehaviour
{
    void Awake()
    {
        // Rigidbody 추가
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;

        // XR Grab Interactable 추가
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = gameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null)
            grab = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Grab 설정
        grab.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.VelocityTracking; // 자연스러운 VR 이동
        grab.trackRotation = true;     // 회전 허용
        grab.trackPosition = true;     // 이동 허용
        grab.throwOnDetach = false;    // 던지기 비활성 (필요하면 on)

        // 두 손 조작 허용 (스케일 조정 가능)
        grab.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Multiple;

        Debug.Log("[VRObjectInteractableInitializer] Grab/Rotate 준비 완료: " + gameObject.name);
    }
}
