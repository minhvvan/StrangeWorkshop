using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkewApplier : MonoBehaviour
{
    [SerializeField] Shader skewImageShader;  // Image용 Skew Shader
    [SerializeField] Shader skewTextShader;   // TMP_Text용 Skew Shader

    [SerializeField] float targetSkewX = 0.5f;

    void Start()
    {
        ApplySkewToChildren(transform);
    }

    void ApplySkewToChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Image img = child.GetComponent<Image>();
            if (img != null && skewImageShader != null)
            {
                img.material = new Material(skewImageShader);
                img.material.SetFloat("_SkewX", targetSkewX);
            }

            TMP_Text text = child.GetComponent<TMP_Text>();
            if (text != null && skewTextShader != null)
            {
                Material newFontMaterial = new Material(text.fontSharedMaterial);
                newFontMaterial.shader = skewTextShader;
                newFontMaterial.SetFloat("_SkewX", targetSkewX);

                newFontMaterial.mainTexture = text.fontSharedMaterial.mainTexture;

                if (text.fontSharedMaterial.HasProperty("_OutlineTex"))
                {
                    newFontMaterial.SetTexture("_OutlineTex", text.fontSharedMaterial.GetTexture("_OutlineTex"));
                }

                text.fontMaterial = newFontMaterial;
                text.UpdateMeshPadding();
                text.ForceMeshUpdate();
            }

            ApplySkewToChildren(child);
        }
    }

    void OnValidate()
    {
        ApplySkewToChildren(transform);
    }
}