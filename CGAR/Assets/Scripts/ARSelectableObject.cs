using UnityEngine;

public class ARSelectableObject : MonoBehaviour
{
    private Renderer[] renderers;
    private Color[] originalColors;

    [Header("Select Color")]
    public Color selectedColor = Color.yellow;

    void Awake()
    {
        // 자기 자신 + 자식까지 포함한 모든 Renderer 수집
        renderers = GetComponentsInChildren<Renderer>();

        // 원래 색 저장
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    public void Select()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = selectedColor;
        }
    }

    public void Deselect()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }
}
