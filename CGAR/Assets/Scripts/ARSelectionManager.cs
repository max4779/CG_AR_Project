using UnityEngine;
using UnityEngine.EventSystems;

public class ARSelectionManager : MonoBehaviour
{
    public Camera arCamera;
    public GameObject editPanel;

    private ARSelectableObject current;

    void Start()
    {
        editPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Began) return;

        if (EventSystem.current.IsPointerOverGameObject(t.fingerId))
            return;

        Ray ray = arCamera.ScreenPointToRay(t.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ARSelectableObject obj = hit.collider.GetComponent<ARSelectableObject>();
            if (obj != null)
            {
                Select(obj);
                return;
            }
        }

        Deselect();
    }

    void Select(ARSelectableObject obj)
    {
        if (current != null)
            current.Deselect();

        current = obj;
        current.Select();
        editPanel.SetActive(true);

        ARManipulationMode.Instance.SetTarget(current.transform);
    }

    void Deselect()
    {
        if (current != null)
            current.Deselect();

        current = null;
        editPanel.SetActive(false);

        ARManipulationMode.Instance.ClearTarget();
    }
}
