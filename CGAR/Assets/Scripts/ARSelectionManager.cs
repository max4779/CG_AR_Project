using UnityEngine;
using UnityEngine.EventSystems;

public class ARSelectionManager : MonoBehaviour
{
    public GameObject editPanel;

    private ARSelectableObject current;

    void Start()
    {
        if (editPanel != null)
            editPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Began)
            return;

        Ray ray = Camera.main.ScreenPointToRay(t.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log("🎯 Ray hit: " + hit.collider.gameObject.name);

            // 🔴 핵심 수정 포인트
            ARSelectableObject obj =
                hit.collider.GetComponentInParent<ARSelectableObject>();

            if (obj != null)
            {
                Debug.Log("✅ Selectable found");
                Select(obj);
                return;
            }
            else
            {
                Debug.Log("❌ Hit but no ARSelectableObject in parents");
            }
        }
        else
        {
            Debug.Log("❌ Raycast missed");
        }
    }



    void Select(ARSelectableObject obj)
    {
        if (current != null)
            current.Deselect();

        current = obj;
        current.Select();

        if (editPanel != null)
            editPanel.SetActive(true);

        ARManipulationMode.Instance.SetTarget(current.transform);
    }

    void Deselect()
    {
        if (current != null)
            current.Deselect();

        current = null;

        if (editPanel != null)
            editPanel.SetActive(false);

        ARManipulationMode.Instance.ClearTarget();
    }
}
