using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageFilter : MonoBehaviour
{

    public Material filter;
    [Range(0f, 1f)] public float should;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        filter.SetFloat("_Should", should);
        Graphics.Blit(source, destination, filter);
    }
}
