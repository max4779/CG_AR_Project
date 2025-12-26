using UnityEngine;

public class ARSelectableObject : MonoBehaviour
{
    [Header("Selection Visual")]
    public Color selectedColor = new Color(1f, 1f, 0.6f); // 살짝 노란빛
    public float brightness = 1.3f;

    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();
        Deselect();
    }

    public void Select()
    {
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(mpb);

            // _Color 지원 셰이더 기준 (Standard / URP Lit 모두 가능)
            if (r.sharedMaterial != null && r.sharedMaterial.HasProperty("_Color"))
            {
                Color baseColor = r.sharedMaterial.color;
                Color finalColor = baseColor * brightness;
                finalColor = Color.Lerp(finalColor, selectedColor, 0.4f);

                mpb.SetColor("_Color", finalColor);
            }

            r.SetPropertyBlock(mpb);
        }
    }

    public void Deselect()
    {
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(mpb);

            // PropertyBlock 비우면 원래 머티리얼로 돌아감
            mpb.Clear();
            r.SetPropertyBlock(mpb);
        }
    }
}
